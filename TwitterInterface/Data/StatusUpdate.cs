using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class StatusUpdate
    {
        public string text;

        public long inReplyToStatusId;

        public bool autoPopulateReplyMetadata = false;
        public long[] excludeReplyUserIds;

        public string attachmentURL;

        public long[] mediaIDs;

        public bool possiblySensitive;
        
        //TODO: lat
        //TODO: long
        //TODO: place
    }
}
