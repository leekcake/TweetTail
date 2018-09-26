using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;

namespace Library.Container.Fetch
{
    public abstract class SinceFetch<Data> : Fetchable<Data>
    {
        protected abstract long GetID(Data data);
        protected abstract Task<List<Data>> GetDatas(long sinceId, long maxId);

        private long sinceId = -1, maxId = -1;
        public bool IsSupportRefresh => true;
        
        public async Task<List<Data>> FetchNew()
        {
            var datas = await GetDatas(sinceId, -1);

            if (datas.Count == 0)
            {
                return datas;
            }

            sinceId = GetID(datas[0]);
            if (maxId == -1)
            {
                maxId = GetID(datas[datas.Count - 1]);
            }
            return datas;
        }

        public async Task<List<Data>> FetchOld()
        {
            var datas = await GetDatas(-1, maxId - 1);
            if(datas.Count == 0)
            {
                return datas;
            }
            maxId = GetID(datas[datas.Count - 1]);

            return datas;
        }
    }
}
