using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using CameraUtility.CameraFiles;
using CameraUtility.ExceptionHandling;
using CameraUtility.Exif;
using CameraUtility.FileSystemIsolation;
using CameraUtility.Reporting;
using CommandLine;
using JetBrains.Annotations;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace CameraUtility
{
    public class Program
    {
        [NotNull] private readonly ICameraDirectoryCopier _cameraDirectoryCopier;
        [NotNull] private readonly CancellationTokenSource _cancellationTokenSource;
        [NotNull] private readonly Options _options;
        [NotNull] private readonly Report _report;

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global") /* Used implicitly by AutoFixture in tests */]
        public Program(
            [NotNull] Options options,
            [NotNull] IFileSystem fileSystem,
            [NotNull] IMetadataReader metadataReader,
            [NotNull] CancellationTokenSource cancellationTokenSource)
        {
            if (fileSystem == null)
            {
                throw new ArgumentNullException(nameof(fileSystem));
            }

            if (metadataReader == null)
            {
                throw new ArgumentNullException(nameof(metadataReader));
            }

            /* Composition Root. Out of process resources can be swapped with fakes in tests. */
            _report = new Report();
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
                                    fileSystem)
                                {
                                    Console = Console.Out
                                },
                                _report),
                            options.TryContinueOnError
                        )));
            _cancellationTokenSource = cancellationTokenSource ??
                                       throw new ArgumentNullException(nameof(cancellationTokenSource));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public static void Main(
            [NotNull] [ItemNotNull] string[] args)
        {
            var options = ParseArgs(args);
            if (options == null)
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

        [CanBeNull]
        private static Options ParseArgs(
            [NotNull] IEnumerable<string> args)
        {
            Options result = null;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => result = options);
            return result;
        }

        public void Execute()
        {
            _cameraDirectoryCopier.CopyCameraFiles(
                _options.SourceDirectory,
                _options.DestinationDirectory,
                _options.DryRun,
                _cancellationTokenSource.Token);
        }

        [NotNull]
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
                string sourceDirectory,
                string destinationDirectory,
                bool dryRun,
                bool tryContinueOnError)
            {
                SourceDirectory = sourceDirectory;
                DestinationDirectory = destinationDirectory;
                DryRun = dryRun;
                TryContinueOnError = tryContinueOnError;
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

            [Option('k', "keep-going", Required = false, Default = false,
                HelpText = "Try to continue operation when errors for individual files occur.")]
            public bool TryContinueOnError { get; }
        }
    }
}