using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.AutoMoq;
using CameraUtility.FileSystemIsolation;
using Moq;
using NUnit.Framework;

namespace CameraUtility.Tests
{
    [TestFixture]
    [TestOf(typeof(CameraFileCopier))]
    [ExcludeFromCodeCoverage]
    public sealed class CameraFileCopierTests
    {
        private IFixture NewFixture()
        {
            return new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});
        }

        [Test]
        [TestOf(nameof(ICameraFileCopier.CopyCameraFiles))]
        public void CopyCameraFiles_DryRun_DoesNotModifyFileSystem()
        {
            var fixture = NewFixture();
            var fileSystemMock = fixture.Freeze<Mock<IFileSystem>>();
            ICameraFileCopier sut = fixture.Create<CameraFileCopier>();

            sut.CopyCameraFiles("sourceDir", "destDir", true);

            fileSystemMock.Verify(
                fs => fs.CreateDirectoryIfNotExists(It.IsAny<string>()), Times.Never);
            fileSystemMock.Verify(
                fs => fs.CopyFileIfDoesNotExist(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}