﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitterInterface.Data;

namespace TwitterInterface.Container
{
    /// <summary>
    /// Helper class for login to twitter
    /// </summary>
    public interface ILoginToken
    {
        string LoginURL {
            get;
        }

        Task<Account> LoginAsync(string pin);
    }
}
