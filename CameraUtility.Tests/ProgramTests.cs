using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using CameraUtility.Exif;
using CameraUtility.FileSystemIsolation;
using Moq;
using NUnit.Framework;

namespace CameraUtility.Tests
{
    /// <summary>
    ///     Integration test checking that entire application is working as expected. External resources are faked
    ///     (IFileSystem and IMetadataReader). This test considers entire app as a unit or work. It is more complex
    ///     but should be more resilient to refactorings.
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    [TestOf(typeof(Program))]
    [ExcludeFromCodeCoverage]
    public sealed class ProgramTests
    {
        private IFixture NewFixture()
        {
            return new Fixture().Customize(new AutoMoqCustomization());
        }

        private Mock<IFileSystem> SetupDefaultFakeFileSystem(Mock<IFileSystem> fakeFileSystem)
        {
            /* By default GetFiles returns empty collection. Specific tests extend this setup individually. */
            fakeFileSystem
                .Setup(fs => fs.GetFiles(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Enumerable.Empty<string>());

            /* Lock otherwise system-dependent behavior of Path.Combine, so tests don't have to consider if they're
             * executed in Windows or Linux. */
            fakeFileSystem
                .Setup(fs => fs.CombinePaths(It.IsAny<string[]>()))
                .Returns<string[]>(paths => string.Join('/', paths));

            return fakeFileSystem;
        }

        private ITag NewCreatedDateTimeOriginalTag(string value)
        {
            return Mock.Of<ITag>(t =>
                t.Directory == "Exif SubIFD" && t.Type == 0x9003 && t.Value == value);
        }

        private ITag NewSubSecondsTag(string value)
        {
            return Mock.Of<ITag>(t =>
                t.Directory == "Exif SubIFD" && t.Type == 0x9291 && t.Value == value);
        }

        private IEnumerable<ITag> NewImageFileTags(string createdDateOriginal, string subSeconds)
        {
            return new[]
            {
                NewCreatedDateTimeOriginalTag(createdDateOriginal),
                NewSubSecondsTag(subSeconds)
            };
        }

        private ITag NewQuickTimeCreatedTag(string value)
        {
            return Mock.Of<ITag>(t =>
                t.Directory == "QuickTime Movie Header" && t.Type == 0x3 && t.Value == value);
        }

        private void AssertDirectoryCreated(Mock<IFileSystem> fileSystemMock, string directory)
        {
            fileSystemMock.Verify(fs => fs.CreateDirectoryIfNotExists(directory, false), Times.AtLeastOnce);
        }

        private void AssertFileCopied(Mock<IFileSystem> fileSystemMock, string sourceFile, string destinationFile)
        {
            fileSystemMock.Verify(fs => fs.CopyFileIfDoesNotExist(sourceFile, destinationFile, false), Times.Once);
        }
        
        private void AssertFileMoved(Mock<IFileSystem> fileSystemMock, string sourceFile, string destinationFile)
        {
            fileSystemMock.Verify(fs => fs.MoveFileIfDoesNotExist(sourceFile, destinationFile, false), Times.Once);
        }

        private Mock<IFileSystem> SetupFilesInFileSystem(IFixture fixture, string sourceDir)
        {
            var fileSystemMock = fixture.Freeze<Mock<IFileSystem>>();
            fileSystemMock
                .Setup(fs => fs.Exists($"{sourceDir}"))
                .Returns(true);
            fileSystemMock
                .Setup(fs => fs.IsDirectory(It.IsAny<string>()))
                .Returns(true);
            SetupDefaultFakeFileSystem(fileSystemMock)
                .Setup(fs => fs.GetFiles(sourceDir, "*"))
                .Returns(new[] {
                    $"{sourceDir}/IMG_1234.JPG",
                    $"{sourceDir}/IMG_4231.JPG",
                    $"{sourceDir}/IMG_1234.CR2",
                    $"{sourceDir}/MVI_1234.MP4" });
            fixture
                .Freeze<Mock<IMetadataReader>>()
                .SetupSequence(mr => mr.ExtractTags(It.IsAny<string>()))
                .Returns(NewImageFileTags("2010:01:12 13:14:15", "42"))
                .Returns(NewImageFileTags("2011:02:13 14:15:16", "43"))
                .Returns(NewImageFileTags("2012:03:14 15:16:17", "44"))
                .Returns(new[] { NewQuickTimeCreatedTag("Fri Jun 13 14.15.16 1980") });
            return fileSystemMock;
        }
        
        /// <summary>
        ///     Basic sanity check for copy-mode.
        /// </summary>
        [Test]
        [TestOf(nameof(Program.Execute))]
        public void Execute__FourImageFiles_CopyMode__FilesGetCopied()
        {
            /* Arrange */
            var fixture = NewFixture();
            fixture.Inject(
                new Program.Options(
                    "sourceDir",
                    "destDir",
                    false,
                    false,
                    false));
            var fileSystemMock = SetupFilesInFileSystem(fixture, "sourceDir");
            var sut = fixture.Create<Program>();

            /* Act */
            sut.Execute();

            /* Assert */
            AssertDirectoryCreated(fileSystemMock, "destDir/2010_01_12");
            AssertFileCopied(
                fileSystemMock, "sourceDir/IMG_1234.JPG", "destDir/2010_01_12/IMG_20100112_131415420.JPG");
            AssertDirectoryCreated(fileSystemMock, "destDir/2011_02_13");
            AssertFileCopied(
                fileSystemMock, "sourceDir/IMG_4231.JPG", "destDir/2011_02_13/IMG_20110213_141516430.JPG");
            AssertDirectoryCreated(fileSystemMock, "destDir/2012_03_14");
            AssertFileCopied(
                fileSystemMock, "sourceDir/IMG_1234.CR2", "destDir/2012_03_14/IMG_20120314_151617440.CR2");
            AssertDirectoryCreated(fileSystemMock, "destDir/1980_06_13");
            AssertFileCopied(
                fileSystemMock, "sourceDir/MVI_1234.MP4", "destDir/1980_06_13/VID_19800613_141516000.MP4");
        }
        
        /// <summary>
        ///     Basic sanity check for move-mode.
        /// </summary>
        [Test]
        [TestOf(nameof(Program.Execute))]
        public void Execute__FourImageFiles_MoveMode__FilesGetMoved()
        {
            /* Arrange */
            var fixture = NewFixture();
            fixture.Inject(
                new Program.Options(
                    "sourceDir",
                    "destDir",
                    false,
                    false,
                    true));
            var fileSystemMock = SetupFilesInFileSystem(fixture, "sourceDir");
            var sut = fixture.Create<Program>();

            /* Act */
            sut.Execute();

            /* Assert */
            AssertDirectoryCreated(fileSystemMock, "destDir/2010_01_12");
            AssertFileMoved(
                fileSystemMock, "sourceDir/IMG_1234.JPG", "destDir/2010_01_12/IMG_20100112_131415420.JPG");
            AssertDirectoryCreated(fileSystemMock, "destDir/2011_02_13");
            AssertFileMoved(
                fileSystemMock, "sourceDir/IMG_4231.JPG", "destDir/2011_02_13/IMG_20110213_141516430.JPG");
            AssertDirectoryCreated(fileSystemMock, "destDir/2012_03_14");
            AssertFileMoved(
                fileSystemMock, "sourceDir/IMG_1234.CR2", "destDir/2012_03_14/IMG_20120314_151617440.CR2");
            AssertDirectoryCreated(fileSystemMock, "destDir/1980_06_13");
            AssertFileMoved(
                fileSystemMock, "sourceDir/MVI_1234.MP4", "destDir/1980_06_13/VID_19800613_141516000.MP4");
        }
        
        /// <summary>
        ///     Basic sanity check for dry-run-mode.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        [TestOf(nameof(Program.Execute))]
        public void Execute__FourImageFiles_PretendMode__FilesGetMoved(bool moveMode)
        {
            /* Arrange */
            var fixture = NewFixture();
            fixture.Inject(
                new Program.Options(
                    "sourceDir",
                    "destDir",
                    true,
                    false,
                    moveMode));
            var fileSystemMock = SetupFilesInFileSystem(fixture, "sourceDir");
            var sut = fixture.Create<Program>();

            /* Act */
            sut.Execute();

            /* Assert */
            fileSystemMock.Verify(
                fs => fs.CreateDirectoryIfNotExists(It.IsAny<string>(), false),
                Times.Never);
            fileSystemMock.Verify(
                fs => fs.CopyFileIfDoesNotExist(It.IsAny<string>(), It.IsAny<string>(), false),
                Times.Never);
            fileSystemMock.Verify(
                fs => fs.MoveFileIfDoesNotExist(It.IsAny<string>(), It.IsAny<string>(), false),
                Times.Never);
        }
    }
}