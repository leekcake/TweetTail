using System;
using System.Threading.Tasks;

namespace TweetTail.Controls.FormsVideoLibrary
{
    public interface IVideoPicker
    {
        Task<string> GetVideoFileAsync();
    }
}
