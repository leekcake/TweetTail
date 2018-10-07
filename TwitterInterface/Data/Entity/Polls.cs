using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data.Entity
{
    public class Polls
    {
        public class Option
        {
            public int position;
            public string name;
            public int count;
        }

        public DateTime endDateTime;
        public int durationMinutes;
        public Option[] options;
        public string url; //If it parsed in card, card url

        public int totalCount;
    }
}
