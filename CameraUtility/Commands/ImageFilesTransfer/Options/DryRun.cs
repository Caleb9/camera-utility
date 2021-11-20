namespace CameraUtility.Commands.ImageFilesTransfer.Options;

internal sealed record DryRun(bool Value) :
    TypedOption<bool>(Value);