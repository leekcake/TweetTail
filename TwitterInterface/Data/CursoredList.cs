using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    class CursoredList<T> : List<T>
    {
        long previousCursor;
        long nextCursor;
    }
}
