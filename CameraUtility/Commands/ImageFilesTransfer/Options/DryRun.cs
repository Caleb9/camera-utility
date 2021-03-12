namespace CameraUtility.Commands.ImageFilesTransfer.Options
{
    internal sealed record DryRun(bool Value) :
        TypeWrapper<bool>(Value);
}