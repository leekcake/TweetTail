using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.API
{
    public interface API : AccountAPI, CollectionAPI, MediaAPI, StatusAPI, TwitterListAPI, UserAPI
    {
    }
}
