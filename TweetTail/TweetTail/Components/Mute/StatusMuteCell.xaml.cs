﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataMute = TwitterInterface.Data.Mute;

namespace TweetTail.Components.Mute
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StatusMuteCell : ViewCell
	{
		public StatusMuteCell ()
		{
			InitializeComponent ();
		}

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext == null) return;

            var mute = BindingContext as DataMute;
            var target = mute.Target as DataMute.StatusTarget;

            StatusView.BindingContext = target.Status;
            StatusView.Update();
        }
    }
}