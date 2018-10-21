using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Xamarin.Forms.Platform.WPF;
using Xamarin.Forms.Platform.WPF.Interfaces;

namespace TweetTail.WPF.Hotfix
{
    public class FormsContentLoaderPublic : FormsContentLoader
    {
        public object LoadContentPublic(FrameworkElement parent, object page)
        {
            return LoadContent(parent, page);
        }
    }

    public class StackingContentLoader : IContentLoader
    {
        private FormsContentLoaderPublic Parent = new FormsContentLoaderPublic();

        //Doesn't destroy oldContent for Navigate Stacking
        public Task<object> LoadContentAsync(FrameworkElement parent, object oldContent, object newContent, CancellationToken cancellationToken)
        {
            if (!System.Windows.Application.Current.Dispatcher.CheckAccess())
                throw new InvalidOperationException("UIThreadRequired");

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return Task.Factory.StartNew(() => Parent.LoadContentPublic(parent, newContent), cancellationToken, TaskCreationOptions.None, scheduler);
        }

        public void OnSizeContentChanged(FrameworkElement parent, object content)
        {
            ((IContentLoader)Parent).OnSizeContentChanged(parent, content);
        }
    }
}
