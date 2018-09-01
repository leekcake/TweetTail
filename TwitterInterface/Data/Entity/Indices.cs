using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data.Entity
{
    public class Indices
    {
        public int start, end;

        public int Length {
            get {
                return end - start;
            }
        }
    }
}
