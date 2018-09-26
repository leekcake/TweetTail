using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;

namespace Library.Container.Fetch
{
    public abstract class CursoredFetch<Data> : Fetchable<Data>
    {
        protected abstract long GetID(Data data);
        protected abstract Task<CursoredList<Data>> GetDatas(long cursor);

        private long nextCursor = -1;
        public bool IsSupportRefresh => false;

        public Task<List<Data>> FetchNew()
        {
            if(nextCursor != -1)
            {
                throw new InvalidOperationException("CursoredFetch doesn't support FetchNew");
            }
            return FetchOld();
        }

        public async Task<List<Data>> FetchOld()
        {
            var datas = await GetDatas(nextCursor);

            nextCursor = datas.nextCursor;

            return datas;
        }
    }
}
