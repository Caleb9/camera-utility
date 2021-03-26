using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using CameraUtility.Exif;
using FluentAssertions;
using Moq;
using Xunit;
using static CameraUtility.Tests.Helpers;

namespace CameraUtility.Tests
{
    public sealed class CheckFilesCommandTests
    {
        /// <summary>
        ///     Basic sanity check
        /// </summary>
        [Fact]
        public void Check_directory_when_all_files_are_camera_files_with_metadata()
        {
            /* Arrange */
            var fixture = NewFixture();
            const string sourceDirPath = "/source/dir";
            (string sourceFile, IEnumerable<ITag> exifTags)[] sourceFiles =
            {
                (
                    $"{sourceDirPath}/IMG_1234.JPG",
                    NewImageFileTags("2010:01:12 13:14:15", "42")),
                (
                    $"{sourceDirPath}/IMG_2345.JPEG",
                    NewImageFileTags("2011:02:13 14:15:16", "43")),
                (
                    $"{sourceDirPath}/IMG_3456.CR2",
                    NewImageFileTags("2012:03:14 15:16:17", "44")),
                (
                    $"{sourceDirPath}/IMG_4567.DNG",
                    NewImageFileTags("2012:03:14 15:16:17", "44")),
                (
                    $"{sourceDirPath}/MVI_5678.MP4",
                    NewQuickTimeFileTags("Fri Jun 13 14.15.16 1980")),
                (
                    $"{sourceDirPath}/MVI_6789.MOV",
                    NewQuickTimeFileTags("Fri Jun 13 15.16.17 1980"))
            };
            SetupFileSystemStub(fixture, sourceDirPath, sourceFiles.Select(f => f.sourceFile));
            fixture.Freeze<Mock<IMetadataReader>>().SetupMetadata(sourceFiles);
            var consoleTextWriterMock = SetupConsoleTextWriterMock(fixture);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute($"check {sourceDirPath}".Split());

            /* Assert */
            result.Should().Be(0);
            var output = consoleTextWriterMock.ToString();
            output.Should().Be("Found 6 camera file(s). All files contain metadata.\n");
        }
        
        [Fact]
        public void Check_directory_when_all_files_are_camera_files_but_some_are_missing_metadata()
        {
            /* Arrange */
            var fixture = NewFixture();
            const string sourceDirPath = "/source/dir";
            (string sourceFile, IEnumerable<ITag> exifTags)[] sourceFiles =
            {
                (
                    $"{sourceDirPath}/IMG_1234.JPG",
                    NewImageFileTags("2010:01:12 13:14:15", "42")),
                (
                    $"{sourceDirPath}/IMG_2345.JPEG",
                    Array.Empty<ITag>())
            };
            SetupFileSystemStub(fixture, sourceDirPath, sourceFiles.Select(f => f.sourceFile));
            fixture.Freeze<Mock<IMetadataReader>>().SetupMetadata(sourceFiles);
            var consoleTextWriterMock = SetupConsoleTextWriterMock(fixture);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute($"check {sourceDirPath}".Split());

            /* Assert */
            result.Should().Be(0);
            var output = consoleTextWriterMock.ToString();
            output.Should().Be(
                "Found 2 camera file(s). Missing metadata in 1 file(s).\n" +
                "Following files are missing metadata:\n" +
                $"{sourceDirPath}/IMG_2345.JPEG\n");
        }

        [Fact]
        public void Check_directory_when_not_all_files_are_camera_files()
        {
            /* Arrange */
            var fixture = NewFixture();
            const string sourceDirPath = "/source/dir";
            (string sourceFile, IEnumerable<ITag> exifTags)[] sourceFiles =
            {
                (
                    $"{sourceDirPath}/IMG_1234.JPG",
                    NewImageFileTags("2010:01:12 13:14:15", "42")),
                (
                    $"{sourceDirPath}/not-a-camera.file",
                    Array.Empty<ITag>())
            };
            SetupFileSystemStub(fixture, sourceDirPath, sourceFiles.Select(f => f.sourceFile));
            fixture.Freeze<Mock<IMetadataReader>>().SetupMetadata(sourceFiles);
            var consoleTextWriterMock = SetupConsoleTextWriterMock(fixture);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute($"check {sourceDirPath}".Split());

            /* Assert */
            result.Should().Be(0);
            var output = consoleTextWriterMock.ToString();
            output.Should().Be(
                "Found 1 camera file(s). All files contain metadata.\n");
        }
        
