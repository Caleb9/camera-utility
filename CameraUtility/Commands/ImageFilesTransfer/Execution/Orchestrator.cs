using System;
using System.Threading;

namespace CameraUtility.Commands.ImageFilesTransfer.Execution
{
    internal sealed class Orchestrator :
        IOrchestrator
    {
        private const int NoErrorsResultCode = 0;
        private const int ErrorResultCode = 2;
        private readonly CameraFilesFinder _cameraFilesFinder;
        private readonly CameraFileTransferer _cameraFileTransferer;
        private readonly CancellationToken _cancellationToken;

        internal Orchestrator(
            CameraFilesFinder cameraFilesFinder,
            CameraFileTransferer cameraFileTransferer,
            CancellationToken cancellationToken)
        {
            _cameraFilesFinder = cameraFilesFinder;
            _cameraFileTransferer = cameraFileTransferer;
            _cancellationToken = cancellationToken;
        }

        int IOrchestrator.Execute(AbstractTransferImageFilesCommand.OptionArgs args)
        {
            var cameraFilesResult = _cameraFilesFinder.FindCameraFiles(args.SourcePath);
            if (cameraFilesResult.IsFailure)
            {
                OnError(this, cameraFilesResult.Error);
                return ErrorResultCode;
            }

            var result = NoErrorsResultCode;
            foreach (var cameraFilePath in cameraFilesResult.Value)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    _cameraFileTransferer.TransferFile(
                        new CameraFileTransferer.Args(
                            cameraFilePath, args.DestinationDirectory, args.DryRun, args.SkipDateSubdirectory));
                }
                catch (Exception exception)
                {
                    OnException(this, (cameraFilePath, exception));
                    if (!args.KeepGoing)
                    {
                        return ErrorResultCode;
                    }

                    result = ErrorResultCode;
                }
            }

            return result;
        }

        internal event EventHandler<string> OnError = (_, _) => { };

        internal event EventHandler<(string filePath, Exception exception)> OnException = (_, _) => { };
    }
}