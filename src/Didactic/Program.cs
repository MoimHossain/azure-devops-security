using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using Waddle;
using Waddle.Constants;
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

            var group = factory.GetGrouoService().CreateAadGroupByObjectId(Guid.Parse("00238ebc-66c1-4220-a859-ed0a00243f27")).Result;
            // var groups = factory.GetGrouoService().ListGroupsAsync().Result;




            var kubernetesGroupSid = "Microsoft.TeamFoundation.Identity;S-1-9-1551374245-1204400969-2402986413-2179408616-3-2250019746-1978212418-2861161535-2502070516";
            var moimSid = "Microsoft.IdentityModel.Claims.ClaimsIdentity;cac2cc32-7de9-4f3d-8d79-76375427b620\\Moim_Hossain@hotmail.com";

            SetAclsToAreaPath(factory, group.Sid, moimSid);

            // SetAclsToAreaPath(factory, kubernetesGroupSid, moimSid);
            // SetAclsToRepository(factory, kubernetesGroupSid, moimSid);

            Console.WriteLine("test");
        }

        private static void SetAclsToAreaPath(
            AdoConnectionFactory factory,
            string kubernetesGroupSid,
            string moimSid)
        {
            var projects = factory.GetProjectService().GetProjectsAsync().Result;
            var project = projects.Value[0];

            var secService = factory.GetSecurityNamespaceService();
            var cssNamespace = secService.GetNamespaceAsync(SecurityNamespaceConstants.Classifications).Result;
            var secNamespaceId = cssNamespace.NamespaceId;

            var claSvc = factory.GetClassificationService();
            var areapaths = claSvc.GetAllAreaPathsAsync(project.Id).Result;
            
            // token for area paths
            var token = $"vstfs:///Classification/Node/{areapaths.Identifier}";

            var k8sScDescriptor = new VstsAcesDictionaryEntry
            {
                Allow = 1,
                Deny = 0,
                Descriptor = kubernetesGroupSid
            };
            var moimScDescriptor = new VstsAcesDictionaryEntry
            {
                Allow = 1,
                Deny = 0,
                Descriptor = moimSid
            };
            var aclDictioanry = new Dictionary<string, VstsAcesDictionaryEntry>();
            aclDictioanry.Add(kubernetesGroupSid, k8sScDescriptor);
            aclDictioanry.Add(moimSid, moimScDescriptor);

            var aclService = factory.GetAclListService();
            var aclCollection = aclService.GetAllAclsAsync(secNamespaceId).Result;
            //var aclList = aclService.GetAllAclsByTokenAsync(secNamespaceId, token).Result;
            aclService.SetAclsAsync(secNamespaceId, token, aclDictioanry, false).Wait();
        }

        private static void SetAclsToRepository(
            AdoConnectionFactory factory, 
            string kubernetesGroupSid, 
            string moimSid)
        {
            var projects = factory.GetProjectService().GetProjectsAsync().Result;
            var project = projects.Value[0];
            var repoService = factory.GetRepositoryService();
            var repos = repoService.GetRepositoryListAsync().Result;
            var eksRepo = repos[2];
            var secService = factory.GetSecurityNamespaceService();
            var eksRepoToken = $"repoV2/{project.Id}/{eksRepo.Id}";
            var gitNamespace = secService.GetNamespaceAsync(SecurityNamespaceConstants.Git_Repositories).Result;
            var gitSecurityNamespaceId = gitNamespace.NamespaceId;
            var k8sScDescriptor = new VstsAcesDictionaryEntry
            {
                Allow = 16388,
                Deny = 0,
                Descriptor = kubernetesGroupSid
            };
            var moimScDescriptor = new VstsAcesDictionaryEntry
            {
                Allow = 32382,
                Deny = 0,
                Descriptor = moimSid
            };
            var aclDictioanry = new Dictionary<string, VstsAcesDictionaryEntry>();
            aclDictioanry.Add(kubernetesGroupSid, k8sScDescriptor);
            aclDictioanry.Add(moimSid, moimScDescriptor);

            var aclService = factory.GetAclListService();
            var aclCollection = aclService.GetAllAclsAsync(gitSecurityNamespaceId).Result;
            var aclList = aclService.GetAllAclsByTokenAsync(gitSecurityNamespaceId, eksRepoToken).Result;
            aclService.SetAclsAsync(gitSecurityNamespaceId, eksRepoToken, aclDictioanry, false).Wait();
        }
    }
}
