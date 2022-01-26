

using System.Collections.Generic;

namespace Kdoctl.Schema
{
    public abstract class MigrationSchemaBase
    {
        public string ApiVersion { get; set; }

        public MigrationSource Source { get; set; }
        public MigrationDestination Destination { get; set; }
    }

    public class MigrationSource
    {
        public string ProjectName { get; set; }

        public string Query { get; set; }
    }

    public class MigrationDestination
    {
        public string ProjectName { get; set; }
    }

    public class FieldMapRuleSpec
    {
        public string SourcePattern { get; set; }
        public string Destination { get; set; }
    }

    public class WorkItemFieldMigrationMap
    {
        public WorkItemFieldMigrationMap()
        {
            this.Rules = new List<FieldMapRuleSpec>();
        }
        public string Default { get; set; }
        public List<FieldMapRuleSpec> Rules  { get; private set;}
    }

    public class WorkItemMigrationMapping
    {
        public WorkItemFieldMigrationMap AreaPath { get; set; }
        public WorkItemFieldMigrationMap IterationPath { get; set; }
        public WorkItemFieldMigrationMap WorkItemType { get; set; }
        public WorkItemFieldMigrationMap State { get; set; }
    }

    public class WorkItemMigrationSpec : MigrationSchemaBase
    {
        public WorkItemMigrationMapping FieldMaps { get; set; }
    }
}
