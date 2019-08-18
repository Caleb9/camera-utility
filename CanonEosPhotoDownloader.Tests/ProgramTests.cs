using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using CanonEosPhotoDownloader.Exif;
using CanonEosPhotoDownloader.FileSystemIsolation;
using Moq;
using NUnit.Framework;

namespace CanonEosPhotoDownloader.Tests
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
                .Setup(fs => fs.PathCombine(It.IsAny<string[]>()))
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
            fileSystemMock.Verify(fs => fs.CreateDirectoryIfNotExists(directory), Times.AtLeastOnce);
        }

        private void AssertFileCopied(Mock<IFileSystem> fileSystemMock, string sourceFile, string destinationFile)
        {
            fileSystemMock.Verify(fs => fs.FileCopyIfDoesNotExist(sourceFile, destinationFile), Times.Once);
        }

        [Test]
        [TestOf(nameof(Program.Execute))]
        public void FourImageFiles_FilesGetCopied()
        {
            /* Arrange */
            var fixture = NewFixture();
            var fileSystemMock = fixture.Freeze<Mock<IFileSystem>>();
            SetupDefaultFakeFileSystem(fileSystemMock)
                .SetupSequence(fs => fs.GetFiles("sourceDir", It.Is<string>(mask => mask.StartsWith("*."))))
                .Returns(new[] {"sourceDir/IMG_1234.JPG", "sourceDir/IMG_4231.JPG"})
                .Returns(new[] {"sourceDir/IMG_1234.CR2"})
                .Returns(new[] {"sourceDir/MVI_1234.MP4"});
            fixture
                .Freeze<Mock<IMetadataReader>>()
                .SetupSequence(mr => mr.ExtractTags(It.IsAny<string>()))
                .Returns(NewImageFileTags("2010:01:12 13:14:15", "42"))
                .Returns(NewImageFileTags("2011:02:13 14:15:16", "43"))
                .Returns(NewImageFileTags("2012:03:14 15:16:17", "44"))
                .Returns(new[] {NewQuickTimeCreatedTag("Fri Jun 13 14.15.16 1980")});
            var sut = fixture.Create<Program>();

            /* Act */
            sut.Execute("-s", "sourceDir", "-d", "destDir");

            /* Assert */
            AssertDirectoryCreated(fileSystemMock, "destDir/2010_01_12");
            AssertFileCopied(
                fileSystemMock, "sourceDir/IMG_1234.JPG", "destDir/2010_01_12/IMG_20100112_131415420.jpg");
            AssertDirectoryCreated(fileSystemMock, "destDir/2011_02_13");
            AssertFileCopied(
                fileSystemMock, "sourceDir/IMG_4231.JPG", "destDir/2011_02_13/IMG_20110213_141516430.jpg");
            AssertDirectoryCreated(fileSystemMock, "destDir/2012_03_14");
            AssertFileCopied(
                fileSystemMock, "sourceDir/IMG_1234.CR2", "destDir/2012_03_14/IMG_20120314_151617440.cr2");
            AssertDirectoryCreated(fileSystemMock, "destDir/1980_06_13");
            AssertFileCopied(
                fileSystemMock, "sourceDir/MVI_1234.MP4", "destDir/1980_06_13/VID_19800613_141516000.mp4");
        }
    }
}