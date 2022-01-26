

using Kdoctl.CliOptions;
using Kdoctl.CliServices.AzDoServices;
using Kdoctl.Schema;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.Tasks
{
    public class WorkItemMigrationTask : MigrationTaskBase<SingleMigrationWorkItem>
    {
        private readonly WorkItemMigrateOptions options;
        private WorkItemMigrationSpec spec;
        private WorkItemService client;

        public WorkItemMigrationTask(
            IServiceProvider services, 
            WorkItemMigrateOptions options) : base(services)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));                    
        }

        protected async override Task<IEnumerable<SingleMigrationWorkItem>> ListMigrationItemsAsync()
        {
            var qrs = await GetWorkItemService()
                .ListWorkItemsFromWqlQueryAsync(spec.Source.Query);
            return qrs.Select(q => new SingleMigrationWorkItem(q));
        }

        protected async override Task EnrichMigrationFieldsAsync(SingleMigrationWorkItem migrationItem)
        {
            migrationItem.Fields.Add(SingleMigrationWorkItem.SystemProperties.TeamProject, spec.Destination.ProjectName);
            await Task.CompletedTask;
        }

        protected async override Task MapMigrationFieldsAsync(SingleMigrationWorkItem migrationItem)
        {
            var pm = GetPatternMatchAssistant();

            migrationItem.Fields.Add(
                SingleMigrationWorkItem.SystemProperties.AreaPath,
                await GetFieldValueWithPatternMachings(pm, migrationItem,
                    spec.FieldMaps.AreaPath,
                    SingleMigrationWorkItem.SystemProperties.AreaPath));

            migrationItem.Fields.Add(
                SingleMigrationWorkItem.SystemProperties.IterationPath,
                await GetFieldValueWithPatternMachings(pm, migrationItem,
                    spec.FieldMaps.IterationPath,
                    SingleMigrationWorkItem.SystemProperties.IterationPath));

            migrationItem.Fields.Add(
                SingleMigrationWorkItem.SystemProperties.WorkItemType,
                await GetFieldValueWithPatternMachings(pm, migrationItem,
                    spec.FieldMaps.WorkItemType,
                    SingleMigrationWorkItem.SystemProperties.WorkItemType));

            migrationItem.Fields.Add(
                SingleMigrationWorkItem.SystemProperties.State,
                await GetFieldValueWithPatternMachings(pm, migrationItem,
                    spec.FieldMaps.State,
                    SingleMigrationWorkItem.SystemProperties.State));
        }



        protected async override Task<bool> MigrateItemAsync(SingleMigrationWorkItem migrationItem)
        {
            await client.UpdateWorkItemFieldsAsync(migrationItem.WorkItemRef.Id, migrationItem.Fields);
            return true;
        }

        protected async override Task PrepareMigrationAsync()
        {
            if (!File.Exists(options.SpecPath))
            {
                throw new ArgumentException($"'{nameof(options.SpecPath)}' points to a file ({options.SpecPath}) that doesn't exist.");
            }
            this.spec = Deserialize<WorkItemMigrationSpec>(File.ReadAllText(options.SpecPath));
            this.client = GetWorkItemService();
            await Task.CompletedTask;
        }

        private async Task<string> GetFieldValueWithPatternMachings(
            Supports.PatternMatchAssistant pm,
            SingleMigrationWorkItem migrationItem,
            WorkItemFieldMigrationMap mapConfig,
            string fieldName)
        {
            if (mapConfig.Rules != null && mapConfig.Rules.Any())
            {
                foreach (var rule in mapConfig.Rules)
                {
                    var wi = await migrationItem.EnsureWorkItemLoadedAsync(this.client);
                    var orginalValue = wi.Fields[fieldName] as string;
                    if (!string.IsNullOrWhiteSpace(orginalValue) && pm.IsMatch(rule.SourcePattern, orginalValue))
                    {
                        return rule.Destination;
                    }
                }
            }
            return mapConfig.Default;
        }
    }

    public class SingleMigrationWorkItem
    {
        public class SystemProperties 
        {
            public const string TeamProject = "System.TeamProject";
            public const string AreaPath = "System.AreaPath";
            public const string IterationPath = "System.IterationPath";
            public const string WorkItemType = "System.WorkItemType";
            public const string State = "System.State";
        }

        public SingleMigrationWorkItem(WorkItemReference actualItemRef)
        {
            this.WorkItemRef = actualItemRef;
            this.Fields = new Dictionary<string, object>();
        }
        public WorkItemReference WorkItemRef { get; private set; }
        public Dictionary<string, object> Fields { get; private set; }

        private WorkItem _underlyingItem;

        public async Task<WorkItem> EnsureWorkItemLoadedAsync(WorkItemService service)
        {
            if(_underlyingItem == null)
            {
                _underlyingItem = await service.GetWorkItemAsync(this.WorkItemRef.Id);
            }
            return _underlyingItem;
        }
    }
}