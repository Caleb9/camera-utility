using System.Collections.Generic;
using JetBrains.Annotations;

namespace CanonEosPhotoDownloader.FileSystemIsolation
{
    public interface IFileSystem
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<string> GetFiles(
            [NotNull] string directory,
            [NotNull] string searchMask);

        bool CreateDirectoryIfNotExists(
            [NotNull] string path);

        bool FileCopyIfDoesNotExist(
            [NotNull] string source,
            [NotNull] string destination);

        [NotNull] string PathCombine(
            [NotNull] [ItemNotNull] params string[] paths);

        bool Exists(
            [NotNull] string path);
    }
}