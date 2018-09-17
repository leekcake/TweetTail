using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Library.Manager;
using TwitterInterface.Data;
using Xamarin.Forms;

namespace Library.Container.Media
{
    public class BannerRequester : Requester
    {
        public BannerRequester(MediaManager owner) : base(owner, "banner")
        {
        }

        public Task<ImageSource> request(User user)
        {
            return request(user.profileBannerURL, user.id);
        }

        public void release(User user)
        {
            release(user.id);
        }
    }
}
