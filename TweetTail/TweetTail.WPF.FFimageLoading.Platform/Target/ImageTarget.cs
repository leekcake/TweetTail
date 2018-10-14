/**
 * 
 from: https://github.com/luberda-molinet/FFImageLoading/blob/87c69858b4546d8734968ae40fac4e616d256514/source/FFImageLoading.Windows/Targets/ImageTarget.cs
The MIT License (MIT)

Copyright (c) 2015 Daniel Luberda & Fabien Molinet

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 **/

using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using FFImageLoading.Work;

namespace FFImageLoading.Targets
{
    public class ImageTarget : Target<BitmapSource, Image>
    {
        private readonly WeakReference<Image> _controlWeakReference;

        public ImageTarget(Image control)
        {
            _controlWeakReference = new WeakReference<Image>(control);
        }

        public override bool IsValid {
            get {
                return Control != null;
            }
        }

        public override void SetAsEmpty(IImageLoaderTask task)
        {
            var control = Control;
            if (control == null)
                return;

            control.Source = null;
        }

        public override void Set(IImageLoaderTask task, BitmapSource image, bool animated)
        {
            if (task.IsCancelled)
                return;

            var control = Control;
            if (control == null || control.Source == image)
                return;

            var parameters = task.Parameters;
            
            if (animated)
            {
                // fade animation
                int fadeDuration = parameters.FadeAnimationDuration.HasValue ?
                    parameters.FadeAnimationDuration.Value : ImageService.Instance.Config.FadeAnimationDuration;
                DoubleAnimation fade = new DoubleAnimation();
                fade.Duration = TimeSpan.FromMilliseconds(fadeDuration);
                fade.From = 0f;
                fade.To = 1f;
                fade.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };

                Storyboard fadeInStoryboard = new Storyboard();
                Storyboard.SetTargetProperty(fade, new System.Windows.PropertyPath("Opacity"));
                Storyboard.SetTarget(fade, control);
                fadeInStoryboard.Children.Add(fade);
                fadeInStoryboard.Begin();
                control.Source = image;
                if (IsLayoutNeeded(task))
                    control.UpdateLayout();
            }
            else
            {
                control.Source = image;
                if (IsLayoutNeeded(task))
                    control.UpdateLayout();
            }
        }

        bool IsLayoutNeeded(IImageLoaderTask task)
        {
            if (task.Parameters.InvalidateLayoutEnabled.HasValue)
            {
                if (!task.Parameters.InvalidateLayoutEnabled.Value)
                    return false;
            }
            else if (!task.Configuration.InvalidateLayout)
            {
                return false;
            }

            return true;
        }

        public override Image Control {
            get {
                Image control;
                if (!_controlWeakReference.TryGetTarget(out control))
                    return null;

                if (control == null)
                    return null;

                return control;
            }
        }
    }
}