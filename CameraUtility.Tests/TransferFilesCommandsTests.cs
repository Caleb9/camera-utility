using System.IO.Abstractions;
using AutoFixture;
using AutoFixture.AutoMoq;
using CameraUtility.Exif;
using FluentAssertions;
using Moq;
using Xunit;
using static CameraUtility.Tests.Helpers;

namespace CameraUtility.Tests;

public static class TransferFilesCommandsTests
{
    private const string SourceDirPath = "/source/directory";
    private const string DestinationDirPath = "/destination/directory";

    public sealed class WithDateSubdirectory
    {
        private static readonly IEnumerable<(
                string sourceFile,
                IEnumerable<ITag> exifTags,
                string expectedDestinationDirectory,
                string expectedDestinationFile)>
            FilesWithMetadata = new[]
            {
                (
                    $"{SourceDirPath}/IMG_1234.JPG",
                    NewImageFileTags("2010:01:12 13:14:15", "42"),
                    $"{DestinationDirPath}/2010_01_12",
                    "IMG_20100112_131415420.JPG"),
                (
                    $"{SourceDirPath}/IMG_4231.JPEG",
                    NewImageFileTags("2011:02:13 14:15:16", "43"),
                    $"{DestinationDirPath}/2011_02_13",
                    "IMG_20110213_141516430.JPEG"),
                (
                    $"{SourceDirPath}/IMG_1234.CR2",
                    NewImageFileTags("2012:03:14 15:16:17", "44"),
                    $"{DestinationDirPath}/2012_03_14",
                    "IMG_20120314_151617440.CR2"),
                (
                    $"{SourceDirPath}/MVI_1234.MP4",
                    NewQuickTimeFileTags("Fri Jun 13 14.15.16 1980"),
                    $"{DestinationDirPath}/1980_06_13",
                    "VID_19800613_141516000.MP4"),
                (
                    $"{SourceDirPath}/MVI_2345.MOV",
                    NewQuickTimeFileTags("Fri Jun 13 15.16.17 1980"),
                    $"{DestinationDirPath}/1980_06_13",
                    "VID_19800613_151617000.MOV"),
                ( /* Checks if files in sub-directories are also copied and if extension is case-insensitive */
                    $"{SourceDirPath}/sub-dir/IMG_1234.jpg",
                    NewImageFileTags("2013:04:15 16:17:18", "45"),
                    $"{DestinationDirPath}/2013_04_15",
                    "IMG_20130415_161718450.jpg")
            };

        /// <summary>
        ///     Basic sanity check
        /// </summary>
        [Fact]
        public void Copy_directory_copies_all_supported_files()
        {
            /* Arrange */
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var fileSystemMock =
                fixture
                    .Freeze<Mock<IFileSystem>>()
                    .SetupDefaults()
                    .SetupSourceDirectoryWithFiles(SourceDirPath, FilesWithMetadata.Select(f => f.sourceFile));
            fixture
                .Freeze<Mock<IMetadataReader>>()
                .SetupMetadata(FilesWithMetadata.Select(f => (file: f.sourceFile, tags: f.exifTags)));
            var consoleTextWriterMock = new StringWriter();
            fixture.Inject<TextWriter>(consoleTextWriterMock);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute($"copy {SourceDirPath} {DestinationDirPath}".Split());

            /* Assert */
            result.Should().Be(0);
            var output = consoleTextWriterMock.ToString();
            foreach (var (file, _, expectedDestinationDirectory, expectedDestinationFile) in FilesWithMetadata)
            {
                VerifyDirectoryCreatedAndFileCopied(file, expectedDestinationDirectory, expectedDestinationFile);
                output.Should()
                    .Contain($"Created {expectedDestinationDirectory}").And
                    .Contain($"{file} -> {expectedDestinationDirectory}/{expectedDestinationFile}");
            }

            var filesCount = FilesWithMetadata.Count();
            output.Should().EndWith(
                $"Found {filesCount} camera file(s). Processed {filesCount}. Skipped 0. Transferred {filesCount}.\n");


            void VerifyDirectoryCreatedAndFileCopied(
                string sourceFile,
                string destinationDirectory,
                string destinationFile)
            {
                fileSystemMock.Verify(fs => fs.Directory.CreateDirectory(destinationDirectory));
                fileSystemMock.Verify(fs =>
                    fs.File.Copy(
                        sourceFile,
                        $"{destinationDirectory}/{destinationFile}",
                        false));
            }
        }

