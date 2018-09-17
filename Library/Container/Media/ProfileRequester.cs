using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Library.Manager;
using TwitterInterface.Data;
using Xamarin.Forms;

namespace Library.Container.Media
{
    public class ProfileRequester : Requester
    {
        public ProfileRequester(MediaManager owner) : base(owner, "profile")
        {
        }

        public Task<ImageSource> request(User user)
        {
            return request(user.profileHttpsImageURL, user.id);
        }

        public void release(User user)
        {
            release(user.id);
        }
    }
}
