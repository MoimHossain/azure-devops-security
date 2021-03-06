﻿

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

        [Option('u', "url", Required = false, HelpText = "Azure DevOps Organization URL. (e.g. https://dev.azure.com/myorg/)")]
        public string OrganizationURL { get; set; }

        [Option('p', "pat", Required = false, HelpText = "Azure DevOps Personal Access Token.")]
        public string PAT { get; set; }
    }
}