        [Fact]
        public void Move_directory_copies_all_supported_files()
        {
            /* Arrange */
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var fileSystemMock =
                fixture
                    .Freeze<Mock<IFileSystem>>()
                    .SetupDefaults()
                    .SetupSourceDirectoryWithFiles(SourceDirPath, FilesWithMetadata.Select(f => f.sourceFile));
            fixture
                .Freeze<Mock<IMetadataReader>>()
                .SetupMetadata(FilesWithMetadata.Select(f => (file: f.sourceFile, tags: f.exifTags)));
            var consoleTextWriterMock = new StringWriter();
            fixture.Inject<TextWriter>(consoleTextWriterMock);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute($"move {SourceDirPath} {DestinationDirPath}".Split());

            /* Assert */
            result.Should().Be(0);
            var output = consoleTextWriterMock.ToString();
            foreach (var (file, _, expectedDestinationDirectory, expectedDestinationFile) in FilesWithMetadata)
            {
                VerifyDirectoryCreatedAndFileCopied(file, expectedDestinationDirectory, expectedDestinationFile);
                output.Should()
                    .Contain($"Created {expectedDestinationDirectory}").And
                    .Contain($"{file} -> {expectedDestinationDirectory}/{expectedDestinationFile}");
            }

            var filesCount = FilesWithMetadata.Count();
            output.Should().EndWith(
                $"Found {filesCount} camera file(s). Processed {filesCount}. Skipped 0. Transferred {filesCount}.\n");


            void VerifyDirectoryCreatedAndFileCopied(
                string sourceFile,
                string destinationDirectory,
                string destinationFile)
            {
                fileSystemMock.Verify(fs => fs.Directory.CreateDirectory(destinationDirectory));
                fileSystemMock.Verify(fs =>
                    fs.File.Move(
                        sourceFile,
                        $"{destinationDirectory}/{destinationFile}",
                        false));
            }
        }

        [Fact]
        public void Copy_directory_skips_files_which_already_exist_in_the_destination()
        {
            /* Arrange */
            var fileWithMetadata =
            (
                sourceFile: $"{SourceDirPath}/IMG_1234.JPG",
                exifTags: NewImageFileTags("2010:01:12 13:14:15", "42"),
                expectedDestinationDirectory: $"{DestinationDirPath}/2010_01_12",
                expectedDestinationFile: $"{DestinationDirPath}/2010_01_12/IMG_20100112_131415420.JPG");
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var fileSystemMock =
                fixture
                    .Freeze<Mock<IFileSystem>>()
                    .SetupDefaults()
                    .SetupSourceDirectoryWithFiles(SourceDirPath, new[] { fileWithMetadata.sourceFile });
            fileSystemMock
                .Setup(fs => fs.Directory.Exists(fileWithMetadata.expectedDestinationDirectory))
                .Returns(true);
            fileSystemMock
                .Setup(fs => fs.File.Exists(fileWithMetadata.expectedDestinationFile))
                .Returns(true);
            fixture
                .Freeze<Mock<IMetadataReader>>()
                .SetupMetadata(new[] { (fileWithMetadata.sourceFile, fileWithMetadata.exifTags) });
            var consoleTextWriterMock = new StringWriter();
            fixture.Inject<TextWriter>(consoleTextWriterMock);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute($"copy {SourceDirPath} {DestinationDirPath}".Split());

            /* Assert */
            result.Should().Be(0);
            fileSystemMock.Verify(fs =>
                    fs.File.Copy(It.IsAny<string>(), fileWithMetadata.expectedDestinationFile, It.IsAny<bool>()),
                Times.Never);

            var output = consoleTextWriterMock.ToString();
            output.Should()
                .NotContain($"Created {fileWithMetadata.expectedDestinationDirectory}").And
                .Contain(
                    $"Skipped {fileWithMetadata.sourceFile} " +
                    $"({fileWithMetadata.expectedDestinationFile} already exists");
            output.Should().EndWith(
                "Skipped 1 file(s) because they already exist at the destination.\n" +
                $"{fileWithMetadata.sourceFile} exists as {fileWithMetadata.expectedDestinationFile}\n\n" +
                "Found 1 camera file(s). Processed 1. Skipped 1. Transferred 0.\n");
        }

