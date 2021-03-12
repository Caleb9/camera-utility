namespace CameraUtility.Commands.ImageFilesTransfer
{
    internal sealed class CopyCommand :
        AbstractTransferImageFilesCommand
    {
        private const string DescriptionText = "Copies suppported image files between two directories.";

        public CopyCommand(
            OptionsHandler handler)
            : base("copy", DescriptionText, handler)
        {
        }
    }
}