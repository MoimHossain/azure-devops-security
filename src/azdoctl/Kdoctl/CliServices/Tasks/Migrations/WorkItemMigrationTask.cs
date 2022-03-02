

using Kdoctl.CliOptions;
using Kdoctl.CliServices.AzDoServices;
using Kdoctl.CliServices.Supports;
using Kdoctl.Schema;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Unicode;
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
            var sucessFlag = false;
            var exponentialBackoffFactor = 5000;
            var retryCount = 3;
            await ExecutionSupports.Retry(async () =>
            {
                await client.UpdateWorkItemFieldsAsync(migrationItem.WorkItemRef.Id, migrationItem.Fields);
                Insights.Trace($"{migrationItem.WorkItemRef.Id}", migrationItem.Fields);

                Insights.Debug("$Migrated item: {migrationItem.WorkItemRef.Id}");
                sucessFlag = true;
            },
            exception => { Insights.TrackException(exception); sucessFlag = false; }, exponentialBackoffFactor, retryCount);
            return sucessFlag;
        }

        protected async override Task PrepareMigrationAsync()
        {
            var sessionId = string.Empty;
            if (!string.IsNullOrWhiteSpace(options.SpecPath))
            {
                if (!File.Exists(options.SpecPath))
                {
                    throw new ArgumentException($"'{nameof(options.SpecPath)}' points to a file ({options.SpecPath}) that doesn't exist.");
                }
                spec = Deserialize<WorkItemMigrationSpec>(File.ReadAllText(options.SpecPath));
                sessionId = $"{nameof(WorkItemMigrationTask)}:{Guid.NewGuid()}";
            }
            
            if(!string.IsNullOrWhiteSpace(options.Base64Content))
            {                
                var interopPayload = Deserialize<INTEROP_Payload>(
                    UTF8Encoding.UTF8.GetString(
                        Convert.FromBase64String(options.Base64Content)));
                spec = interopPayload.ConvertToSpec();
                sessionId = interopPayload.Id;

                Insights.Debug($"Base64Content parsed: Session ID: {interopPayload.Id}");
                Insights.Debug($"Body: {UTF8Encoding.UTF8.GetString(Convert.FromBase64String(options.Base64Content))}");
            }
            Insights.InitializeSession(sessionId);
            client = GetWorkItemService();
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
            WorkItemRef = actualItemRef;
            Fields = new Dictionary<string, object>();
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