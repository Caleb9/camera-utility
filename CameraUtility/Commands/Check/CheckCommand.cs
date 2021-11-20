using System.CommandLine;
using System.CommandLine.Invocation;
using CameraUtility.Commands.Check.Options;

namespace CameraUtility.Commands.Check;

internal sealed class CheckCommand :
    Command
{
    private const string DescriptionText =
        "Scans file or directory for supported image and video files and checks if EXIF metadata is present.";

    internal CheckCommand(
        OptionsHandler handler)
        : base("check", DescriptionText)
    {
        AddAlias("c");

        AddArgument(new SourceArgument());

        Handler =
            CommandHandler.Create<string[]>(srcPaths =>
                handler(
                    new OptionArgs(
                        new SourcePaths(
                            srcPaths))));
    }

    private class SourceArgument :
        Argument<string>
    {
        public SourceArgument()
            : base(
                "src-paths",
                () => ".",
                "Paths to a camera files (image or video) or a directories containing camera files. " +
                "For directories, all sub-directories will be scanned as well. Multiple values can be specified.")
        {
            Arity = ArgumentArity.OneOrMore;
        }
    }

    internal delegate int OptionsHandler(
        OptionArgs options);

    internal sealed record OptionArgs(
        SourcePaths SourcePaths);
}