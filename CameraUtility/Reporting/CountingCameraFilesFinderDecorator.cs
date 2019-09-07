using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace CameraUtility.Reporting
{
    internal sealed class CountingCameraFilesFinderDecorator
        : ICameraFilesFinder
    {
        [NotNull] private readonly ICameraFilesFinder _decorated;
        [NotNull] private readonly Report _report;


        internal CountingCameraFilesFinderDecorator(
            [NotNull] ICameraFilesFinder decorated,
            [NotNull] Report report)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _report = report ?? throw new ArgumentNullException(nameof(report));
        }

        

        IEnumerable<string> ICameraFilesFinder.FindCameraFiles(
            string directory)
        {
            var result = _decorated.FindCameraFiles(directory).ToList();
            _report.AddNumberOfFilesFoundIn(directory, result.Count);
            return result;
        }
    }
}