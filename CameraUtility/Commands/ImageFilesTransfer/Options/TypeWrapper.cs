namespace CameraUtility.Commands.ImageFilesTransfer.Options
{
    internal abstract record TypeWrapper<T>(T Value)
    {
        public static implicit operator T(
            TypeWrapper<T> dryRun)
        {
            return dryRun.Value;
        }

        public override string ToString()
        {
            return Value!.ToString() ?? string.Empty;
        }
    }
}