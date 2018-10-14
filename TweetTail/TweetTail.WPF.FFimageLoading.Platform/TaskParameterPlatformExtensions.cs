/**
 * 
 from: https://github.com/luberda-molinet/FFImageLoading/blob/87c69858b4546d8734968ae40fac4e616d256514/source/FFImageLoading.Windows/Extensions/TaskParameterPlatformExtensions.cs
The MIT License (MIT)

Copyright (c) 2015 Daniel Luberda & Fabien Molinet

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 **/

using System.Threading.Tasks;
using FFImageLoading.Work;
using System.IO;
using System.Windows.Controls;
using FFImageLoading.Targets;
using System.Windows.Media.Imaging;
using FFImageLoading.Extensions;

namespace FFImageLoading
{
    /// <summary>
    /// TaskParameterPlatformExtensions
    /// </summary>
    public static class TaskParameterPlatformExtensions
    {
        /// <summary>
        /// Loads the image into PNG Stream
        /// </summary>
        /// <returns>The PNG Stream async.</returns>
        /// <param name="parameters">Parameters.</param>
        public static async Task<Stream> AsPNGStreamAsync(this TaskParameter parameters)
        {
            var result = await AsWriteableBitmapAsync(parameters);
            var stream = await result.AsPngStreamAsync();

            return stream;
        }

        /// <summary>
        /// Loads the image into JPG Stream
        /// </summary>
        /// <returns>The JPG Stream async.</returns>
        /// <param name="parameters">Parameters.</param>
        public static async Task<Stream> AsJPGStreamAsync(this TaskParameter parameters, int quality = 80)
        {
            var result = await AsWriteableBitmapAsync(parameters);
            var stream = await result.AsJpegStreamAsync(quality);

            return stream;
        }

        /// <summary>
        /// Loads the image into given Image using defined parameters.
        /// </summary>
        /// <param name="parameters">Parameters for loading the image.</param>
        /// <param name="imageView">Image view that should receive the image.</param>
        public static IScheduledWork Into(this TaskParameter parameters, Image imageView)
        {
            var target = new ImageTarget(imageView);

            if (parameters.Source != Work.ImageSource.Stream && string.IsNullOrWhiteSpace(parameters.Path))
            {
                target.SetAsEmpty(null);
                parameters.TryDispose();
                return null;
            }

            var task = ImageService.CreateTask(parameters, target);
            ImageService.Instance.LoadImage(task);
            return task;
        }

        /// <summary>
        /// Loads the image into given Image using defined parameters.
        /// IMPORTANT: It throws image loading exceptions - you should handle them
        /// </summary>
        /// <returns>An awaitable Task.</returns>
        /// <param name="parameters">Parameters for loading the image.</param>
        /// <param name="imageView">Image view that should receive the image.</param>
        public static Task<IScheduledWork> IntoAsync(this TaskParameter parameters, Image imageView)
        {
            var userErrorCallback = parameters.OnError;
            var finishCallback = parameters.OnFinish;
            var tcs = new TaskCompletionSource<IScheduledWork>();

            parameters
                .Error(ex =>
                {
                    tcs.TrySetException(ex);
                    userErrorCallback?.Invoke(ex);
                })
                .Finish(scheduledWork =>
                {
                    finishCallback?.Invoke(scheduledWork);
                    tcs.TrySetResult(scheduledWork);
                })
                .Into(imageView);

            return tcs.Task;
        }

        /// <summary>
        /// Loads and gets WriteableBitmap using defined parameters.
        /// IMPORTANT: It throws image loading exceptions - you should handle them
        /// </summary>
        /// <returns>The WriteableBitmap.</returns>
        /// <param name="parameters">Parameters.</param>
        public static Task<WriteableBitmap> AsWriteableBitmapAsync(this TaskParameter parameters)
        {
            var target = new BitmapTarget();
            var userErrorCallback = parameters.OnError;
            var finishCallback = parameters.OnFinish;
            var tcs = new TaskCompletionSource<WriteableBitmap>();

            parameters
                .Error(ex =>
                {
                    tcs.TrySetException(ex);
                    userErrorCallback?.Invoke(ex);
                })
                .Finish(scheduledWork =>
                {
                    finishCallback?.Invoke(scheduledWork);
                    tcs.TrySetResult(target.BitmapSource as WriteableBitmap);
                });

            if (parameters.Source != Work.ImageSource.Stream && string.IsNullOrWhiteSpace(parameters.Path))
            {
                target.SetAsEmpty(null);
                parameters.TryDispose();
                return null;
            }

            var task = ImageService.CreateTask(parameters, target);
            ImageService.Instance.LoadImage(task);

            return tcs.Task;
        }
    }
}
