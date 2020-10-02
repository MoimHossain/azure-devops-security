using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Waddle.Dtos;
using Waddle.Supports;

namespace Waddle
{
    public class AclListService : RestServiceBase
    {
        public AclListService(string adoUrl, string pat)
            : base(adoUrl, pat)
        {

        }
    
        public async Task<VstsAclList> GetAllAclsAsync(string namespaceId)
        {
            var path = $"_apis/accesscontrollists/{namespaceId}?api-version=6.0";
            var aclList = await GetAzureDevOpsDefaultUri()
                .GetRestAsync<VstsAclList>(path, await GetBearerTokenAsync());

            return aclList;
        }

        public async Task<VstsAclList> GetAllAclsByTokenAsync(string namespaceId, string token)
        {
            var path = $"_apis/accesscontrollists/{namespaceId}?token={HttpUtility.UrlEncode(token)}&api-version=6.0";
            var aclList = await GetAzureDevOpsDefaultUri()
                .GetRestAsync<VstsAclList>(path, await GetBearerTokenAsync());

            return aclList;
        }

        public async Task SetAclsAsync(
            string namespaceId, string token, 
            Dictionary<string, VstsAcesDictionaryEntry> aces, bool inherit = true)
        {
            var payload = new { 
                value = new List<object> 
                {
                    new VstsAclEntry { Token = token, AcesDictionary = aces, InheritPermissions = inherit }
                }
            };

            var sample = JsonConvert.SerializeObject(payload);

            await GetAzureDevOpsDefaultUri()
               .PostRestAsync<object>(
               $"_apis/accesscontrollists/{namespaceId}?api-version=6.0",
               payload,
               await GetBearerTokenAsync());
        }
    }
}
