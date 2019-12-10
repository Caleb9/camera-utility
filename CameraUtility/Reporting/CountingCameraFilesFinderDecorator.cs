using System;
using System.Collections.Generic;
using System.Linq;

namespace CameraUtility.Reporting
{
    internal sealed class CountingCameraFilesFinderDecorator
        : ICameraFilesFinder
    {
        private readonly ICameraFilesFinder _decorated;
        private readonly Report _report;


        internal CountingCameraFilesFinderDecorator(
            ICameraFilesFinder decorated,
            Report report)
        {
            _decorated = decorated;
            _report = report;
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