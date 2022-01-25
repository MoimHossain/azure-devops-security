

using CommandLine;

namespace Kdoctl.CliOptions
{
    public abstract class OptionBase
    {
        [Option('u', "url", Required = false, HelpText = "Azure DevOps Organization URL. (e.g. https://dev.azure.com/myorg/)")]
        public string OrganizationURL { get; set; }

        [Option('p', "pat", Required = false, HelpText = "Azure DevOps Personal Access Token.")]
        public string PAT { get; set; }

        public static T Sanitize<T>(T opts) where T : OptionBase
        {
            if (!string.IsNullOrWhiteSpace(opts.OrganizationURL))
            {
                if (!opts.OrganizationURL.EndsWith("/"))
                {
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

            return opts;
        }
    }
}
