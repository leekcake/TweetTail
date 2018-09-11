using System;
using System.Collections.Generic;
using TwitterInterface.Data;

namespace TwitterInterface.Container
{
    /// <summary>
    /// Helper class for login to twitter
    /// </summary>
    public interface LoginToken
    {
        string loginURL {
            get;
        }

        Account login(string pin);
    }
}
