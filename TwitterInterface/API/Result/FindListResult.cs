using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;

namespace TwitterInterface.API.Result
{
    public struct FindListResult
    {
        public List<Collection> collections;

        public string nextCursor;
    }
}
