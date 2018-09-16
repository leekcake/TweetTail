using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TwitterInterface.Data;
using System.Threading.Tasks;

namespace TwitterInterface.API
{
    public interface MediaAPI
    {
        //POST media/upload
        Task<long> uploadMedia(Account account, string fileName, Stream image);
    }
}
