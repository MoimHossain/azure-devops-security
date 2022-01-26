

using CommandLine;

namespace Kdoctl.CliOptions
{
    public abstract class OptionBase
    {
        [Option('u', "url", Required = false, HelpText = "Azure DevOps Organization URL. (e.g. https://dev.azure.com/myorg/)")]
        public string OrganizationURL { get; set; }

        [Option('p', "pat", Required = false, HelpText = "Azure DevOps Personal Access Token.")]
        public string PAT { get; set; }

        public virtual void Sanitize()
        {
            if (!string.IsNullOrWhiteSpace(this.OrganizationURL))
            {
                if (!this.OrganizationURL.EndsWith("/"))
                {
                    this.OrganizationURL = $"{this.OrganizationURL}/";
                }
            }
            else
            {
                this.OrganizationURL = System.Environment.GetEnvironmentVariable("AzDOAADJoinedURL");
            }
            if (string.IsNullOrWhiteSpace(this.PAT))
            {
                this.PAT = System.Environment.GetEnvironmentVariable("AzDOAADJoinedPAT");
            }
        }
    }
}
