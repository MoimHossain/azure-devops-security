﻿


```

//var factory = new AdoConnectionFactory(new Uri(orgUri), pat);
//var projects = factory.GetProjectService().GetProjectsAsync().Result;
//var project = projects.Value[0];

//var group = factory.GetGrouoService().CreateAadGroupByObjectId(Guid.Parse("00238ebc-66c1-4220-a859-ed0a00243f27")).Result;
//var groups = factory.GetGrouoService().ListGroupsAsync().Result;
//var identities = factory.GetGrouoService().GetLegacyIdentitiesBySidAsync(group.Sid).Result;
//var localId = identities.Value.First().Id;


//var seService = factory.GetServiceEndpointService();
//var ses = seService.ListServiceEndpointsAsync(project.Id).Result;
//var endpoint = ses.Value.Last();

//seService.SetPermissionProjectLevelAsync(project.Id, endpoint.Id, localId, ServiceEndpointService.ServiceEndpointPermissions.User).Wait();
//seService.SetPermissionCollectionLevelAsync(endpoint.Id, localId).Wait();


////var content = factory.GetSecurityNamespaceService()
////    .GenerateEnumerationsAsync(typeof(SecurityNamespaceConstants).Namespace).Result;


//var kubernetesGroupSid = "Microsoft.TeamFoundation.Identity;S-1-9-1551374245-1204400969-2402986413-2179408616-3-2250019746-1978212418-2861161535-2502070516";
//var moimSid = "Microsoft.IdentityModel.Claims.ClaimsIdentity;cac2cc32-7de9-4f3d-8d79-76375427b620\\Moim_Hossain@hotmail.com";

//// SetAclsToBuildFolders(factory, group.Sid, moimSid);
//// SetAclsToReleaseFolders(factory, group.Sid, moimSid);
//// SetAclsToAreaPath(factory, group.Sid, moimSid);
//// SetAclsToAreaPath(factory, kubernetesGroupSid, moimSid);
//// SetAclsToRepository(factory, kubernetesGroupSid, moimSid);
//Console.WriteLine("test");
return 0;
```

```
    private static void SetAclsToBuildFolders(
        AdoConnectionFactory factory,
        string kubernetesGroupSid,
        string moimSid)
    {
        var projects = factory.GetProjectService().GetProjectsAsync().Result;
        var project = projects.Value[0];

        var buildService = factory.GetBuildService();
        var folders = buildService.ListFoldersAsync(project.Id).Result;
        var fpath = folders.Value[1].Path;

        fpath = fpath.TrimStart("\\".ToCharArray()).Replace("\\", "/");

        var secService = factory.GetSecurityNamespaceService();
        var releaseNamespace = secService.GetNamespaceAsync(SecurityNamespaceConstants.Build).Result;
        var secNamespaceId = releaseNamespace.NamespaceId;
           
        // token for area paths
        var token = $"{project.Id}/{fpath}";

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
        var aclList = aclService.GetAllAclsByTokenAsync(secNamespaceId, token).Result;
        aclService.SetAclsAsync(secNamespaceId, token, aclDictioanry, false).Wait();
    }

    private static void SetAclsToReleaseFolders(
        AdoConnectionFactory factory,
        string kubernetesGroupSid,
        string moimSid)
    {
        var projects = factory.GetProjectService().GetProjectsAsync().Result;
        var project = projects.Value[0];

        var releaseService = factory.GetReleaseService();
        var folders = releaseService.ListFoldersAsync(project.Id).Result;
        var fpath = folders.Value[1].Path;

        fpath = fpath.TrimStart("\\".ToCharArray()).Replace("\\", "/");

        var secService = factory.GetSecurityNamespaceService();
        var releaseNamespace = secService.GetNamespaceAsync(SecurityNamespaceConstants.ReleaseManagement, "AdministerReleasePermissions").Result;
        var secNamespaceId = releaseNamespace.NamespaceId;

          

        // token for area paths
        var token = $"{project.Id}/{fpath}";

        //releaseNamespace = secService.GetNamespaceAsync(SecurityNamespaceConstants.Build).Result;
        //secNamespaceId = releaseNamespace.NamespaceId;
        //token = $"{project.Id}/Test-folder-one";

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
        var aclList = aclService.GetAllAclsByTokenAsync(secNamespaceId, token).Result;
        aclService.SetAclsAsync(secNamespaceId, token, aclDictioanry, false).Wait();
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
```