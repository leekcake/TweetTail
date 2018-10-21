using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TwitterInterface.Data;
using System.Threading.Tasks;

namespace TwitterInterface.API
{
    public interface IMediaAPI
    {
        //POST media/upload
        Task<long> UploadMediaAsync(Account account, string fileName, Stream image);
    }
}
