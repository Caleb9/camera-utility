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
            string path)
        {
            var result = _decorated.FindCameraFiles(path).ToList();
            _report.AddNumberOfFilesFoundIn(path, result.Count);
            return result;
        }
    }
}