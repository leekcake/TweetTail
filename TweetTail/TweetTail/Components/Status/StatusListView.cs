﻿using Library.Container.Account;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Utils;
using TwitterInterface.Data;
using Xamarin.Forms;
using DataStatus = TwitterInterface.Data.Status;

namespace TweetTail.Components.Status
{
    public class StatusListView : TwitterListView<DataStatus, StatusCell>
    {

    }
}
