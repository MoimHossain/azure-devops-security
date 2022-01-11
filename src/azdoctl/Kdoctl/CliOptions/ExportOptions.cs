

using CommandLine;
using Kdoctl.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliOptions
{
    [Verb("export", HelpText = "Export resources from Azure DevOps")]
    public class ExportOptions : OptionBase
    {
        [Option('r', "resources", Required = true, HelpText = "Specific resources to be exported.")]
        public IEnumerable<ManifestKind> Resources { get; set; }

        [Option('d', "directory", Required = true, HelpText = "Directory where all input files to be processed.")]
        public string Directory { get; set; }
    }
}
