using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using Waddle;
using Waddle.Dtos;

namespace Didactic
{
    class Program
    {
        // sampel data in moim org
        // EKS REPO :: https://dev.azure.com/moim/ae92e48d-4a62-4da3-9302-94f8ed43d939/_apis/git/repositories/d5f2d1f0-fc7e-4897-8083-3d1c051e4928
        // 
        static void Main(string[] args)
        {
            var pat = Environment.GetEnvironmentVariable("AzDOAADJoinedPAT");
            var orgUri = Environment.GetEnvironmentVariable("AzDOAADJoinedURL");

            var factory = new AdoConnectionFactory(new Uri(orgUri), pat);
            var repoService = factory.GetRepositoryService();
            var repos = repoService.GetRepositoryListAsync().Result;


            //var secService = factory.GetSecurityNamespaceService();
            //var nscollection = secService.GetAllNamespacesAsync().Result;

            var gitSecurityNamespaceId = "2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87";
            var eksRepoToken = "repoV2/ae92e48d-4a62-4da3-9302-94f8ed43d939/d5f2d1f0-fc7e-4897-8083-3d1c051e4928";
            var kubernetesGroupSid = "Microsoft.TeamFoundation.Identity;S-1-9-1551374245-1204400969-2402986413-2179408616-3-2250019746-1978212418-2861161535-2502070516";
            var secDescriptor = new VstsAcesDictionaryEntry
            {
                Allow  = 16388,
                Deny = 0,
                Descriptor = kubernetesGroupSid
            };
            var aclDictioanry = new Dictionary<string, VstsAcesDictionaryEntry>();
            aclDictioanry.Add(kubernetesGroupSid, secDescriptor);

            var aclService = factory.GetAclListService();
            //var aclCollection = aclService.GetAllAclsAsync(gitSecurityNamespaceId).Result;

            // var aclList = aclService.GetAllAclsByTokenAsync(gitSecurityNamespaceId, eksRepoToken).Result;

            aclService.SetAclsAsync(gitSecurityNamespaceId, eksRepoToken, aclDictioanry, true).Wait();


            Console.WriteLine("test");
        }
    }
}
