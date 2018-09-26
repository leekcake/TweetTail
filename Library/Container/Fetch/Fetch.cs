using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;

namespace Library.Container.Fetch
{
    public interface Fetchable<Data>
    {
        bool IsSupportRefresh {
            get;
        }

        Task<List<Data>> FetchOld();
        Task<List<Data>> FetchNew();
    }
}
