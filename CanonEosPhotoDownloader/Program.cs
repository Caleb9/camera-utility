using System;
using System.Diagnostics.CodeAnalysis;
using CanonEosPhotoDownloader.CameraFiles;
using CanonEosPhotoDownloader.Exif;
using CanonEosPhotoDownloader.FileSystemIsolation;
using CommandLine;
using JetBrains.Annotations;

namespace CanonEosPhotoDownloader
{
    public class Program
    {
        [NotNull] private readonly ICameraFileFinder _cameraFileFinder;
        [NotNull] private readonly ICameraFileNameConverter _cameraFileNameConverter;
        [NotNull] private readonly IFileSystem _fileSystem;

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global") /* Used implicitly by AutoFixture in tests */]
        public Program(
            [NotNull] IFileSystem fileSystem,
            [NotNull] IMetadataReader metadataReader)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _cameraFileFinder =
                new CameraFileFinder(
                    fileSystem);
            _cameraFileNameConverter =
                new CameraFileNameConverter(
                    metadataReader,
                    new CameraFileFactory(),
                    fileSystem);
        }

        public static void Main(
            [NotNull] [ItemNotNull] string[] args)
        {
            /* Composition Root */
            var program = new Program(
                new FileSystem(),
                new MetadataReader());

            program.Execute(args);
        }

        public void Execute(
            [NotNull] [ItemNotNull] params string[] args)
        {
            var copier =
                new CameraImageCopier(
                    _fileSystem,
                    _cameraFileFinder,
                    _cameraFileNameConverter)
                {
                    Console = Console.Out
                };

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o => copier.CopyFiles(o.SourceDirectory, o.DestinationDirectory, o.DryRun));
        }

        [UsedImplicitly]
        internal sealed class Options
        {
            public Options(
                string sourceDirectory,
                string destinationDirectory,
                bool dryRun)
            {
                SourceDirectory = sourceDirectory;
                DestinationDirectory = destinationDirectory;
                DryRun = dryRun;
            }

            [Option('s', "src-dir", Required = true,
                HelpText = "Directory containing pictures and/or videos. All sub-directories will be searched too.")]
            public string SourceDirectory { get; }

            [Option('d', "dest-dir", Required = true,
                HelpText = "Destination directory root path where files will be copied into auto-created" +
                           " sub-directories named after file creation date (e.g. 2019_08_22/).")]
            public string DestinationDirectory { get; }

            [Option('n', "dry-run", Required = false, Default = false,
                HelpText = "If present, no actual files will be copied. " +
                           "The output will contain information about source and destination paths.")]
            public bool DryRun { get; }
        }
    }
}