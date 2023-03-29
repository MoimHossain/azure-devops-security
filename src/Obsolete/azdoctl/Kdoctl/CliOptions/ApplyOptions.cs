

using CommandLine;
using System.Collections.Generic;

namespace Kdoctl.CliOptions
{
    [Verb("apply", HelpText = "Apply a manifest file.")]
    public class ApplyOptions : OptionBase
    {
        [Option('d', "directory", Required = false, HelpText = "Directory where all input files to be processed.")]
        public string Directory { get; set; }

        [Option('f', "files", Required = false, HelpText = "Input file to be processed.")]
        public IEnumerable<string> ManifestFiles { get; set; }
    }
}
