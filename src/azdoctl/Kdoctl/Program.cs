

using CommandLine;
using Kdoctl.CliOptions;
using System;

namespace Kdoctl
{
    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<ApplyOptions, ExportOptions>(args)
                .MapResult(
                (ApplyOptions applyOpts) => { return new CliRunner().RunApplyVerb(OptionBase.Sanitize(applyOpts)); },
                (ExportOptions exportOpts) => { return new CliRunner().RunExportVerb(OptionBase.Sanitize(exportOpts)); },
                errs => 1);
        }    
    }
}
