using System;
using System.Threading.Tasks;
using Android.Content;
using TweetTail.Controls.FormsVideoLibrary;
using TweetTail.Droid;
using Xamarin.Forms;

// Need application's MainActivity

[assembly: Dependency(typeof(TweetTail.Droid.Controls.FormsVideoLibrary.VideoPicker))]

namespace TweetTail.Droid.Controls.FormsVideoLibrary
{
    public class VideoPicker : IVideoPicker
    {
        public Task<string> GetVideoFileAsync()
        {
            // Define the Intent for getting images
            Intent intent = new Intent();
            intent.SetType("video/*");
            intent.SetAction(Intent.ActionGetContent);

            // Get the MainActivity instance
            MainActivity activity = MainActivity.Current;

            // Start the picture-picker activity (resumes in MainActivity.cs)
            activity.StartActivityForResult(
                Intent.CreateChooser(intent, "Select Video"),
                MainActivity.PickImageId);

            // Save the TaskCompletionSource object as a MainActivity property
            activity.PickImageTaskCompletionSource = new TaskCompletionSource<string>();

            // Return Task object
            return activity.PickImageTaskCompletionSource.Task;
        }
    }
}