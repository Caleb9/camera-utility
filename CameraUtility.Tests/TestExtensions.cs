using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using CameraUtility.Exif;
using Moq;

namespace CameraUtility.Tests
{
    internal static class TestExtensions
    {
        internal static Mock<IFileSystem> SetupDefaults(
            this Mock<IFileSystem> fileSystemMock)
        {
            fileSystemMock
                .Setup(fs => fs.Path.Combine(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((s1, s2) => $"{s1}/{s2}");
            return fileSystemMock;
        }

        internal static Mock<IFileSystem> SetupSourceDirectoryWithFiles(
            this Mock<IFileSystem> fileSystemMock,
            string sourceDirectoryPath,
            IEnumerable<string> files)
        {
            fileSystemMock
                .Setup(fs => fs.Directory.Exists(sourceDirectoryPath))
                .Returns(true);

            fileSystemMock
                .Setup(fs =>
                    fs.Directory.GetFiles(
                        sourceDirectoryPath,
                        It.Is<string>(searchPattern => searchPattern == "*" || searchPattern == string.Empty),
                        SearchOption.AllDirectories))
                .Returns(files.ToArray());

            return fileSystemMock;
        }

        internal static Mock<IMetadataReader> SetupMetadata(
            this Mock<IMetadataReader> metadataReaderMock,
            IEnumerable<(string file, IEnumerable<ITag> tags)> filesWithExifMetadata)
        {
            foreach (var (file, tags) in filesWithExifMetadata)
            {
                metadataReaderMock
                    .Setup(mr => mr.ExtractTags(new CameraFilePath(file)))
                    .Returns(tags);
            }

            return metadataReaderMock;
        }
    }
}