namespace CameraUtility.Commands.ImageFilesTransfer.Options;

internal sealed record KeepGoing(bool Value) :
    TypedOption<bool>(Value);