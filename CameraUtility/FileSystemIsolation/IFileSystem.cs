using System.Collections.Generic;
using JetBrains.Annotations;

namespace CameraUtility.FileSystemIsolation
{
    /// <summary>
    ///     Isolates file-system operations (static Directory, File and Path classes).
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        ///     Finds file names (including their paths) that match the specified search pattern in
        ///     <paramref name="directory"/> and its sub-directories.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="searchMask">
        ///     The search string to match against the names of files in path. This parameter can contain a combination
        ///     of valid literal path and wildcard (* and ?) characters, but it does not support regular expressions.
        /// </param>
        /// <returns></returns>
        [NotNull]
        [ItemNotNull]
        IEnumerable<string> GetFiles(
            [NotNull] string directory,
            [NotNull] string searchMask = "*");

        /// <summary>
        ///     Checks if directory exists and if not, creates it.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pretend">
        ///     If true, files will not be copied, but new names will be detected and printed out.
        /// </param>
        /// <returns>True if directory was created; false if it already exists.</returns>
        bool CreateDirectoryIfNotExists(
            [NotNull] string path,
            bool pretend);

        /// <summary>
        ///     Checks if destination file exists and if not, copies source to destination.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="pretend">
        ///     If true, files will not be copied, but new names will be detected and printed out.
        /// </param>
        /// <returns>True if source file was copied to destination; false if it already exists.</returns>
        bool CopyFileIfDoesNotExist(
            [NotNull] string source,
            [NotNull] string destination,
            bool pretend);

        /// <summary>
        ///     Concatenates paths with system-dependent path separator ('/' on Unix/Linux and '\' on Windows).
        /// </summary>
        [NotNull]
        string CombinePaths(
            [NotNull] [ItemNotNull] params string[] paths);

        /// <summary>
        ///     Checks if file or directory exists.
        /// </summary>
        bool Exists(
            [NotNull] string path);
    }
}