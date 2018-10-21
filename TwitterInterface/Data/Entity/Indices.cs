using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data.Entity
{
    public class Indices
    {
        public int Start, End;

        public int Length {
            get {
                return End - Start;
            }
        }
    }
}
