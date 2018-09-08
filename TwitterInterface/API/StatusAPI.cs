using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    interface StatusAPI
    {
        //GET statuses/home_timeline
        List<Status> GetTimeline(Account account, long count = 200, long sinceId = -1, long maxId = -1);

        //GET statuses/user_timeline
        List<Status> GetUserline(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1);

        //GET statuses/mentions_timeline
        List<Status> GetMentionline(Account account, long count = 200, long sinceId = -1, long maxId = -1);
    }
}
