using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    interface MediaAPI
    {
        //POST media/upload
        long uploadMedia(Account account, Stream image);
    }
}
