using System.Collections.Generic;

namespace CanonEosPhotoDownloader
{
    public interface ICameraFileFinder
    {
        /// <summary>
        ///    Finds all the files of supported types located in <paramref name="directory"/> and its sub-directories.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        IEnumerable<string> FindCameraFiles(string directory);
    }
}