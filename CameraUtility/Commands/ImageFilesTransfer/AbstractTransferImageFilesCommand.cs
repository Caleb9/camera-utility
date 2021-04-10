using System.CommandLine;
using System.CommandLine.Invocation;
using CameraUtility.Commands.ImageFilesTransfer.Options;

namespace CameraUtility.Commands.ImageFilesTransfer
{
    internal abstract class AbstractTransferImageFilesCommand :
        Command
    {
        protected AbstractTransferImageFilesCommand(
            string name,
            string description,
            OptionsHandler handler)
            : base(name, description)
        {
            AddArgument(new SourceArgument());
            AddArgument(new DestinationArgument());
            AddOption(new DryRunOption());
            AddOption(new KeepGoingOption());
            AddOption(new SkipDateSubdirOption());
            AddOption(new OverwriteOption());

            Handler = CommandHandler.Create<string, string, bool, bool, bool, bool>(
                (srcPath, dstDir, dryRun, keepGoing, skipDateSubdir, overwrite) =>
                    handler(
                        new OptionArgs(
                            new SourcePath(srcPath),
                            new DestinationDirectory(dstDir),
                            new DryRun(dryRun),
                            new KeepGoing(keepGoing),
                            new SkipDateSubdirectory(skipDateSubdir),
                            new Overwrite(overwrite))));
        }

        internal delegate int OptionsHandler(OptionArgs options);

        private class SourceArgument :
            Argument<string>
        {
            public SourceArgument()
                : base(
                    "src-path",
                    "Path to a camera file (image or video) or a directory containing camera files. " +
                    "When a directory is specified, all sub-directories will be scanned as well.")
            {
                Arity = ArgumentArity.ExactlyOne;
            }
        }

        private class DestinationArgument :
            Argument<string>
        {
            public DestinationArgument()
                : base(
                    "dst-dir",
                    "Destination directory root path where files will be copied or moved into auto-created " +
                    "sub-directories named after file creation date (e.g. 2019_08_22/), " +
                    "unless --skip-date-subdir option is present.")
            {
                Arity = ArgumentArity.ExactlyOne;
            }
        }

        private class DryRunOption :
            Option<bool>
        {
            public DryRunOption()
                : base(
                    new[] {"--dry-run", "-n"},
                    "If present, no actual files will be transferred. " +
                    "The output will contain information about source and destination paths.")
            {
            }
        }

        private class KeepGoingOption :
            Option<bool>
        {
            public KeepGoingOption()
                : base(
                    new[] {"--keep-going", "-k"},
                    "Try to continue operation when errors for individual files occur.")
            {
            }
        }

        private class SkipDateSubdirOption :
            Option<bool>
        {
            public SkipDateSubdirOption()
                : base(
                    "--skip-date-subdir",
                    "Do not create date sub-directories in destination directory.")
            {
            }
        }
        
        private class OverwriteOption :
            Option<bool>
        {
            public OverwriteOption()
                : base(
                    "--overwrite",
                    "Transfer files even if they already exist in destination.")
            {
            }
        }

        internal sealed record OptionArgs(
            SourcePath SourcePath,
            DestinationDirectory DestinationDirectory,
            DryRun DryRun,
            KeepGoing KeepGoing,
            SkipDateSubdirectory SkipDateSubdirectory,
            Overwrite Overwrite);
    }
}