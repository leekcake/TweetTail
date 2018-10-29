using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TweetTail.Controls.FormsVideoLibrary;
using Vlc.DotNet.Wpf;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(TweetTail.Controls.FormsVideoLibrary.VideoPlayer),
                          typeof(TweetTail.UWP.Controls.FormsVideoLibrary.VideoPlayerRenderer))]

namespace TweetTail.UWP.Controls.FormsVideoLibrary
{
    public class VideoPlayerRenderer : ViewRenderer<VideoPlayer, VlcControl>
    {
        private Vlc.DotNet.Forms.VlcControl MediaPlayer => Control.MediaPlayer;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayer> args)
        {
            base.OnElementChanged(args);

            if (args.NewElement != null)
            {
                if (Control == null)
                {
                    VlcControl mediaElement = new VlcControl();
                    SetNativeControl(mediaElement);

                    MediaPlayer.VlcLibDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));                    
                    MediaPlayer.EndInit();
                }

                SetAreTransportControlsEnabled();
                SetSource();
                SetAutoPlay();

                args.NewElement.UpdateStatus += OnUpdateStatus;
                args.NewElement.PlayRequested += OnPlayRequested;
                args.NewElement.PauseRequested += OnPauseRequested;
                args.NewElement.StopRequested += OnStopRequested;
            }

            if (args.OldElement != null)
            {
                args.OldElement.UpdateStatus -= OnUpdateStatus;
                args.OldElement.PlayRequested -= OnPlayRequested;
                args.OldElement.PauseRequested -= OnPauseRequested;
                args.OldElement.StopRequested -= OnStopRequested;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        void OnMediaElementMediaOpened(object sender, RoutedEventArgs args)
        {
            ((IVideoPlayerController)Element).Duration = new TimeSpan(MediaPlayer.Length);
        }

        void OnMediaElementCurrentStateChanged(object sender, RoutedEventArgs args)
        {
            VideoStatus videoStatus = VideoStatus.NotReady;
            
            //TODO

            ((IVideoPlayerController)Element).Status = videoStatus;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(sender, args);

            if (args.PropertyName == VideoPlayer.AreTransportControlsEnabledProperty.PropertyName)
            {
                SetAreTransportControlsEnabled();
            }
            else if (args.PropertyName == VideoPlayer.SourceProperty.PropertyName)
            {
                SetSource();
            }
            else if (args.PropertyName == VideoPlayer.AutoPlayProperty.PropertyName)
            {
                SetAutoPlay();
            }
            else if (args.PropertyName == VideoPlayer.PositionProperty.PropertyName)
            {
                //TODO:
            }
        }

        void SetAreTransportControlsEnabled()
        {
            //TODO:
        }

        void SetSource()
        {
            bool hasSetSource = false;

            if (Element.Source is UriVideoSource)
            {
                string uri = (Element.Source as UriVideoSource).Uri;

                if (!String.IsNullOrWhiteSpace(uri))
                {
                    MediaPlayer.SetMedia(new Uri(uri));
                    hasSetSource = true;
                }
            }
            else if (Element.Source is FileVideoSource)
            {
                // Code requires Pictures Library in Package.appxmanifest Capabilities to be enabled
                string filename = (Element.Source as FileVideoSource).File;
                if (!String.IsNullOrWhiteSpace(filename))
                {
                    MediaPlayer.SetMedia(new Uri(filename));
                    hasSetSource = true;
                }
            }
            else if (Element.Source is ResourceVideoSource)
            {
                string path = "ms-appx:///" + (Element.Source as ResourceVideoSource).Path;

                if (!String.IsNullOrWhiteSpace(path))
                {
                    MediaPlayer.SetMedia(new Uri(path));
                    hasSetSource = true;
                }
            }

            if (!hasSetSource)
            {
                MediaPlayer.Stop();
            }
            else
            {
                MediaPlayer.Play();
            }
        }

        void SetAutoPlay()
        {
            //Control.AutoPlay = Element.AutoPlay;
        }

        // Event handler to update status
        void OnUpdateStatus(object sender, EventArgs args)
        {
            //TODO:
            //((IElementController)Element).SetValueFromRenderer(VideoPlayer.PositionProperty, Control.Position);
        }

        // Event handlers to implement methods
        void OnPlayRequested(object sender, EventArgs args)
        {
            MediaPlayer.Play();
        }

        void OnPauseRequested(object sender, EventArgs args)
        {
            MediaPlayer.Pause();
        }

        void OnStopRequested(object sender, EventArgs args)
        {
            MediaPlayer.Stop();
        }
    }
}