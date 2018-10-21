using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Container;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    public interface IAPI : IAccountAPI, ICollectionAPI, IMediaAPI, IStatusAPI, ITwitterListAPI, IUserAPI
    {
        FilterStore<Status> StatusFilter {
            get;
        }

        FilterStore<User> UserFilter {
            get;
        }
    }
}
