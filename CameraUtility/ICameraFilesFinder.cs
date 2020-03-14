using System.Collections.Generic;

namespace CameraUtility
{
    public interface ICameraFilesFinder
    {
        /// <summary>
        ///     Finds all the files of supported types located in <paramref name="path"/> and its sub-directories. If
        ///     <paramref name="path"/> is pointing to a file, returns that file if it is of the supported type,
        ///     otherwise throws an exception.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IEnumerable<string> FindCameraFiles(string path);
    }
}