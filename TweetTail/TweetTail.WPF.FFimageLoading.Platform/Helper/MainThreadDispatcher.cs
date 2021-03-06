﻿using System;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

namespace FFImageLoading.Helpers
{
    public class MainThreadDispatcher : IMainThreadDispatcher
    {
        public async void Post(Action action)
        {
            if(Application.Current.Dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                await Application.Current.Dispatcher.BeginInvoke(action);
            }
        }

        public Task PostAsync(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();
            Post(() =>
            {
                try
                {
                    action?.Invoke();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }

        public Task PostAsync(Func<Task> action)
        {
            var tcs = new TaskCompletionSource<bool>();
            Post(async () =>
            {
                try
                {
                    await action?.Invoke();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }
    }
}