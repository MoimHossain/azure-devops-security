﻿


using Kdoctl.CliServices.Abstract;
using Kdoctl.CliServices.AzDoServices.Dtos;
using Kdoctl.CliServices.Supports;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Kdoctl.CliServices.AzDoServices
{
    public class AclListService : RestServiceBase
    {
        public AclListService(IHttpClientFactory clientFactory) : base(clientFactory) { }

        public async Task<VstsAclList> GetAllAclsAsync(Guid namespaceId)
        {
            var path = $"_apis/accesscontrollists/{namespaceId}?api-version=6.0";
            var aclList = await CoreApi()                .GetRestAsync<VstsAclList>(path);

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