        [Fact]
        public void Copy_overwrites_files_which_already_exist_in_the_destination_in_overwrite_mode()
        {
            /* Arrange */
            var fileWithMetadata =
            (
                sourceFile: $"{SourceDirPath}/IMG_1234.JPG",
                exifTags: NewImageFileTags("2010:01:12 13:14:15", "42"),
                expectedDestinationDirectory: $"{DestinationDirPath}/2010_01_12",
                expectedDestinationFile: $"{DestinationDirPath}/2010_01_12/IMG_20100112_131415420.JPG");
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var fileSystemMock =
                fixture
                    .Freeze<Mock<IFileSystem>>()
                    .SetupDefaults()
                    .SetupSourceDirectoryWithFiles(SourceDirPath, new[] { fileWithMetadata.sourceFile });
            fileSystemMock
                .Setup(fs => fs.Directory.Exists(fileWithMetadata.expectedDestinationDirectory))
                .Returns(true);
            fileSystemMock
                .Setup(fs => fs.File.Exists(fileWithMetadata.expectedDestinationFile))
                .Returns(true);
            fixture
                .Freeze<Mock<IMetadataReader>>()
                .SetupMetadata(new[] { (fileWithMetadata.sourceFile, fileWithMetadata.exifTags) });
            var consoleTextWriterMock = new StringWriter();
            fixture.Inject<TextWriter>(consoleTextWriterMock);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute($"copy {SourceDirPath} {DestinationDirPath} --overwrite".Split());

            /* Assert */
            result.Should().Be(0);
            var output = consoleTextWriterMock.ToString();
            var (file, _, _, expectedDestinationFile) = fileWithMetadata;
            VerifyDirectoryCreatedAndFileCopied(file, expectedDestinationFile);
            output.Should().Contain($"{file} -> {expectedDestinationFile}");
            output.Should().EndWith("Found 1 camera file(s). Processed 1. Skipped 0. Transferred 1.\n");

            void VerifyDirectoryCreatedAndFileCopied(
                string sourceFile,
                string destinationFile)
            {
                fileSystemMock.Verify(fs =>
                    fs.File.Copy(
                        sourceFile,
                        destinationFile,
                        true));
            }
        }

        [Fact]
        public void Error_is_printed_when_source_directory_does_not_exist()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var consoleTextWriterMock = new StringWriter();
            fixture.Inject<TextWriter>(consoleTextWriterMock);
            var sut = fixture.Create<Program>();

            var result = sut.Execute("copy not-existing output-dir".Split());

            result.Should().NotBe(0);
            var output = consoleTextWriterMock.ToString();
            output.Should().StartWith("not-existing does not exist.");
        }

