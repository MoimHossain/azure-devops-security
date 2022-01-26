

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Kdoctl.CliServices.AzDoServices
{
    public class WorkItemService
    {
        private readonly WorkItemTrackingHttpClient client;

        public WorkItemService( WorkItemTrackingHttpClient client)
        {
            this.client = client;
        }
    }
}
