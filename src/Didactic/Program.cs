

using CommandLine;
using Didactic.CliOptions;
using System;
using Waddle;

namespace Didactic
{
    class Program
    {
        static int Main(string[] args)
        {
            var pat = System.Environment.GetEnvironmentVariable("AzDOAADJoinedPAT");
            var orgUri = System.Environment.GetEnvironmentVariable("AzDOAADJoinedURL");
            return Parser.Default.ParseArguments<ApplyOptions>(args)
               .MapResult(
                 (ApplyOptions opts) => new CliRunner().RunApplyVerb(opts, orgUri, pat),
                 errs => 1);
        }
    }
}
