using System.Collections.Generic;
using JetBrains.Annotations;

namespace CanonEosPhotoDownloader.FileSystemIsolation
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
        /// <returns>True if directory was created; false if it already exists.</returns>
        bool CreateDirectoryIfNotExists(
            [NotNull] string path);

        /// <summary>
        ///     Checks if destination file exists and if not, copies source to destination.
        /// </summary>
        /// <returns>True if source file was copied to destination; false if it already exists.</returns>
        bool CopyFileIfDoesNotExist(
            [NotNull] string source,
            [NotNull] string destination);

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