using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class CursoredList<T> : List<T>
    {
        public CursoredList(long previousCursor, long nextCursor)
        {
            this.PreviousCursor = previousCursor;
            this.NextCursor = nextCursor;
        }

        public long PreviousCursor;
        public long NextCursor;
    }
}
