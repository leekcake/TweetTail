using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using FFImageLoading.Cache;
using FFImageLoading.Config;
using FFImageLoading.DataResolvers;
using FFImageLoading.Helpers;
using FFImageLoading.Work;

using ImageSource = System.Windows.Media.ImageSource;

namespace FFImageLoading
{
    [Preserve(AllMembers = true)]
    public class ImageService : ImageServiceBase<ImageSource>
    {
        static ConditionalWeakTable<object, IImageLoaderTask> _viewsReferences = new ConditionalWeakTable<object, IImageLoaderTask>();
        static IImageService _instance;

        /// <summary>
        /// FFImageLoading instance.
        /// </summary>
        /// <value>The instance.</value>
        public static IImageService Instance {
            get {
                if (_instance == null)
                    _instance = new ImageService();

                return _instance;
            }
        }

        protected override void PlatformSpecificConfiguration(Config.Configuration configuration)
        {
            base.PlatformSpecificConfiguration(configuration);

            configuration.ClearMemoryCacheOnOutOfMemory = false;
            configuration.ExecuteCallbacksOnUIThread = true;
        }

        protected override IMemoryCache<ImageSource> MemoryCache => ImageCache.Instance;
        protected override IMD5Helper CreatePlatformMD5HelperInstance(Configuration configuration) => new MD5Helper();
        protected override IMiniLogger CreatePlatformLoggerInstance(Configuration configuration) => new MiniLogger();
        protected override IPlatformPerformance CreatePlatformPerformanceInstance(Configuration configuration) => new PlatformPerformance();
        protected override IMainThreadDispatcher CreateMainThreadDispatcherInstance(Configuration configuration) => new MainThreadDispatcher();
        protected override IDataResolverFactory CreateDataResolverFactoryInstance(Configuration configuration) => new DataResolverFactory();

        protected override IDiskCache CreatePlatformDiskCacheInstance(Configuration configuration)
        {
            string rootFolder = null;
            string folderName = null;

            if (string.IsNullOrWhiteSpace(configuration.DiskCachePath))
            {
                rootFolder = Path.GetTempPath();
                folderName = "FFSimpleDiskCache";
                string cachePath = Path.Combine(rootFolder, folderName);
                configuration.DiskCachePath = cachePath;
            }
            else
            {
                var separated = configuration.DiskCachePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                folderName = separated.Last();
                var rootPath = configuration.DiskCachePath.Substring(0, configuration.DiskCachePath.LastIndexOf(folderName));
                rootFolder = rootPath;
            }

            return new SimpleDiskCache(rootFolder, folderName, Config);
        }

        internal static IImageLoaderTask CreateTask<TImageView>(TaskParameter parameters, ITarget<ImageSource, TImageView> target) where TImageView : class
        {
            return new PlatformImageLoaderTask<TImageView>(target, parameters, Instance);
        }

        internal static IImageLoaderTask CreateTask(TaskParameter parameters)
        {
            return new PlatformImageLoaderTask<object>(null, parameters, Instance);
        }

        protected override void SetTaskForTarget(IImageLoaderTask currentTask)
        {
            var targetView = currentTask?.Target?.TargetControl;

            if (!(targetView is Image))
                return;

            lock (_viewsReferences)
            {
                if (_viewsReferences.TryGetValue(targetView, out var existingTask))
                {
                    try
                    {
                        if (existingTask != null && !existingTask.IsCancelled && !existingTask.IsCompleted)
                        {
                            existingTask.Cancel();
                        }
                    }
                    catch (ObjectDisposedException) { }

                    _viewsReferences.Remove(targetView);
                }

                _viewsReferences.Add(targetView, currentTask);
            }
        }

        public override void CancelWorkForView(object view)
        {
            lock (_viewsReferences)
            {
                if (_viewsReferences.TryGetValue(view, out var existingTask))
                {
                    try
                    {
                        if (existingTask != null && !existingTask.IsCancelled && !existingTask.IsCompleted)
                        {
                            existingTask.Cancel();
                        }
                    }
                    catch (ObjectDisposedException) { }
                }
            }
        }

        public override int DpToPixels(double dp)
        {
            return (int)Math.Floor(dp * ScaleHelper.Scale);
        }

        public override double PixelsToDp(double px)
        {
            if (Math.Abs(px) < double.Epsilon)
                return 0d;

            return Math.Floor(px / ScaleHelper.Scale);
        }
    }
}
