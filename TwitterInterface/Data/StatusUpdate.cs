using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class StatusUpdate
    {
        public string Text;

        public long InReplyToStatusId;

        public bool AutoPopulateReplyMetadata = false;
        public long[] ExcludeReplyUserIds;

        public string AttachmentURL;

        public long[] MediaIDs;

        public bool PossiblySensitive;
        
        //TODO: lat
        //TODO: long
        //TODO: place
    }
}
