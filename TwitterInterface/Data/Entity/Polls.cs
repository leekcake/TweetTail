using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data.Entity
{
    public class Polls
    {
        public class Option
        {
            int position;
            string name;
        }

        public DateTime endDateTime;
        public int durationMinutes;
    }
}
