using System.Collections.Generic;

namespace CanonEosPhotoDownloader
{
    public interface ICameraFileFinder
    {
        IEnumerable<string> FindFilePaths(string directory);
    }
}