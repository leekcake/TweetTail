using System;
using FFImageLoading.Work;
using FFImageLoading.Helpers;
using System.Linq;
using System.Windows.Media.Imaging;

using ImageSource = System.Windows.Media.ImageSource;

namespace FFImageLoading.Cache
{
    class ImageCache : IImageCache
    {
        private static IImageCache _instance;
        private readonly WriteableBitmapLRUCache _reusableBitmaps;
        private readonly IMiniLogger _logger;

        private ImageCache(int maxCacheSize, IMiniLogger logger)
        {
            _logger = logger;

            if (maxCacheSize == 0)
            {
                maxCacheSize = 1000000 * 256; //256MB
                _logger?.Debug($"Memory cache size: {maxCacheSize} bytes");
            }

            _reusableBitmaps = new WriteableBitmapLRUCache(maxCacheSize);
        }

        public static IImageCache Instance {
            get {
                return _instance ?? (_instance = new ImageCache(ImageService.Instance.Config.MaxMemoryCacheSize, ImageService.Instance.Config.Logger));
            }
        }

        public void Add(string key, ImageInformation imageInformation, ImageSource bitmap)
        {
            if (string.IsNullOrWhiteSpace(key) || bitmap == null)
                return;

            _reusableBitmaps.TryAdd(key, new Tuple<ImageSource, ImageInformation>(bitmap, imageInformation));
        }

        public ImageInformation GetInfo(string key)
        {
            if (_reusableBitmaps.TryGetValue(key, out Tuple<ImageSource, ImageInformation> cacheEntry))
            {
                return cacheEntry.Item2;
            }

            return null;
        }

        public Tuple<ImageSource, ImageInformation> Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;


            if (_reusableBitmaps.TryGetValue(key, out Tuple<ImageSource, ImageInformation> cacheEntry) && cacheEntry.Item1 != null)
            {
                return new Tuple<ImageSource, ImageInformation>(cacheEntry.Item1, cacheEntry.Item2);
            }

            return null;
        }

        public void Clear()
        {
            _reusableBitmaps.Clear();

            // Force immediate Garbage collection. Please note that is resource intensive.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            _logger.Debug(string.Format("Called remove from memory cache for '{0}'", key));
            _reusableBitmaps.Remove(key);
        }

        public void RemoveSimilar(string baseKey)
        {
            if (string.IsNullOrWhiteSpace(baseKey))
                return;

            var pattern = baseKey + ";";

            var keysToRemove = _reusableBitmaps.Keys.Where(i => i.StartsWith(pattern, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var key in keysToRemove)
            {
                Remove(key);
            }
        }
    }
}