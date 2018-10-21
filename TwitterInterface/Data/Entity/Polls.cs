using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data.Entity
{
    public class Polls
    {
        public class Option
        {
            public int Position;
            public string Name;
            public int Count;
        }

        public DateTime EndDateTime;
        public int DurationMinutes;
        public Option[] Options;
        public string URL; //If it parsed in card, card url

        public int TotalCount;
    }
}
