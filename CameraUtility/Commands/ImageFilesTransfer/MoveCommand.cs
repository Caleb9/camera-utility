namespace CameraUtility.Commands.ImageFilesTransfer
{
    internal sealed class MoveCommand :
        AbstractTransferImageFilesCommand
    {
        private const string DescriptionText = "Moves suppported image files between two directories.";

        public MoveCommand(
            OptionsHandler handler)
            : base("move", DescriptionText, handler)
        {
        }
    }
}