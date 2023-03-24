

using Cielo.Azdo.Abstract;
using Cielo.Azdo.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Azdo
{
    public class AclService : RestServiceBase
    {
        public AclService(IHttpClientFactory clientFactory) : base(clientFactory) { }

        public async Task<VstsAclList> GetAllAclsAsync(Guid namespaceId)
        {
            var path = $"_apis/accesscontrollists/{namespaceId}?api-version=6.0";
            var aclList = await CoreApi().GetRestAsync<VstsAclList>(path);

            return aclList;
        }

        public async Task<VstsAclList> GetAllAclsByTokenAsync(Guid namespaceId, string token)
        {
            var path = $"_apis/accesscontrollists/{namespaceId}?token={RestUtils.UriEncode(token)}&api-version=6.0";
            var aclList = await CoreApi()
                .GetRestAsync<VstsAclList>(path);

            return aclList;
        }

        public async Task<bool> SetAclsAsync(
            Guid namespaceId, string token,
            Dictionary<string, VstsAcesDictionaryEntry> aces, bool inherit = true)
        {
            var payload = new
            {
                value = new List<object>
                {
                    new VstsAclEntry { Token = token, AcesDictionary = aces, InheritPermissions = inherit }
                }
            };
            return await CoreApi().PostWithoutResponseBodyAsync(
               $"_apis/accesscontrollists/{namespaceId}?api-version=6.0",
               payload);
        }
    }
}
