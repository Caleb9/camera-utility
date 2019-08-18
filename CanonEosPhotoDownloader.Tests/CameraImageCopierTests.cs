using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.AutoMoq;
using CanonEosPhotoDownloader.FileSystemIsolation;
using Moq;
using NUnit.Framework;

namespace CanonEosPhotoDownloader.Tests
{
    [TestFixture]
    [TestOf(typeof(CameraImageCopier))]
    [ExcludeFromCodeCoverage]
    public sealed class CameraImageCopierTests
    {
        private IFixture NewFixture()
        {
            return new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});
        }

        [Test]
        [TestOf(nameof(CameraImageCopier.CopyFiles))]
        public void CopyFiles_DryRun_DoesNotModifyFileSystem()
        {
            var fixture = NewFixture();
            var fileSystemMock = fixture.Freeze<Mock<IFileSystem>>();
            var sut = fixture.Create<CameraImageCopier>();

            sut.CopyFiles("sourceDir", "destDir", true);

            fileSystemMock.Verify(
                fs => fs.CreateDirectoryIfNotExists(It.IsAny<string>()), Times.Never);
            fileSystemMock.Verify(
                fs => fs.FileCopyIfDoesNotExist(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}