using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using CameraUtility.CameraFiles;
using CameraUtility.ExceptionHandling;
using CameraUtility.Exif;
using CameraUtility.FileSystemIsolation;
using CameraUtility.Reporting;
using CommandLine;

namespace CameraUtility
{
    public class Program
    {
        private readonly ICameraDirectoryCopier _cameraDirectoryCopier;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Options _options;
        private readonly Report _report;

        /* AutoFixture uses this constructor implicitly. It should not be made private. */
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public Program(
            Options options,
            IFileSystem fileSystem,
            IMetadataReader metadataReader,
            CancellationTokenSource cancellationTokenSource)
        {
            if (fileSystem is null)
            {
                throw new ArgumentNullException(nameof(fileSystem));
            }

            if (metadataReader is null)
            {
                throw new ArgumentNullException(nameof(metadataReader));
            }

            /* Composition Root. Out of process resources can be swapped with fakes in tests. */
            var copyOrMove = options.MoveMode ? CopyOrMoveMode.Move : CopyOrMoveMode.Copy;
            _report = new Report(copyOrMove);
            fileSystem = new CountingFileSystemDecorator(fileSystem, _report);
            _cameraDirectoryCopier =
                new ExceptionHandlingCameraDirectoryCopierDecorator(
                    new CameraDirectoryCopier(
                        new CountingCameraFilesFinderDecorator(
                            new CameraFilesFinder(
                                fileSystem),
                            _report),
                        new ExceptionHandlingCameraFileCopierDecorator(
                            new CountingCameraFileCopierDecorator(
                                new CameraFileCopier(
                                    new CameraFileNameConverter(
                                        new ExceptionHandlingMetadataReaderDecorator(
                                            metadataReader),
                                        new ExceptionHandlingCameraFileFactoryDecorator(
                                            new CameraFileFactory()),
                                        fileSystem),
                                    fileSystem,
                                    options.DryRun,
                                    copyOrMove)
                                {
                                    Console = Console.Out
                                },
                                _report),
                            options.TryContinueOnError
                        )));
            _cancellationTokenSource = cancellationTokenSource ??
                                       throw new ArgumentNullException(nameof(cancellationTokenSource));
            _options = options;
        }

        public static void Main(
            string[] args)
        {
            var options = ParseArgs(args);
            if (options is null)
            {
                Environment.Exit(1);
            }

            /* Compose the application with "real" implementations of FileSystem and MetadataReader. */
            var program =
                new Program(
                    options,
                    new FileSystem(),
                    new MetadataReader(),
                    new CancellationTokenSource());

            Console.CancelKeyPress += program.Abort();

            try
            {
                program.Execute();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation interrupted by user.");
            }
            finally
            {
                program.PrintReport();
            }
        }

        private static Options? ParseArgs(
            IEnumerable<string> args)
        {
            Options? result = null;
            Parser.Default.ParseArguments<Options?>(args)
                .WithParsed(options => result = options);
            return result;
        }

        public void Execute()
        {
            Debug.Assert(_options.SourceDirectory != null, "_options.SourceDirectory != null");
            Debug.Assert(_options.DestinationDirectory != null, "_options.DestinationDirectory != null");
            
            _cameraDirectoryCopier.CopyCameraFiles(
                _options.SourceDirectory,
                _options.DestinationDirectory,
                _cancellationTokenSource.Token);
        }

        private ConsoleCancelEventHandler Abort()
        {
            return (sender, eventArgs) =>
            {
                _cancellationTokenSource.Cancel();
                /* Give us a chance to print out a goodbye message and the report. */
                Thread.Sleep(1000);
            };
        }

        private void PrintReport()
        {
            var printErrors = _options.TryContinueOnError;
            _report.PrintReport(printErrors);
        }

        /// <summary>
        ///     Command line options.
        /// </summary>
        public sealed class Options
        {
            public Options(
                string? sourceDirectory,
                string? destinationDirectory,
                bool dryRun,
                bool tryContinueOnError,
                bool moveMode)
            {
                SourceDirectory = sourceDirectory;
                DestinationDirectory = destinationDirectory;
                DryRun = dryRun;
                TryContinueOnError = tryContinueOnError;
                MoveMode = moveMode;
            }

            [Option('s', "src-dir", Required = true,
                HelpText = "Directory containing pictures and/or videos. All sub-directories will be searched too.")]
            public string? SourceDirectory { get; }

            [Option('d', "dest-dir", Required = true,
                HelpText = "Destination directory root path where files will be copied into auto-created" +
                           " sub-directories named after file creation date (e.g. 2019_08_22/).")]
            public string? DestinationDirectory { get; }

            [Option('n', "dry-run", Required = false, Default = false,
                HelpText = "If present, no actual files will be copied. " +
                           "The output will contain information about source and destination paths.")]
            public bool DryRun { get; }

            [Option('k', "keep-going", Required = false, Default = false,
                HelpText = "Try to continue operation when errors for individual files occur.")]
            public bool TryContinueOnError { get; }
            
            [Option('m', "move", Required = false, Default = false,
                HelpText = "Move files instead of just copying them.")]
            public bool MoveMode { get; }
        }
    }
}