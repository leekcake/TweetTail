﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataUser = TwitterInterface.Data.User;
using DataStatus = TwitterInterface.Data.Status;
using TweetTail.Utils;

namespace TweetTail.Pages.Status
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StatusExpandPage : ContentPage
	{
        private DataStatus parent;

		public StatusExpandPage (DataStatus parent)
		{
            this.parent = parent;
			InitializeComponent();
            StatusListView.Items.Add(parent);

#pragma warning disable CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
            Fetch();
#pragma warning restore CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
        }

        public async Task Fetch()
        {
            try
            {
                var fetch = await App.Tail.TwitterAPI.GetConversationAsync(App.Tail.Account.GetAccountGroup(parent.Issuer[0]).AccountForRead, parent.ID);

                StatusListView.Items.Clear();
                DataStatus orig = null;
                foreach(var status in fetch)
                {
                    if(status.ID == parent.ID)
                    {
                        orig = status;
                    }
                    StatusListView.Items.Add(status);
                }
                if (orig != null) {
                    StatusListView.ScrollTo(orig, ScrollToPosition.MakeVisible, true);
                }
            }
            catch(Exception e)
            {
                Util.HandleException(e);
            }
        }
	}
}