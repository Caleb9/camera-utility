using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading;
using CameraUtility.CameraFiles;
using CameraUtility.Commands;
using CameraUtility.Commands.Check;
using CameraUtility.Commands.Check.Execution;
using CameraUtility.Commands.ImageFilesTransfer;
using CameraUtility.Commands.ImageFilesTransfer.Execution;
using CameraUtility.Commands.ImageFilesTransfer.Output;
using CameraUtility.Exif;

namespace CameraUtility
{
    public sealed class Program
    {
        private const int CancelledExitCode = 2;

        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly CopyCommand _copyCommand;
        private readonly MoveCommand _moveCommand;
        private readonly CheckCommand _checkCommand;

        private readonly TextWriter _outputWriter;

        public Program(
            IFileSystem fileSystem,
            IMetadataReader metadataReader,
            TextWriter outputWriter,
            CancellationTokenSource cancellationTokenSource)
        {
            /* Composition Root. Out of process resources can be swapped with fakes in tests. */
            _outputWriter = outputWriter;
            _cancellationTokenSource = cancellationTokenSource;
            var report = new Report(_outputWriter);
            var cameraFilesFinder = new CameraFilesFinder(fileSystem);
            cameraFilesFinder.OnCameraFilesFound += report.HandleCameraFilesFound;
            var cameraFileFactory = new CameraFileFactory();
            var cameraFileNameConverter =
                new CameraFileNameConverter(
                    metadataReader,
                    cameraFileFactory,
                    fileSystem);
            var consoleOutput = new ConsoleOutput(_outputWriter);
            var cameraFileCopier =
                new CameraFileTransferer(
                    cameraFileNameConverter,
                    fileSystem,
                    (s, d, o) => fileSystem.File.Copy(s, d, o));
            cameraFileCopier.OnFileTransferred +=
                (_, args) => consoleOutput.HandleFileCopied(args.sourceFile, args.destinationFile);
            AssignEventHandlersForCameraFileTransferer(cameraFileCopier);
            var internalCopyingOrchestrator =
                new Orchestrator(
                    cameraFilesFinder,
                    cameraFileCopier,
                    _cancellationTokenSource.Token);
            internalCopyingOrchestrator.OnError +=
                (_, error) => consoleOutput.HandleError(error);
            internalCopyingOrchestrator.OnException +=
                (_, args) => consoleOutput.HandleException(args.filePath, args.exception);
            internalCopyingOrchestrator.OnException +=
                (_, args) => report.AddExceptionForFile(args.filePath, args.exception);
            internalCopyingOrchestrator.OnException +=
                (_, _) => report.IncrementProcessed();
            IOrchestrator copyingOrchestrator =
                new ReportingOrchestratorDecorator(
                    internalCopyingOrchestrator,
                    report);
            var cameraFileMover =
                new CameraFileTransferer(
                    cameraFileNameConverter,
                    fileSystem,
                    (s, d, o) => fileSystem.File.Move(s, d, o));
            cameraFileMover.OnFileTransferred +=
                (_, args) => consoleOutput.HandleFileMoved(args.sourceFile, args.destinationFile);
            AssignEventHandlersForCameraFileTransferer(cameraFileMover);
            var internalMovingOrchestrator =
                new Orchestrator(
                    cameraFilesFinder,
                    cameraFileMover,
                    _cancellationTokenSource.Token);
            internalMovingOrchestrator.OnError +=
                (_, error) => consoleOutput.HandleError(error);
            internalMovingOrchestrator.OnException +=
                (_, args) => consoleOutput.HandleException(args.filePath, args.exception);
            internalMovingOrchestrator.OnException +=
                (_, args) => report.AddExceptionForFile(args.filePath, args.exception);
            internalMovingOrchestrator.OnException +=
                (_, _) => report.IncrementProcessed();
            IOrchestrator movingOrchestrator =
                new ReportingOrchestratorDecorator(
                    internalMovingOrchestrator,
                    report);

            _copyCommand = new CopyCommand(args => copyingOrchestrator.Execute(args));
            _moveCommand = new MoveCommand(args => movingOrchestrator.Execute(args));

            var fileChecker =
                new FileChecker(
                    cameraFilesFinder,
                    metadataReader,
                    cameraFileFactory,
                    new Commands.Check.Output.ConsoleOutput(_outputWriter));
            _checkCommand = new CheckCommand(args => fileChecker.Execute(args));

            void AssignEventHandlersForCameraFileTransferer(
                CameraFileTransferer cameraFileTransferer)
            {
                cameraFileTransferer.OnDirectoryCreated +=
                    (_, directory) => consoleOutput.HandleCreatedDirectory(directory);
                cameraFileTransferer.OnFileSkipped +=
                    (_, args) => consoleOutput.HandleFileSkipped(args.sourceFile, args.destinationFile);
                cameraFileTransferer.OnFileSkipped +=
                    (_, args) => report.AddSkippedFile(args.sourceFile, args.destinationFile);
                cameraFileTransferer.OnFileSkipped +=
                    (_, _) => report.IncrementProcessed();
                cameraFileTransferer.OnFileTransferred +=
                    (_, _) => report.IncrementProcessed();
                cameraFileTransferer.OnFileTransferred +=
                    (_, args) => report.IncrementTransferred(args.dryRun);
            }
        }

        private static int Main(string[] args)
        {
            /* Compose the application with "real" dependencies. */
            var program =
                new Program(
                    new FileSystem(),
                    new MetadataReader(),
                    Console.Out,
                    new CancellationTokenSource());

            Console.CancelKeyPress += program.Abort();

            return program.Execute(args);
        }

        public int Execute(params string[] args)
        {
            var parser =
                new CommandLineBuilder(
                        new RootCommand
                        {
                            _copyCommand,
                            _moveCommand,
                            _checkCommand
                        })
                    /* Workaround https://github.com/dotnet/command-line-api/issues/796#issuecomment-673083521 */
                    .UseVersionOption()
                    .UseHelp()
                    .UseEnvironmentVariableDirective()
                    .UseParseDirective()
                    .UseDebugDirective()
                    .UseSuggestDirective()
                    .RegisterWithDotnetSuggest()
                    .UseTypoCorrections()
                    .UseParseErrorReporting()
                    .CancelOnProcessTermination()
                    .UseExceptionHandler((exception, _) => throw exception)
                    .Build();
            try
            {
                return parser.Invoke(args);
            }
            catch (TargetInvocationException exception) when (exception.InnerException is OperationCanceledException)
            {
                _outputWriter.WriteLine("Operation interrupted by user.");
                return CancelledExitCode;
            }
        }

        private ConsoleCancelEventHandler Abort()
        {
            return (_, _) =>
            {
                _cancellationTokenSource.Cancel();
                /* Give us a chance to print out a goodbye message and the report. */
                Thread.Sleep(1000);
            };
        }
    }
}