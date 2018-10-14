using FFImageLoading.Work;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Reflection;
using System.Collections;
using System.Linq;

namespace FFImageLoading.DataResolvers
{
    public class ResourceDataResolver : IDataResolver
    {
        public static string[] GetResourceNames()
        {
            var asm = Assembly.GetEntryAssembly();
            string resName = asm.GetName().Name + ".g.resources";
            using (var stream = asm.GetManifestResourceStream(resName))
            using (var reader = new System.Resources.ResourceReader(stream))
            {
                return reader.Cast<DictionaryEntry>().Select(entry => (string)entry.Key).ToArray();
            }
        }

        private static Dictionary<string, string> TrimmedResource = new Dictionary<string, string>();

        public ResourceDataResolver()
        {
            foreach(var name in GetResourceNames())
            {
                TrimmedResource[Path.GetFileNameWithoutExtension(name)] = name;
            }
        }

        public async virtual Task<DataResolverResult> Resolve(string identifier, TaskParameter parameters, CancellationToken token)
        {
            try
            {
                if( TrimmedResource.ContainsKey(identifier) )
                {
                    identifier = TrimmedResource[identifier];
                }

                var data = Application.GetResourceStream(new Uri(identifier, UriKind.Relative));
                var image = new ImageInformation();
                image.SetPath(identifier);
                image.SetFilePath(identifier);

                token.ThrowIfCancellationRequested();

                return new DataResolverResult(data.Stream, LoadingResult.CompiledResource, image);
            }
            catch(IOException io)
            {
                throw new FileNotFoundException(identifier, io);
            }
        }
    }
}