

using CommandLine;

namespace Kdoctl.CliOptions
{
    [Verb("migrate-workitems", HelpText = "Migrate work items to Azure DevOps from diverse sources")]
    public class WorkItemMigrateOptions : MigrationOptions
    {
        [Option('s', "spec", Required = true, HelpText = "Migration specification in a YAML file.")]
        public string SpecPath { get; set; }
    }
}
