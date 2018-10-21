using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.API
{
    public interface IAPI : IAccountAPI, ICollectionAPI, IMediaAPI, IStatusAPI, ITwitterListAPI, IUserAPI
    {
    }
}
