using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using CameraUtility.FileSystemIsolation;
using Moq;
using NUnit.Framework;

namespace CameraUtility.Tests
{
    [TestFixture]
    [TestOf(typeof(CameraFilesFinder))]
    [ExcludeFromCodeCoverage]
    public class CameraFilesFinderTests
    {
        private IFixture CreateFixture()
        {
            return new Fixture().Customize(new AutoMoqCustomization());
        }
        
        [Test]
        [TestOf(nameof(ICameraFilesFinder.FindCameraFiles))]
        public void PathDoesNotExist_Throws()
        {
            var fixture = CreateFixture();
            var fileSystemStub = fixture.Freeze<Mock<IFileSystem>>();
            fileSystemStub
                .Setup(fs => fs.Exists(It.IsAny<string>()))
                .Returns(false);
            ICameraFilesFinder sut = fixture.Create<CameraFilesFinder>();

            void TestDelegate() => sut.FindCameraFiles("non existing path");

            Assert.Throws<CameraFilesFinder.PathNotFoundException>(TestDelegate);
        }
        
        [Test]
        [TestOf(nameof(ICameraFilesFinder.FindCameraFiles))]
        public void PathIsDirectory_ContainsImagesAndOtherFiles_ReturnsImageFilePathsOnly()
        {
            var fixture = CreateFixture();
            var fileSystemStub = fixture.Freeze<Mock<IFileSystem>>();
            fileSystemStub
                .Setup(fs => fs.Exists(It.IsAny<string>()))
                .Returns(true);
            fileSystemStub
                .Setup(fs => fs.IsDirectory(It.IsAny<string>()))
                .Returns(true);
            fileSystemStub
                .Setup(fs => fs.GetFiles(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new[] {
                    "image.cr2",
                    "notImage.txt",
                    "image.dng",
                    "notImage.pdf",
                    "image.jpg",
                    "notImage.asd",
                    "image.mp4"
                });
            ICameraFilesFinder sut = fixture.Create<CameraFilesFinder>();

            var result = sut.FindCameraFiles("directory");
            
            CollectionAssert.AreEquivalent(
                new [] {"image.cr2", "image.dng", "image.jpg", "image.mp4"},
                result);
        }
        
        [Test]
        [TestOf(nameof(ICameraFilesFinder.FindCameraFiles))]
        public void PathIsDirectory_DoesNotContainImages_ReturnsEmpty()
        {
            var fixture = CreateFixture();
            var fileSystemStub = fixture.Freeze<Mock<IFileSystem>>();
            fileSystemStub
                .Setup(fs => fs.Exists(It.IsAny<string>()))
                .Returns(true);
            fileSystemStub
                .Setup(fs => fs.IsDirectory(It.IsAny<string>()))
                .Returns(true);
            fileSystemStub
                .Setup(fs => fs.GetFiles(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new[] {
                    "notImage.txt",
                    "notImage.pdf",
                    "notImage.asd"
                });
            ICameraFilesFinder sut = fixture.Create<CameraFilesFinder>();

            var result = sut.FindCameraFiles("directory");
            
            CollectionAssert.IsEmpty(result);
        }
        
        [Test]
        [TestOf(nameof(ICameraFilesFinder.FindCameraFiles))]
        public void PathIsDirectory_IsEmpty_ReturnsEmpty()
        {
            var fixture = CreateFixture();
            var fileSystemStub = fixture.Freeze<Mock<IFileSystem>>();
            fileSystemStub
                .Setup(fs => fs.Exists(It.IsAny<string>()))
                .Returns(true);
            fileSystemStub
                .Setup(fs => fs.IsDirectory(It.IsAny<string>()))
                .Returns(true);
            fileSystemStub
                .Setup(fs => fs.GetFiles(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Enumerable.Empty<string>());
            ICameraFilesFinder sut = fixture.Create<CameraFilesFinder>();

            var result = sut.FindCameraFiles("directory");
            
            CollectionAssert.IsEmpty(result);
        }
        
        [TestCase("cr2")]
        [TestCase("jpg")]
        [TestCase("dng")]
        [TestCase("mp4")]
        [TestOf(nameof(ICameraFilesFinder.FindCameraFiles))]
        public void PathIsFile_IsImage_ReturnsPath(
            string fileExtension)
        {
            var fixture = CreateFixture();
            var fileSystemStub = fixture.Freeze<Mock<IFileSystem>>();
            fileSystemStub
                .Setup(fs => fs.Exists(It.IsAny<string>()))
                .Returns(true);
            fileSystemStub
                .Setup(fs => fs.IsDirectory(It.IsAny<string>()))
                .Returns(false);
            ICameraFilesFinder sut = fixture.Create<CameraFilesFinder>();

            var result = sut.FindCameraFiles($"file.{fileExtension}");
            
            Assert.AreEqual(
                new [] { $"file.{fileExtension}" },
                result);
        }
    }
}