﻿using Library.Container.Fetch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataStatus = TwitterInterface.Data.Status;

namespace TweetTail.Pages.Status
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MentionPage : ContentPage
	{
		public MentionPage ()
		{
			InitializeComponent ();

            Reload();
        }

        public void ScrollToRoot()
        {
            StatusListView.ScrollToRoot();
        }

        public void Reload()
        {
            if (App.Tail.Blend.SelectedBlendedAccount != null)
            {
                StatusListView.Fetchable = new BlendAccountFetch<DataStatus>.Mentionline(App.Tail.Blend.SelectedBlendedAccount);
            }
            else
            {
                StatusListView.Fetchable = new AccountFetch.Mentionline(App.Tail, App.Tail.Account.SelectedAccountGroup);
            }
        }
    }
}