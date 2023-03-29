

using CommandLine;
using Kdoctl.Schema;
using System.Collections.Generic;
using System.Linq;

namespace Kdoctl.CliOptions
{
    [Verb("migrate-workitems", HelpText = "Migrate work items to Azure DevOps from diverse sources")]
    public class WorkItemMigrateOptions : MigrationOptions
    {
        [Option('s', "spec", Required = false, HelpText = "Migration specification in a YAML file.")]
        public string SpecPath { get; set; }

        [Option('b', "base64spec", Required = false, HelpText = "Migration specification in base64 encoded.")]
        public string Base64Content { get; set; }
    }


    public class INTEROP_Payload
    {
        public string? Id { get; set; }

        public string? JobState { get; set; }
        
        public string? RunID { get; set; }

        public INTEROP_ProjectInfo? Source { get; set; }
        public INTEROP_ProjectInfo? Destination { get; set; }

        public string? Query { get; set; }
        public INTEROP_PropertyMap? AreaPath { get; set; }
        public INTEROP_PropertyMap? IterationPath { get; set; }
        public INTEROP_PropertyMap? WorkitemType { get; set; }
        public INTEROP_PropertyMap? State { get; set; }

        public WorkItemMigrationSpec ConvertToSpec()
        {
            var spec = new WorkItemMigrationSpec
            {
                Source = new MigrationSource
                {
                    ProjectName = this.Source.Name,
                    Query = this.Query
                },
                Destination = new MigrationDestination
                {
                    ProjectName = this.Destination.Name
                },
                FieldMaps = new WorkItemMigrationMapping
                {
                    AreaPath = new WorkItemFieldMigrationMap
                    {
                        Default = this.AreaPath.DefaultValue
                    },
                    IterationPath = new WorkItemFieldMigrationMap
                    {
                        Default = this.IterationPath.DefaultValue
                    },
                    WorkItemType = new WorkItemFieldMigrationMap
                    {
                        Default = this.WorkitemType.DefaultValue
                    },
                    State = new WorkItemFieldMigrationMap
                    {
                        Default = this.State.DefaultValue
                    }
                }
            };

            if (this.AreaPath.Rules != null && this.AreaPath.Rules.Any()) 
            {
                spec.FieldMaps.AreaPath.Rules.AddRange(
                    this.AreaPath.Rules.Select(x => new FieldMapRuleSpec 
                    {
                        SourcePattern = x.SourcePattern,
                        Destination = x.Target
                    }));
            }
            if (this.IterationPath.Rules != null && this.IterationPath.Rules.Any())
            {
                spec.FieldMaps.IterationPath.Rules.AddRange(
                    this.IterationPath.Rules.Select(x => new FieldMapRuleSpec
                    {
                        SourcePattern = x.SourcePattern,
                        Destination = x.Target
                    }));
            }
            if (this.WorkitemType.Rules != null && this.WorkitemType.Rules.Any())
            {
                spec.FieldMaps.WorkItemType.Rules.AddRange(
                    this.WorkitemType.Rules.Select(x => new FieldMapRuleSpec
                    {
                        SourcePattern = x.SourcePattern,
                        Destination = x.Target
                    }));
            }
            if (this.State.Rules != null && this.State.Rules.Any())
            {
                spec.FieldMaps.State.Rules.AddRange(
                    this.State.Rules.Select(x => new FieldMapRuleSpec
                    {
                        SourcePattern = x.SourcePattern,
                        Destination = x.Target
                    }));
            }
            return spec;
        }
    }

    public class INTEROP_PropertyMap
    {
        public string? DefaultValue { get; set; }
        public List<INTEROP_MatchCondition>? Rules { get; set; }
    }

    public class INTEROP_MatchCondition
    {
        public string? SourcePattern { get; set; }
        public string? Target { get; set; }
    }

    public class INTEROP_ProjectInfo
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}
