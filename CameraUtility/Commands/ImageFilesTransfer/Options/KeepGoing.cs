namespace CameraUtility.Commands.ImageFilesTransfer.Options
{
    internal sealed record KeepGoing(bool Value) :
        TypeWrapper<bool>(Value);
}