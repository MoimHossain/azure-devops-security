
using System;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.AzDoServices
{
    public class WorkItemService
    {
        private readonly WorkItemTrackingHttpClient client;

        public WorkItemService(WorkItemTrackingHttpClient client)
        {
            this.client = client;
        }

        public async Task<IEnumerable<WorkItemReference>> ListWorkItemsFromWqlQueryAsync(string wql)
        {
            var queryResult = await client.QueryByWiqlAsync(new Wiql { Query = wql });
            if(queryResult == null || queryResult.WorkItems == null)
            {
                throw new InvalidOperationException("Unexpected query result returned at ListWorkItemsFromWqlQueryAsync");
            }
            return queryResult.WorkItems;
        }

        public async Task<WorkItem> UpdateWorkItemFieldsAsync(int id, Dictionary<string, object> fields)
        {
            var patchDocument = new JsonPatchDocument();

            foreach (var key in fields.Keys)
                patchDocument.Add(new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/" + key,
                    Value = fields[key]
                });

            return await client.UpdateWorkItemAsync(patchDocument, id);
        }

        public async Task<WorkItem> GetWorkItemAsync(int id)
        {
            return await client.GetWorkItemAsync(id);
        }
    }
}
