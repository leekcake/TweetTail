using FFImageLoading.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ImageSource = System.Windows.Media.ImageSource;

namespace FFImageLoading.Cache
{
    public class WriteableBitmapLRUCache : LRUCache<string, Tuple<ImageSource, ImageInformation>>
    {
        public WriteableBitmapLRUCache(int capacity) : base(capacity)
        {
        }

        public override int GetValueSize(Tuple<ImageSource, ImageInformation> value)
        {
            if (value?.Item2 == null)
                return 0;

            return value.Item2.CurrentHeight * value.Item2.CurrentWidth * 4;
        }
    }
}