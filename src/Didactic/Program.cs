

using CommandLine;
using Didactic.CliOptions;

namespace Didactic
{
    class Program
    {
        static int Main(string[] args)
        {

            return Parser.Default.ParseArguments<ApplyOptions>(args)
               .MapResult(
                 (ApplyOptions opts) => 
                 {
                     if(!string.IsNullOrWhiteSpace(opts.OrganizationURL))
                     {
                         if (!opts.OrganizationURL.EndsWith("/")) {
                             opts.OrganizationURL = $"{opts.OrganizationURL}/";
                         }
                     }
                     else
                     {
                         opts.OrganizationURL = System.Environment.GetEnvironmentVariable("AzDOAADJoinedURL");
                     }
                     if (string.IsNullOrWhiteSpace(opts.PAT)) 
                     {
                         opts.PAT = System.Environment.GetEnvironmentVariable("AzDOAADJoinedPAT");
                     }
                     return new CliRunner().RunApplyVerb(opts);                    
                 },
                 errs => 1);
        }
    }
}
