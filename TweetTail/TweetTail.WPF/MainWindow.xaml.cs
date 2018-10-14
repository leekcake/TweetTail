﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

namespace TweetTail.WPF
{
    

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : FormsApplicationPage
    {
        public MainWindow()
        {
            WebBrowserHelper.FixBrowserVersion();
            InitializeComponent();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            Forms.Init();
            LoadApplication(new TweetTail.App());
        }
    }
}
