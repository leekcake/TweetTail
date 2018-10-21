using FFImageLoading.Work;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

using FFImageLoading;

using Image = System.Windows.Controls.Image;
using FFImageLoading.Forms.Args;
using System.Threading.Tasks;

namespace FFImageLoading.Forms.Platform
{
    public class CachedImageRenderer : ViewRenderer<CachedImage, Image>
    {
        private IScheduledWork _currentTask;
        private ImageSourceBinding _lastImageSource;
        private bool _isSizeSet;

        [RenderWith(typeof(CachedImageRenderer))]
        internal class _CachedImageRenderer
        {
        }

        public static void Init()
        {
            CachedImage.IsRendererInitialized = true;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CachedImage> e)
        {
            if (Control == null && Element != null)
            {
                if (Control == null)
                {
                    var control = new Image();
                    SetNativeControl(control);
                }
            }
            
            if (e.OldElement != null)
            {
                e.OldElement.InternalReloadImage = null;
                e.OldElement.InternalCancel = null;
            }

            if (e.NewElement != null)
            {
                _isSizeSet = false;
                e.NewElement.InternalReloadImage = new Action(ReloadImage);
                e.NewElement.InternalCancel = new Action(CancelIfNeeded);

                UpdateAspect();
                UpdateImage(Control, e.NewElement, e.OldElement);
            }
            
            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CachedImage.SourceProperty.PropertyName)
            {
                UpdateImage(Control, Element, null);
            }
            else if (e.PropertyName == CachedImage.AspectProperty.PropertyName)
            {
                UpdateAspect();
            }
        }

        async void UpdateImage(Image imageView, CachedImage image, CachedImage previousImage)
        {
            CancelIfNeeded();

            if (image == null || imageView == null)
                return;

            var ffSource = await ImageSourceBinding.GetImageSourceBinding(image.Source, image).ConfigureAwait(false);
            if (ffSource == null)
            {
                if (_lastImageSource == null)
                    return;

                _lastImageSource = null;
                imageView.Source = null;
                return;
            }

            if (previousImage != null && !ffSource.Equals(_lastImageSource))
            {
                _lastImageSource = null;
                imageView.Source = null;
            }

            image.SetIsLoading(true);

            var placeholderSource = await ImageSourceBinding.GetImageSourceBinding(image.LoadingPlaceholder, image).ConfigureAwait(false);
            var errorPlaceholderSource = await ImageSourceBinding.GetImageSourceBinding(image.ErrorPlaceholder, image).ConfigureAwait(false);

            image.SetupOnBeforeImageLoading(out TaskParameter imageLoader, ffSource, placeholderSource, errorPlaceholderSource);

            if (imageLoader != null)
            {
                var finishAction = imageLoader.OnFinish;
                var sucessAction = imageLoader.OnSuccess;

                imageLoader.Finish((work) =>
                {
                    finishAction?.Invoke(work);
                    ImageLoadingSizeChanged(image, false);
                });

                imageLoader.Success((imageInformation, loadingResult) =>
                {
                    sucessAction?.Invoke(imageInformation, loadingResult);
                    _lastImageSource = ffSource;
                });

                imageLoader.LoadingPlaceholderSet(() => ImageLoadingSizeChanged(image, true));
                
                _currentTask = imageLoader.Into(imageView);
            }
        }

        void UpdateAspect()
        {
            if (Control == null || Element == null)
                return;
            Control.Stretch = GetStretch(Element.Aspect);
        }

        static Stretch GetStretch(Aspect aspect)
        {
            switch (aspect)
            {
                case Aspect.AspectFill:
                    return Stretch.UniformToFill;
                case Aspect.Fill:
                    return Stretch.Fill;
                default:
                    return Stretch.Uniform;
            }
        }

        async void ImageLoadingSizeChanged(CachedImage element, bool isLoading)
        {
            await ImageService.Instance.Config.MainThreadDispatcher.PostAsync(() =>
            {
                if (element != null)
                {
                    if (!isLoading || !_isSizeSet)
                    {
                        ((IVisualElementController)element)?.InvalidateMeasure(Xamarin.Forms.Internals.InvalidationTrigger.RendererReady);
                        _isSizeSet = true;
                    }

                    if (!isLoading)
                        element.SetIsLoading(isLoading);
                }
            });
        }

        void ReloadImage()
        {
            UpdateImage(Control, Element, null);
        }

        void CancelIfNeeded()
        {
            try
            {
                _currentTask?.Cancel();
            }
            catch (Exception) { }
        }

        protected override void Dispose(bool disposing)
        {
            CancelIfNeeded();

            base.Dispose(disposing);
        }
    }
}
