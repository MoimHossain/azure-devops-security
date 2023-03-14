

using Kdoctl.CliServices.AzDoServices;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static Kdoctl.CliServices.AzDoServices.LowLevels.AzDOHttpSupports;

namespace Kdoctl.CliServices
{
    public partial class StateSynchronizationTask
    {


        protected async Task<VstsGroup> GetGroupByNameAsync(string origin, string groupName, Guid? id = null)
        {
            var gService = this.GetGraphService();
            var groups = await gService.ListGroupsAsync();
            var group = groups.Value
                .FirstOrDefault(g =>
                g.Origin.ToString().Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                g.PrincipalName.Contains(groupName, StringComparison.OrdinalIgnoreCase));

            if (group == null && id.HasValue)
            {
                group = await gService.CreateAadGroupByObjectId(id.Value);
            }
            return group;
        }
    }
}
