using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;

namespace TwitterInterface.API.Result
{
    public struct GetEntriesResult
    {
        Collection collection;
        List<Status> tweet;
        //response - timeline auto merge

        int maxPosition;
        int minPosition;
    }
}
