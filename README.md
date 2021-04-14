# Azure DevOps Security/Permissions API 

This repository helps you automate Azure DevOps project provision and setting up permissions for the project. It uses a declarative approach (kind of Infrastructure as code or GitOps if you will). Relavent [blog post can be found in here](https://moimhossain.com/2020/10/10/azure-devops-security-permissions-rest-api/).


# Instructions

The repository containes two projects (once is a Library - produced a DLL and another is the console executable application) and the console executable is named as ```azdoctl.exe```.

The idea is to create a manifest file (yaml format) and apply the changes via the ```azdoctl.exe```:

```bash
> azdoctl apply -f manifest.yaml
```

## Manifest file
You need to create a manifest file to descibe your Azure DevOps project and permissions. The format of the manifest file is in yaml (and idea is borrowed from Kubernetes manufest files.)

### Schema
Here's the scheam of the manifest file:

```yaml
apiVersion: apps/v1
kind: Project
metadata:
  name: Bi-Team-Project
  description: Project for BI Engineering team
template:
  name: Agile
  sourceControlType: Git  
```

Manifest file starts with the team project name and description. Each manifest file can have only one team project
definition. 

### Teams
Next, we can define **teams** for the projecct with following yaml block:
```yaml
teams:
  - name: Bi-Core-Team
    description: The core team that run BI projects
    admins:
      - name: Timothy Green
        id: 4ae3c851-6ef3-4748-bef9-4f809736d538
      - name: Linda
        id: 9c5918c7-ef03-4059-a49e-aa6e6d761423
    membership:
      groups:
        - name: 'UX Specialists'
          id: a2931c86-e975-4220-aa89-dc3f952290f4
      users:
        - name: Timothy Green
          id: 4ae3c851-6ef3-4748-bef9-4f809736d538
        - name: Linda
          id: 9c5918c7-ef03-4059-a49e-aa6e6d761423
``` 
Here we can create **teams** and assign admins and members to them. All the references (name and ids) must be valid in **Azure Active Directory**. Ids are **Object ID** for group or users in Azure Active directory.

### Repository

Next, we can define the repository - that must be created and assigned permissions to.

```yaml
repositories:
  - name: Sample-Git-Repository
    permissions:
      - group: 'Data-Scientists'
        origin: aad
        allowed:
          - GenericRead
          - GenericContribute
          - CreateBranch
          - PullRequestContribute
      - group: 'BI-Scrum-masters'
        origin: aad
        allowed:
          - GenericRead
          - GenericContribute
          - CreateBranch
          - PullRequestContribute
          - PolicyExempt
```

Again, you can apply an Azure AD group with very fine grained permissions to each repository that you want to create.

List of all the allowed permissions:
```
        Administer,
        GenericRead,
        GenericContribute,
        ForcePush,
        CreateBranch,
        CreateTag, 
        ManageNote,    
        PolicyExempt,   
        CreateRepository, 
        DeleteRepository,
        RenameRepository,
        EditPolicies,
        RemoveOthersLocks,
        ManagePermissions,
        PullRequestContribute,
        PullRequestBypassPolicy
```

### Environment

You can create **environments** and assign permissions to them with following yaml block.

```yaml
environments:
  - name: Development-Environment
    description: 'Deployment environment for Developers'
    permissions:
      - group: 'Bi-Developers'
        origin: aad
        roles: 
          - Administrator
  - name: Production-Environment
    description: 'Deployment environment for Production'
    permissions:
      - group: 'Bi-Developers'
        origin: aad
        roles: 
          - User          
```

### Build and Release (pipeline) folders

You can also create **Folders** for build and release pipelines and apply specific permission during bootstrap. That way teams can have fine grained permissions into these folders.

#### Build Pipeline Folders

Here's the snippet for creating build folders.

```yaml
buildFolders:
  - path: '/Bi-Application-Builds'
    permissions:
      - group: 'Bi-Developers'
        origin: aad
        allowed:
          - ViewBuilds
          - QueueBuilds
          - StopBuilds
          - ViewBuildDefinition
          - EditBuildDefinition
          - DeleteBuilds
``` 

And, for the release pipelines:

```yaml
releaseFolders:
  - path: '/Bi-Application-Relases'
    permissions:
      - group: 'Bi-Developers'
        origin: aad
        allowed:
          - ViewReleaseDefinition
          - EditReleaseDefinition
          - ViewReleases
          - CreateReleases
          - EditReleaseEnvironment
          - DeleteReleaseEnvironment
          - ManageDeployments
```

Once you have the yaml file defined, you can apply it as described above.

## Contibute and EULA

The code is provided as-is, with MIT license. You can use it, replicate it, modify it as much as you wish. I would appreciate if you acknowledge the usefulness, but that's not enforced. You are free to use it anyway you want.

And, that also means, the author is not taking any respondiblity to provide any gurantee or such.

Thanks!


