using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class CursoredList<T> : List<T>
    {
        long previousCursor;
        long nextCursor;
    }
}
