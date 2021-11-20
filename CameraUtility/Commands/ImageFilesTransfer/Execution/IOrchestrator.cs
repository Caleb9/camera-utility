namespace CameraUtility.Commands.ImageFilesTransfer.Execution;

internal interface IOrchestrator
{
    int Execute(AbstractTransferImageFilesCommand.OptionArgs args);
}