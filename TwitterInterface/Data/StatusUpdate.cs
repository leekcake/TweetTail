using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    class StatusUpdate
    {
        string text;

        long inReplyToStatusId;

        bool autoPopulateReplyMetadata = false;
        long[] excludeReplyUserIds;

        string attachmentURL;

        long[] mediaIDs;

        bool possiblySensitive;
        
        //TODO: lat
        //TODO: long
        //TODO: place
    }
}
