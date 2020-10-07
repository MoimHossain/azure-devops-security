

using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Didactic.CliOptions
{
    [Verb("apply", HelpText = "Apply a manifest file.")]
    public class ApplyOptions
    {
        [Option('f', "files", Required = true, HelpText = "Input file to be processed.")]
        public IEnumerable<string> ManifestFiles { get; set; }
    }
}
