using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterLibrary.Container
{
    public class FilterStore<T>
    {
        public delegate T Filter(T data);
        private static List<Filter> filters;
        public void RegisterFilter(Filter filter)
        {
            filters.Add(filter);
        }

        public void UnregisterFilter(Filter filter)
        {
            filters.Remove(filter);
        }

        public T ApplyFilter(T data)
        {
            var result = data;
            foreach(var filter in filters)
            {
                result = filter.Invoke(result);
                if(result == null) //이미 무효가 되었다면 더이상 필터를 적용할 수 없음
                {
                    break;
                }
            }
            return result;
        }
    }
}
