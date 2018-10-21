using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;

namespace TweetTail.WPF.Controls.CrossMedia
{
    public class MediaImplementation : IMedia
    {
        public bool IsCameraAvailable => false;

        public bool IsTakePhotoSupported => false;

        public bool IsPickPhotoSupported => true;

        public bool IsTakeVideoSupported => false;

        public bool IsPickVideoSupported => true;

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        public async Task<bool> Initialize() => true;
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.

        private Task<OpenFileDialog> PickFile(string title)
        {
            return Task.Run(() => {
                var openFileDialog = new OpenFileDialog
                {
                    Title = title
                };
                if (openFileDialog.ShowDialog() == false)
                {
                    return null;
                }
                return openFileDialog;
                });
        }

        public async Task<MediaFile> PickPhotoAsync(PickMediaOptions options = null)
        {
            OpenFileDialog openFileDialog = await PickFile("사진 선택");
            if (openFileDialog != null)
            {
                return new MediaFile(openFileDialog.FileName, () => { return new FileStream(openFileDialog.FileName, FileMode.Open); });
            }
            else
            {
                return null;
            }
        }

        public async Task<MediaFile> PickVideoAsync()
        {
            OpenFileDialog openFileDialog = await PickFile("비디오 선택");
            if (openFileDialog != null)
            {
                return new MediaFile(openFileDialog.FileName, () => { return new FileStream(openFileDialog.FileName, FileMode.Open); });
            }
            else
            {
                return null;
            }
        }

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        public async Task<MediaFile> TakePhotoAsync(StoreCameraMediaOptions options)
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        {
            return null;
        }

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        public async Task<MediaFile> TakeVideoAsync(StoreVideoOptions options)
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
        {
            return null;
        }
    }
}
