using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data.Entity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Components.Status
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PollView : ContentView
	{
        private Polls polls;
        private int inx;

		public PollView ()
		{
			InitializeComponent ();
		}

        public void Update(Polls polls, int inx)
        {
            this.polls = polls;
            this.inx = inx;

            StatusLabel.Text = string.Format("{0}: {1}표", polls.Options[inx].Name, polls.Options[inx].Count);
        }
	}
}