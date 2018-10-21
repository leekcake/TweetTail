using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FFImageLoading.Extensions
{
    public static class WriteableBitmapExtensions
    {
#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        public static async Task<Stream> AsPngStreamAsync(this WriteableBitmap bitmap)
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        {
            // Encode pixels into stream
            var result = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add( BitmapFrame.Create(bitmap) );
            encoder.Save(result);

            return new MemoryStream(result.ToArray());
        }

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        public static async Task<Stream> AsJpegStreamAsync(this WriteableBitmap bitmap, int quality = 90)
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        {
            // Encode pixels into stream
            var result = new MemoryStream();
            var encoder = new JpegBitmapEncoder
            {
                QualityLevel = quality
            };
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(result);

            return new MemoryStream(result.ToArray());
        }
    }
}