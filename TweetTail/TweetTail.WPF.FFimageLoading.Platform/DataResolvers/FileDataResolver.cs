using FFImageLoading.Work;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FFImageLoading.DataResolvers
{
    public class FileDataResolver : IDataResolver
    {
        private ResourceDataResolver resource = new ResourceDataResolver();

        public async virtual Task<DataResolverResult> Resolve(string identifier, TaskParameter parameters, CancellationToken token)
        {
            if (!File.Exists(identifier))
            {
                return await resource.Resolve(identifier, parameters, token);
            }

            var imageInformation = new ImageInformation();
            imageInformation.SetPath(identifier);
            imageInformation.SetFilePath(identifier);

            token.ThrowIfCancellationRequested();
            var stream = File.Open(identifier, FileMode.Open);

            return new DataResolverResult(stream, LoadingResult.Disk, imageInformation);
        }
    }
}