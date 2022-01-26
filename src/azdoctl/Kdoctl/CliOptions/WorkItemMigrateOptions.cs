

using CommandLine;

namespace Kdoctl.CliOptions
{
    [Verb("migrate-workitems", HelpText = "Migrate work items to Azure DevOps from diverse sources")]
    public class WorkItemMigrateOptions : MigrationOptions
    {
        [Option('s', "spec", Required = false, HelpText = "For now fake args.")]
        public string Spec { get; set; }
    }
}
