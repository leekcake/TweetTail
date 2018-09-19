using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Library.Manager;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net.Http;

namespace Library.Container.Media
{
    public abstract class Requester
    {
        internal string cacheDir;

        private MediaManager owner;

        public Requester(MediaManager owner, string type)
        {
            this.owner = owner;
            cacheDir = Path.Combine(owner.cacheDir, type);
            Directory.CreateDirectory(cacheDir);
        }

        private class ImageTask
        {
            internal Task<ImageSource> task;
            internal CancellationTokenSource token = new CancellationTokenSource();

            public ImageTask(Requester owner, string uri, long id)
            {
                task = new Task<ImageSource>(() =>
                {
                    //Check memory cache
                    if(owner.cache.ContainsKey(id))
                    {
                        ImageSource cacheData;
                        owner.cache[id].TryGetTarget(out cacheData);
                        if(cacheData != null)
                        {
                            return cacheData;
                        }
                    }

                    //Check disk cache
                    var cachePath = Path.Combine(owner.cacheDir, id + ".bin");
                    if (File.Exists( cachePath ))
                    {
                        return ImageSource.FromFile(cachePath);
                    }

                    //Download it
                    var message = new HttpRequestMessage();
                    message.Method = HttpMethod.Get;
                    message.RequestUri = new Uri(uri);

                    var response = owner.owner.client.SendAsync(message, token.Token).GetAwaiter().GetResult();
                    var data = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

                    //Save cache in background
                    new Task(() =>
                    {
                        File.WriteAllBytes(cachePath, data);
                    }).Start();

                    var cache = ImageSource.FromStream(() => { return new MemoryStream(data); });
                    owner.cache[id] = new WeakReference<ImageSource>(cache);

                    ImageTask task;
                    owner.tasks.TryRemove(id, out task);

                    return cache;
                });
                task.Start();
            }
        }

        private ConcurrentDictionary<long, ImageTask> tasks = new ConcurrentDictionary<long, ImageTask>();
        private ConcurrentDictionary<long, WeakReference<ImageSource>> cache = new ConcurrentDictionary<long, WeakReference<ImageSource>>();

        internal Task<ImageSource> request(string uri, long id)
        {
            if(tasks.ContainsKey(id))
            {
                return tasks[id].task;
            }
            var task = new ImageTask(this, uri, id);
            tasks[id] = task;
            return task.task;
        }

        internal void release(long id)
        {
            //if operation in progress
            if(tasks.ContainsKey(id))
            {
                //Cancel
                tasks[id].token.Cancel();
            }
        }
    }
}
