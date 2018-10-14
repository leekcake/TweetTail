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

namespace FFImageLoading.Extensions
{
    public static class ImageExtensions
    {
        public static async Task<WriteableBitmap> ToBitmapImageAsync(this BitmapHolder holder)
        {
            if (holder == null || holder.PixelData == null)
                return null;

            WriteableBitmap writeableBitmap = null;

            await ImageService.Instance.Config.MainThreadDispatcher.PostAsync(async () =>
            {
                writeableBitmap = await holder.ToWriteableBitmap();
                //writeableBitmap.Invalidate();
            });

            return writeableBitmap;
        }

        private static async Task<WriteableBitmap> ToWriteableBitmap(this BitmapHolder holder)
        {
            WriteableBitmap bitmap = null;
            await ImageService.Instance.Config.MainThreadDispatcher.PostAsync(() =>
            {
                bitmap = new WriteableBitmap(holder.Width, holder.Height, 96, 96, PixelFormats.Rgba128Float, null);
                var all = new Int32Rect(0, 0, holder.Width, holder.Height);
                bitmap.WritePixels(all, holder.PixelData, 4, 0);
            });

            return bitmap;
        }

        public async static Task<WriteableBitmap> ToBitmapImageAsync(this Stream imageStream, Tuple<int, int> downscale, bool downscaleDipUnits, InterpolationMode mode, bool allowUpscale, ImageInformation imageInformation = null)
        {
            if (imageStream == null)
                return null;
            
            //TODO: Downscale
            WriteableBitmap bitmap = null;
            await ImageService.Instance.Config.MainThreadDispatcher.PostAsync(() =>
            {
                var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                bitmap = new WriteableBitmap(decoder.Frames[0]);
            });
            return bitmap;
        }

        public async static Task<BitmapHolder> ToBitmapHolderAsync(this Stream imageStream, Tuple<int, int> downscale, bool downscaleDipUnits, InterpolationMode mode, bool allowUpscale, ImageInformation imageInformation = null)
        {
            WriteableBitmap bitmap = null;
            await ImageService.Instance.Config.MainThreadDispatcher.PostAsync(() =>
            {
                var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                bitmap = new WriteableBitmap(decoder.Frames[0]);
            });
            var holder = new BitmapHolder(bitmap);
            return holder;
        }
    }
}