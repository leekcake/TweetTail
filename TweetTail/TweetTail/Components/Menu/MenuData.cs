using System;
using System.Collections.Generic;
using System.Text;

namespace TweetTail.Components.Menu
{
    public class MenuData
    {
        public string Title {
            get; set;
        }

        public string Description {
            get; set;
        }

        public string Icon {
            get; set;
        }

        public Action action;
    }
}
