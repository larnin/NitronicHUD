using System.IO;
using static NitronicHUD.Plugin;

namespace NitronicHUD
{
    public static class Util
    {
        public static FileInfo GetFile(string path) => new FileInfo(Resource.NormalizePath($@"{Files.RootDirectory}\Data\{path}"));
    }
}
