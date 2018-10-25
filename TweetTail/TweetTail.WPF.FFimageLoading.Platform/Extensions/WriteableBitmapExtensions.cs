using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FFImageLoading.Extensions
{
    public static class WriteableBitmapExtensions
    {
        public static async Task<Stream> AsPngStreamAsync(this WriteableBitmap bitmap)
        {
            // Encode pixels into stream
            var result = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add( BitmapFrame.Create(bitmap) );
            encoder.Save(result);

            return new MemoryStream(result.ToArray());
        }
        
        public static async Task<Stream> AsJpegStreamAsync(this WriteableBitmap bitmap, int quality = 90)
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