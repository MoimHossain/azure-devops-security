

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


        protected async Task<VstsGroup> GetOrMaterializeGroupAsync(string groupName, Guid? id = null)
        {
            var gService = this.GetGraphService();

            var existingGroup = await gService.GetGroupByNameAsync(groupName);

            if(existingGroup != null)
            {
                return existingGroup;
            }

            if (existingGroup == null && id.HasValue)
            {
                var group = await gService.CreateAadGroupByObjectId(id.Value);
                return group;
            }
            return null;
        }
    }
}
