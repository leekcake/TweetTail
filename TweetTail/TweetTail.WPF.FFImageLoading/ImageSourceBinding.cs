using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFImageLoading.Work;
using Xamarin.Forms;

using ImageSource = FFImageLoading.Work.ImageSource;

namespace FFImageLoading.Forms.Platform
{
    public class ImageSourceBinding : IImageSourceBinding
    {
        public ImageSource ImageSource { get; private set; }
        public Func<CancellationToken, Task<Stream>> Stream { get; private set; }
        public string Path { get; private set; }

        public ImageSourceBinding(ImageSource imageSource, string path)
        {
            ImageSource = imageSource;
            Path = path;
        }

        public ImageSourceBinding(Func<CancellationToken, Task<Stream>> stream)
        {
            ImageSource = ImageSource.Stream;
            Stream = stream;
        }

        internal static async Task<ImageSourceBinding> GetImageSourceBinding(Xamarin.Forms.ImageSource source, CachedImage element)
        {
            if (source == null)
            {
                return null;
            }

            if (source is UriImageSource uriImageSource)
            {
                var uri = uriImageSource.Uri?.OriginalString;
                if (string.IsNullOrWhiteSpace(uri))
                    return null;

                return new ImageSourceBinding(ImageSource.Url, uri);
            }

            if (source is FileImageSource fileImageSource)
            {
                if (string.IsNullOrWhiteSpace(fileImageSource.File))
                    return null;

                return new ImageSourceBinding(Work.ImageSource.Filepath, fileImageSource.File);
            }

            if (source is StreamImageSource streamImageSource)
            {
                return new ImageSourceBinding(streamImageSource.Stream);
            }

            if (source is EmbeddedResourceImageSource embeddedResoureSource)
            {
                var uri = embeddedResoureSource.Uri?.OriginalString;
                if (string.IsNullOrWhiteSpace(uri))
                    return null;

                return new ImageSourceBinding(FFImageLoading.Work.ImageSource.EmbeddedResource, uri);
            }

            if (source is DataUrlImageSource dataUrlSource)
            {
                if (string.IsNullOrWhiteSpace(dataUrlSource.DataUrl))
                    return null;

                return new ImageSourceBinding(FFImageLoading.Work.ImageSource.Url, dataUrlSource.DataUrl);
            }

            if (source is IVectorImageSource vectorSource)
            {
                if (vectorSource.VectorHeight == 0 && vectorSource.VectorHeight == 0)
                {
                    if (element.Height > 0d && !double.IsInfinity(element.Height))
                    {
                        vectorSource.UseDipUnits = true;
                        vectorSource.VectorHeight = (int)element.Height;
                    }
                    else if (element.Width > 0d && !double.IsInfinity(element.Width))
                    {
                        vectorSource.UseDipUnits = true;
                        vectorSource.VectorWidth = (int)element.Width;
                    }
                    else if (element.HeightRequest > 0d && !double.IsInfinity(element.HeightRequest))
                    {
                        vectorSource.UseDipUnits = true;
                        vectorSource.VectorHeight = (int)element.HeightRequest;
                    }
                    else if (element.WidthRequest > 0d && !double.IsInfinity(element.WidthRequest))
                    {
                        vectorSource.UseDipUnits = true;
                        vectorSource.VectorWidth = (int)element.WidthRequest;
                    }
                    else if (element.MinimumHeightRequest > 0d && !double.IsInfinity(element.MinimumHeightRequest))
                    {
                        vectorSource.UseDipUnits = true;
                        vectorSource.VectorHeight = (int)element.MinimumHeightRequest;
                    }
                    else if (element.MinimumWidthRequest > 0d && !double.IsInfinity(element.MinimumWidthRequest))
                    {
                        vectorSource.UseDipUnits = true;
                        vectorSource.VectorWidth = (int)element.MinimumWidthRequest;
                    }
                }

                return await GetImageSourceBinding(vectorSource.ImageSource, element).ConfigureAwait(false);
            }

            throw new NotImplementedException("ImageSource type not supported");
        }
    }
}
