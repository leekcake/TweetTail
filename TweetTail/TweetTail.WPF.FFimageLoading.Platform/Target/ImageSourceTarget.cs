using System;
using System.Windows.Media.Imaging;
using FFImageLoading.Work;

using ImageSource = System.Windows.Media.ImageSource;

namespace FFImageLoading.Targets
{
    public class ImageSourceTarget : Target<ImageSource, WriteableBitmap>
    {
        private WeakReference<ImageSource> _imageWeakReference = null;

        public override void Set(IImageLoaderTask task, ImageSource image, bool animated)
        {
            if (task == null || task.IsCancelled)
                return;

            if (_imageWeakReference == null)
                _imageWeakReference = new WeakReference<ImageSource>(image);
            else
                _imageWeakReference.SetTarget(image);
        }

        public ImageSource ImageSource {
            get {
                if (_imageWeakReference == null)
                    return null;

                _imageWeakReference.TryGetTarget(out ImageSource image);
                return image;
            }
        }
    }
}