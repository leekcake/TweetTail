using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using FFImageLoading.Cache;
using FFImageLoading.Config;
using FFImageLoading.Helpers;
using System.Collections.Concurrent;


namespace FFImageLoading.Cache
{
    public class SimpleDiskCache : IDiskCache
    {
        readonly SemaphoreSlim fileWriteLock = new SemaphoreSlim(1, 1);
        readonly SemaphoreSlim _currentWriteLock = new SemaphoreSlim(1, 1);
        Task initTask = null;
        readonly string cacheFolderName;
        readonly string rootFolder;
        string cacheFolder;
        ConcurrentDictionary<string, byte> fileWritePendingTasks = new ConcurrentDictionary<string, byte>();
        ConcurrentDictionary<string, CacheEntry> entries = new ConcurrentDictionary<string, CacheEntry>();
        Task _currentWrite = Task.FromResult<byte>(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDiskCache"/> class. This constructor attempts
        /// to create a folder of the given name under the <see cref="ApplicationData.TemporaryFolder"/>.
        /// </summary>
        /// <param name="cacheFolderName">The name of the cache folder.</param>
        /// <param name="configuration">The configuration object.</param>
        public SimpleDiskCache(string cacheFolderName, Configuration configuration)
        {
            Configuration = configuration;
            this.cacheFolderName = cacheFolderName;
            initTask = Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDiskCache"/> class. This constructor attempts
        /// to create a folder of the given name under the given root <see cref="StorageFolder"/>.
        /// </summary>
        /// <param name="rootFolder">The root folder where the cache folder will be created.</param>
        /// <param name="cacheFolderName">The cache folder name.</param>
        /// <param name="configuration">The configuration object.</param>
        public SimpleDiskCache(string rootFolder, string cacheFolderName, Configuration configuration)
        {
            Configuration = configuration;
            this.rootFolder = rootFolder ?? Path.GetTempPath();
            this.cacheFolderName = cacheFolderName;
            initTask = Init();
        }

        protected Configuration Configuration { get; private set; }
        protected IMiniLogger Logger { get { return Configuration.Logger; } }

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        protected virtual async Task Init()
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        {
            cacheFolder = Path.Combine(rootFolder, cacheFolderName);
            Directory.CreateDirectory(cacheFolder);
            InitializeEntries();
            var task = CleanCallback();
        }

        protected virtual void InitializeEntries()
        {
            foreach (var file in Directory.GetFiles(cacheFolder))
            {
                string key = Path.GetFileNameWithoutExtension(file);
                var duration = GetDuration(Path.GetExtension(file));
                entries.TryAdd(key, new CacheEntry(File.GetCreationTimeUtc(file), duration, Path.GetFileName(file)));
            }
        }

        protected virtual TimeSpan GetDuration(string text)
        {
            string textToParse = text.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(textToParse))
                return Configuration.DiskCacheDuration;

            return int.TryParse(textToParse, out int duration) ? TimeSpan.FromSeconds(duration) : Configuration.DiskCacheDuration;
        }
        
        protected virtual async Task CleanCallback()
        {
            KeyValuePair<string, CacheEntry>[] kvps;
            var now = DateTime.UtcNow;
            kvps = entries.Where(kvp => kvp.Value.Origin + kvp.Value.TimeToLive < now).ToArray();

            foreach (var kvp in kvps)
            {
                if (entries.TryRemove(kvp.Key, out CacheEntry oldCacheEntry))
                {
                    try
                    {
                        Logger.Debug(string.Format("SimpleDiskCache: Removing expired file {0}", kvp.Key));
                        File.Delete(oldCacheEntry.FileName);
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// GetFilePath
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<string> GetFilePathAsync(string key)
        {
            await initTask.ConfigureAwait(false);

            if (!entries.TryGetValue(key, out CacheEntry entry))
                return null;

            return Path.Combine(cacheFolder, entry.FileName);
        }

        /// <summary>
        /// Checks if cache entry exists/
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="key">Key.</param>
        public virtual async Task<bool> ExistsAsync(string key)
        {
            await initTask.ConfigureAwait(false);

            return entries.ContainsKey(key);
        }

        /// <summary>
        /// Adds the file to cache and file saving queue if not exists.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="bytes">Bytes.</param>
        /// <param name="duration">Duration.</param>
        public virtual async Task AddToSavingQueueIfNotExistsAsync(string key, byte[] bytes, TimeSpan duration, Action writeFinished = null)
        {
            await initTask.ConfigureAwait(false);

            if (!fileWritePendingTasks.TryAdd(key, 1))
                return;

            await _currentWriteLock.WaitAsync().ConfigureAwait(false); // Make sure we don't add multiple continuations to the same task

            try
            {
                _currentWrite = _currentWrite.ContinueWith(async t =>
                {
                    await Task.Yield(); // forces it to be scheduled for later

                    await initTask.ConfigureAwait(false);

                    try
                    {
                        await fileWriteLock.WaitAsync().ConfigureAwait(false);

                        cacheFolder = Path.Combine(rootFolder, cacheFolderName);
                        string filename = key + "." + (long)duration.TotalSeconds;

                        var file = Path.Combine(cacheFolder, filename);

                        using (var fs = new FileStream(file, FileMode.OpenOrCreate))
                        {
                            await fs.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                        }                        

                        entries[key] = new CacheEntry(DateTime.UtcNow, duration, filename);
                        writeFinished?.Invoke();
                    }
                    catch (Exception ex) // Since we don't observe the task (it's not awaited, we should catch all exceptions)
                    {
                        //TODO WinRT doesn't have Console
                        System.Diagnostics.Debug.WriteLine(string.Format("An error occured while caching to disk image '{0}'.", key));
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                    finally
                    {
                        fileWritePendingTasks.TryRemove(key, out byte finishedTask);
                        fileWriteLock.Release();
                    }
                });
            }
            finally
            {
                _currentWriteLock.Release();
            }
        }

        /// <summary>
        /// Tries to get cached file as stream.
        /// </summary>
        /// <returns>The get async.</returns>
        /// <param name="key">Key.</param>
        public virtual async Task<Stream> TryGetStreamAsync(string key)
        {
            await initTask.ConfigureAwait(false);

            await WaitForPendingWriteIfExists(key).ConfigureAwait(false);

            try
            {
                if (!entries.TryGetValue(key, out CacheEntry entry))
                    return null;

                return new FileStream(Path.Combine(cacheFolder, entry.FileName), FileMode.Open);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Removes the specified cache entry.
        /// </summary>
        /// <param name="key">Key.</param>
        public virtual async Task RemoveAsync(string key)
        {
            await initTask.ConfigureAwait(false);

            await WaitForPendingWriteIfExists(key).ConfigureAwait(false);

            if (entries.TryRemove(key, out CacheEntry oldCacheEntry))
            {
                try
                {
                    File.Delete(Path.Combine(cacheFolder, oldCacheEntry.FileName));
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Clears all cache entries.
        /// </summary>
        public virtual async Task ClearAsync()
        {
            await initTask.ConfigureAwait(false);

            while (fileWritePendingTasks.Count != 0)
            {
                await Task.Delay(20).ConfigureAwait(false);
            }

            try
            {
                await fileWriteLock.WaitAsync().ConfigureAwait(false);
                foreach (var item in Directory.GetFiles(cacheFolder) )
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (FileNotFoundException)
                    {
                    }
                }
            }
            catch (IOException)
            {
                
            }
            finally
            {
                entries.Clear();
                fileWriteLock.Release();
            }
        }

        protected virtual async Task WaitForPendingWriteIfExists(string key)
        {
            while (fileWritePendingTasks.ContainsKey(key))
            {
                await Task.Delay(20).ConfigureAwait(false);
            }
        }
    }
}