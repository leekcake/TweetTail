﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Pages.Multi.Tails;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Multi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MutliTailPage : ContentPage
    {
        public MutliTailPage()
        {
            InitializeComponent();

            foreach (var group in App.Tail.Account.ReadOnlyAccountGroups)
            {
                AddTail( new TimelineTail(group) );
            }
        }

        public void AddTail(Tail tail)
        {
            SideMenu.Children.Add(new TailSideMenuView(tail));
            TailContainer.Children.Add(tail);
        }
    }
}