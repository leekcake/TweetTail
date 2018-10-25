using FFImageLoading.Helpers;
using FFImageLoading.Work;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageSource = System.Windows.Media.ImageSource;

namespace FFImageLoading.Extensions
{
    public static class ImageExtensions
    {
        public static async Task<ImageSource> ToBitmapImageAsync(this BitmapHolder holder)
        {
            if (holder == null || holder.PixelData == null)
                return null;

            return await holder.ToWriteableBitmap();
        }

        private static Task<WriteableBitmap> ToWriteableBitmap(this BitmapHolder holder)
        {
            var task = new Task<WriteableBitmap>(() =>
            {
                WriteableBitmap bitmap = new WriteableBitmap(holder.Width, holder.Height, 96, 96, PixelFormats.Rgba128Float, null);
                var all = new Int32Rect(0, 0, holder.Width, holder.Height);
                bitmap.WritePixels(all, holder.PixelData, 4, 0);
                bitmap.Freeze();
                return bitmap;
            });
            task.Start();
            return task;
        }

        public static Task<ImageSource> ToBitmapImageAsync(this Stream imageStream, Tuple<int, int> downscale, bool downscaleDipUnits, InterpolationMode mode, bool allowUpscale, ImageInformation imageInformation = null)
        {
            if (imageStream == null)
                return Task.FromResult<ImageSource>(null);

            var task = new Task<ImageSource>(() =>
            {
                var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                var frame = decoder.Frames[0];
                frame.Freeze();

                if (imageInformation != null)
                {
                    imageInformation.SetCurrentSize(frame.PixelWidth, frame.PixelHeight);
                    imageInformation.SetOriginalSize(frame.PixelWidth, frame.PixelHeight);
                }

                return frame;
            });
            task.Start();

            return task;
        }

        public async static Task<BitmapHolder> ToBitmapHolderAsync(this Stream imageStream, Tuple<int, int> downscale, bool downscaleDipUnits, InterpolationMode mode, bool allowUpscale, ImageInformation imageInformation = null)
        {
            throw new NotImplementedException();
        }
    }
}