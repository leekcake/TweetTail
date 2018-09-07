using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;

namespace TwitterInterface.API.Result
{
    struct FindListResult
    {
        List<Collection> collections;

        string nextCursor;
    }
}