        [Theory]
        [InlineData("copy")]
        [InlineData("move")]
        public void Files_are_not_transferred_in_dry_run_mode(
            string command)
        {
            /* Arrange */
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var fileSystemMock =
                fixture
                    .Freeze<Mock<IFileSystem>>()
                    .SetupDefaults()
                    .SetupSourceDirectoryWithFiles(SourceDirPath, FilesWithMetadata.Select(f => f.sourceFile));
            fixture
                .Freeze<Mock<IMetadataReader>>()
                .SetupMetadata(FilesWithMetadata.Select(f => (file: f.sourceFile, tags: f.exifTags)));
            var consoleTextWriterMock = new StringWriter();
            fixture.Inject<TextWriter>(consoleTextWriterMock);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute(
                $"{command} --dry-run {SourceDirPath} {DestinationDirPath}".Split());

            /* Assert */
            result.Should().Be(0);
            var output = consoleTextWriterMock.ToString();
            fileSystemMock.Verify(fs =>
                    fs.File.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
            fileSystemMock.Verify(fs =>
                    fs.File.Copy(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
            fileSystemMock.Verify(fs =>
                    fs.File.Move(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
            fileSystemMock.Verify(fs =>
                    fs.File.Move(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);

            foreach (var (file, _, expectedDestinationDirectory, expectedDestinationFile) in FilesWithMetadata)
            {
                output.Should()
                    .NotContain($"Created {expectedDestinationDirectory}").And
                    .Contain($"{file} -> {expectedDestinationDirectory}/{expectedDestinationFile}");
            }

            var filesCount = FilesWithMetadata.Count();
            output.Should().EndWith(
                $"Found {filesCount} camera file(s). Processed {filesCount}. Skipped 0. Transferred 0.\n");
        }

        [Fact]
        public void First_error_stops_execution()
        {
            /* Arrange */
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var fileSystemMock =
                fixture
                    .Freeze<Mock<IFileSystem>>()
                    .SetupDefaults()
                    .SetupSourceDirectoryWithFiles(
                        SourceDirPath,
                        new[] { $"{SourceDirPath}/IMG_1234.JPG", $"{SourceDirPath}/IMG_4231.JPEG" });
            var metadataReader = fixture.Freeze<Mock<IMetadataReader>>();
            metadataReader
                .Setup(mr => mr.ExtractTags(new CameraFilePath($"{SourceDirPath}/IMG_1234.JPG")))
                .Throws(new Exception("PROCESSING EXCEPTION"));
            metadataReader
                .Setup(mr => mr.ExtractTags(new CameraFilePath($"{SourceDirPath}/IMG_4231.JPEG")))
                .Returns(NewImageFileTags("2011:02:13 14:15:16", "43"));

            var consoleTextWriterMock = new StringWriter();
            fixture.Inject<TextWriter>(consoleTextWriterMock);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute($"copy {SourceDirPath} {DestinationDirPath}".Split());

            /* Assert */
            result.Should().NotBe(0);
            var output = consoleTextWriterMock.ToString();
            output.Should().Be(
                $"Failed {SourceDirPath}/IMG_1234.JPG: PROCESSING EXCEPTION\n\n" +
                "Found 2 camera file(s). Processed 1. Skipped 0. Transferred 0.\n");
            fileSystemMock.Verify(fs =>
                    fs.File.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
            fileSystemMock.Verify(fs =>
                    fs.File.Copy(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public void Errors_do_not_stop_execution_in_keep_going_mode()
        {
            /* Arrange */
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var fileSystemMock =
                fixture
                    .Freeze<Mock<IFileSystem>>()
                    .SetupDefaults()
                    .SetupSourceDirectoryWithFiles(
                        SourceDirPath,
                        new[]
                        {
                            $"{SourceDirPath}/IMG_1234.JPG",
                            $"{SourceDirPath}/IMG_2345.jpg",
                            $"{SourceDirPath}/IMG_3456.JPEG"
                        });
            var metadataReaderStub = fixture.Freeze<Mock<IMetadataReader>>();
            metadataReaderStub
                .Setup(mr => mr.ExtractTags(new CameraFilePath($"{SourceDirPath}/IMG_1234.JPG")))
                .Throws(new Exception("PROCESSING EXCEPTION"));
            metadataReaderStub
                .Setup(mr => mr.ExtractTags(new CameraFilePath($"{SourceDirPath}/IMG_2345.jpg")))
                .Returns(Array.Empty<ITag>());
            metadataReaderStub
                .Setup(mr => mr.ExtractTags(new CameraFilePath($"{SourceDirPath}/IMG_3456.JPEG")))
                .Returns(NewImageFileTags("2011:02:13 14:15:16", "43"));

            var consoleTextWriterMock = new StringWriter();
            fixture.Inject<TextWriter>(consoleTextWriterMock);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute(
                $"copy {SourceDirPath} {DestinationDirPath} --keep-going".Split());

            /* Assert */
            result.Should().NotBe(0);
            var output = consoleTextWriterMock.ToString();
            output.Should().Be(
                $"Failed {SourceDirPath}/IMG_1234.JPG: PROCESSING EXCEPTION\n" +
                $"Failed {SourceDirPath}/IMG_2345.jpg: Metadata not found\n" +
                $"Created {DestinationDirPath}/2011_02_13\n" +
                $"{SourceDirPath}/IMG_3456.JPEG -> {DestinationDirPath}/2011_02_13/IMG_20110213_141516430.JPEG\n\n" +
                "Following errors occurred:\n" +
                $"{SourceDirPath}/IMG_1234.JPG: PROCESSING EXCEPTION\n" +
                $"{SourceDirPath}/IMG_2345.jpg: Metadata not found\n\n" +
                "Found 3 camera file(s). Processed 3. Skipped 0. Transferred 1.\n");
            fileSystemMock.Verify(fs =>
                    fs.File.Copy($"{SourceDirPath}/IMG_1234.JPG", It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
            fileSystemMock.Verify(fs =>
                    fs.File.Copy($"{SourceDirPath}/IMG_2345.jpg", It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
            fileSystemMock.Verify(fs =>
                fs.File.Copy(
                    $"{SourceDirPath}/IMG_3456.JPEG",
                    $"{DestinationDirPath}/2011_02_13/IMG_20110213_141516430.JPEG",
                    false));
        }

        [Fact]
        public void Cancellation_stops_execution_and_prints_summary()
        {
            /* Arrange */
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var fileSystemMock =
                fixture
                    .Freeze<Mock<IFileSystem>>()
                    .SetupDefaults()
                    .SetupSourceDirectoryWithFiles(
                        SourceDirPath,
                        new[]
                        {
                            $"{SourceDirPath}/IMG_1234.JPG",
                            $"{SourceDirPath}/IMG_4231.JPEG",
                            $"{SourceDirPath}/IMG_1234.CR2"
                        });
            var cancellationTokenSource = new CancellationTokenSource();
            fixture.Inject(cancellationTokenSource);
            var metadataReaderStub = fixture.Freeze<Mock<IMetadataReader>>();
            metadataReaderStub
                .Setup(mr => mr.ExtractTags(new CameraFilePath($"{SourceDirPath}/IMG_1234.JPG")))
                .Returns(NewImageFileTags("2010:01:12 13:14:15", "42"));
            metadataReaderStub
                .Setup(mr => mr.ExtractTags(new CameraFilePath($"{SourceDirPath}/IMG_4231.JPEG")))
                .Returns(NewImageFileTags("2011:02:13 14:15:16", "43"))
                /* Simulate pressing Ctrl-C after second file has been processed */
                .Callback(cancellationTokenSource.Cancel);

            var consoleTextWriterMock = new StringWriter();
            fixture.Inject<TextWriter>(consoleTextWriterMock);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute($"copy {SourceDirPath} {DestinationDirPath}".Split());

            /* Assert */
            result.Should().NotBe(0);
            var output = consoleTextWriterMock.ToString();
            output.Should().Be(
                $"Created {DestinationDirPath}/2010_01_12\n" +
                $"{SourceDirPath}/IMG_1234.JPG -> {DestinationDirPath}/2010_01_12/IMG_20100112_131415420.JPG\n" +
                $"Created {DestinationDirPath}/2011_02_13\n" +
                $"{SourceDirPath}/IMG_4231.JPEG -> {DestinationDirPath}/2011_02_13/IMG_20110213_141516430.JPEG\n\n" +
                "Found 3 camera file(s). Processed 2. Skipped 0. Transferred 2.\n" +
                "Operation interrupted by user.\n");
            fileSystemMock.Verify(fs =>
                fs.File.Copy(
                    $"{SourceDirPath}/IMG_1234.JPG",
                    $"{DestinationDirPath}/2010_01_12/IMG_20100112_131415420.JPG",
                    false));
            fileSystemMock.Verify(fs =>
                fs.File.Copy(
                    $"{SourceDirPath}/IMG_4231.JPEG",
                    $"{DestinationDirPath}/2011_02_13/IMG_20110213_141516430.JPEG",
                    false));
            fileSystemMock.Verify(fs =>
                    fs.File.Copy(
                        $"{SourceDirPath}/IMG_1234.CR2",
                        It.IsAny<string>(),
                        It.IsAny<bool>()),
                Times.Never);
        }
    }

    public sealed class WithoutDateSubdirectory
    {
        private static readonly IEnumerable<(
                string sourceFile,
                IEnumerable<ITag> exifTags,
                string expectedDestinationFile)>
            FilesWithMetadata = new[]
            {
                (
                    $"{SourceDirPath}/IMG_1234.JPG",
                    NewImageFileTags("2010:01:12 13:14:15", "42"),
                    "IMG_20100112_131415420.JPG"),
                (
                    $"{SourceDirPath}/IMG_1234.CR2",
                    NewImageFileTags("2012:03:14 15:16:17", "44"),
                    "IMG_20120314_151617440.CR2")
            };

        [Fact]
        public void Copy_directory_copies_all_supported_files()
        {
            /* Arrange */
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var fileSystemMock =
                fixture
                    .Freeze<Mock<IFileSystem>>()
                    .SetupDefaults()
                    .SetupSourceDirectoryWithFiles(SourceDirPath, FilesWithMetadata.Select(f => f.sourceFile));
            fixture
                .Freeze<Mock<IMetadataReader>>()
                .SetupMetadata(FilesWithMetadata.Select(f => (file: f.sourceFile, tags: f.exifTags)));
            var consoleTextWriterMock = new StringWriter();
            fixture.Inject<TextWriter>(consoleTextWriterMock);
            var sut = fixture.Create<Program>();

            /* Act */
            var result = sut.Execute(
                $"copy {SourceDirPath} {DestinationDirPath} --skip-date-subdir".Split());

            /* Assert */
            result.Should().Be(0);
            var output = consoleTextWriterMock.ToString();
            foreach (var (file, _, expectedDestinationFile) in FilesWithMetadata)
            {
                VerifyDirectoryCreatedAndFileCopied(file, DestinationDirPath, expectedDestinationFile);
                output.Should()
                    .Contain($"Created {DestinationDirPath}").And
                    .Contain($"{file} -> {DestinationDirPath}/{expectedDestinationFile}");
            }

            var filesCount = FilesWithMetadata.Count();
            output.Should().EndWith(
                $"Found {filesCount} camera file(s). Processed {filesCount}. Skipped 0. Transferred {filesCount}.\n");


            void VerifyDirectoryCreatedAndFileCopied(
                string sourceFile,
                string destinationDirectory,
                string destinationFile)
            {
                fileSystemMock.Verify(fs => fs.Directory.CreateDirectory(destinationDirectory));
                fileSystemMock.Verify(fs =>
                    fs.File.Copy(
                        sourceFile,
                        $"{destinationDirectory}/{destinationFile}",
                        false));
            }
        }
    }
}