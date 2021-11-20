using CameraUtility.Commands.ImageFilesTransfer.Execution;

namespace CameraUtility.Commands.ImageFilesTransfer.Output;

internal sealed class ReportingOrchestratorDecorator :
    IOrchestrator
{
    private readonly IOrchestrator _decorated;
    private readonly Report _report;

    internal ReportingOrchestratorDecorator(
        IOrchestrator decorated,
        Report report)
    {
        _decorated = decorated;
        _report = report;
    }

    int IOrchestrator.Execute(
        AbstractTransferImageFilesCommand.OptionArgs args)
    {
        try
        {
            return _decorated.Execute(args);
        }
        finally
        {
            _report.PrintReport(args.KeepGoing);
        }
    }
}