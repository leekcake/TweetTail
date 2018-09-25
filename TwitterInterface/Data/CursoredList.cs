using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class CursoredList<T> : List<T>
    {
        public CursoredList(long previousCursor, long nextCursor)
        {
            this.previousCursor = previousCursor;
            this.nextCursor = nextCursor;
        }

        public long previousCursor;
        public long nextCursor;
    }
}
