using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FFImageLoading.Helpers
{
    public static class ScaleHelper
    {
        static double? _scale;
        public static double Scale {
            get {
                //TODO: 
                return 1d;
                /*
                if (!_scale.HasValue)
                {
                    InitAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                }

                return _scale.Value;
                */
            }
        }

        public static async Task InitAsync()
        {
            if (_scale.HasValue)
                return;

            var dispatcher = ImageService.Instance.Config.MainThreadDispatcher;

            await dispatcher.PostAsync(() =>
            {
                _scale = 1d;
            }).ConfigureAwait(false);
        }
    }
}