        [Fact]
        public void Check_directory_when_none_of_the_files_are_camera_files()
        {
            /* Arrange */
            var fixture = NewFixture();
            const string sourceDirPath = "/source/dir";
            var sourceFiles = new []
            {
                    $"{sourceDirPath}/not-a-camera.file1",
                    $"{sourceDirPath}/not-a-camera.file2",
            };
            SetupFileSystemStub(fixture, sourceDirPath, sourceFiles);
            var consoleTextWriterMock = SetupConsoleTextWriterMock(fixture);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute($"check {sourceDirPath}".Split());

            /* Assert */
            result.Should().Be(0);
            var output = consoleTextWriterMock.ToString();
            output.Should().Be(
                "Found 0 camera file(s).\n");
        }
        
        [Fact]
        public void Check_multiple_directories_with_subdirectories()
        {
            /* Arrange */
            var fixture = NewFixture();
            const string sourceDirPath1 = "/source/dir1";
            (string sourceFile, IEnumerable<ITag> exifTags)[] sourceFiles1 =
            {
                (
                    $"{sourceDirPath1}/IMG_1234.JPG",
                    NewImageFileTags("2010:01:12 13:14:15", "42"))
            };
            const string sourceDirPath2 = "/source/dir2";
            (string sourceFile, IEnumerable<ITag> exifTags)[] sourceFiles2 =
            {
                (
                    $"{sourceDirPath2}/subdirectory/IMG_2345.JPG",
                    NewImageFileTags("2011:02:13 14:15:16", "43"))
            };
            fixture
                .Freeze<Mock<IFileSystem>>()
                .SetupDefaults()
                .SetupSourceDirectoryWithFiles(sourceDirPath1, sourceFiles1.Select(f => f.sourceFile))
                .SetupSourceDirectoryWithFiles(sourceDirPath2, sourceFiles2.Select(f => f.sourceFile));

            fixture
                .Freeze<Mock<IMetadataReader>>()
                .SetupMetadata(sourceFiles1)
                .SetupMetadata(sourceFiles2);
            var consoleTextWriterMock = SetupConsoleTextWriterMock(fixture);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute($"check {sourceDirPath1} {sourceDirPath2}".Split());

            /* Assert */
            result.Should().Be(0);
            var output = consoleTextWriterMock.ToString();
            output.Should().Be(
                "Found 2 camera file(s). All files contain metadata.\n");
        }
        
        [Fact]
        public void Check_single_file()
        {
            /* Arrange */
            var fixture = NewFixture();
            const string sourceDirPath = "/source/dir";
            (string sourceFile, IEnumerable<ITag> exifTags)[] sourceFiles =
            {
                (
                    $"{sourceDirPath}/IMG_1234.JPG",
                    NewImageFileTags("2010:01:12 13:14:15", "42"))
            };
            fixture
                .Freeze<Mock<IFileSystem>>()
                .SetupDefaults()
                .Setup(fs => fs.File.Exists($"{sourceDirPath}/IMG_1234.JPG"))
                .Returns(true);
            fixture.Freeze<Mock<IMetadataReader>>().SetupMetadata(sourceFiles);
            var consoleTextWriterMock = SetupConsoleTextWriterMock(fixture);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute($"check {sourceDirPath}/IMG_1234.JPG".Split());

            /* Assert */
            result.Should().Be(0);
            var output = consoleTextWriterMock.ToString();
            output.Should().Be(
                "Found 1 camera file(s). All files contain metadata.\n");
        }

        private IFixture NewFixture()
        {
            return new Fixture().Customize(new AutoMoqCustomization());
        }

        private static void SetupFileSystemStub(
            IFixture fixture,
            string sourceDirPath,
            IEnumerable<string> sourceFiles)
        {
            fixture
                .Freeze<Mock<IFileSystem>>()
                .SetupDefaults()
                .SetupSourceDirectoryWithFiles(sourceDirPath, sourceFiles);
        }

        private static TextWriter SetupConsoleTextWriterMock(
            IFixture fixture)
        {
            var consoleTextWriterMock = new StringWriter();
            fixture.Inject<TextWriter>(consoleTextWriterMock);
            return consoleTextWriterMock;
        }
    }
}