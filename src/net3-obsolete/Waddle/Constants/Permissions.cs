using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Runtime.InteropServices;

namespace Waddle.Constants
{
    [Guid("58450c49-b02d-465a-ab12-59ae512d6531")]
    [Description("Analytics")]
    public enum Analytics
    {
        [Description("View analytics")]
        [DefaultValue(1)]
        Read,
        [Description("Manage analytics permissions")]
        [DefaultValue(2)]
        Administer,
        [Description("Push the data to staging area")]
        [DefaultValue(4)]
        Stage,
        [Description("Execute query without any restrictions on the query form")]
        [DefaultValue(8)]
        ExecuteUnrestrictedQuery,
        [Description("Read EUII data")]
        [DefaultValue(16)]
        ReadEuii
    }

    [Guid("d34d3680-dfe5-4cc6-a949-7d9c68f73cba")]
    [Description("AnalyticsViews")]
    public enum AnalyticsViews
    {
        [Description("View shared Analytics views")]
        [DefaultValue(1)]
        Read,
        [Description("Edit shared Analytics views")]
        [DefaultValue(2)]
        Edit,
        [Description("Delete shared Analytics views")]
        [DefaultValue(4)]
        Delete,
        [Description("Execute Analytics views")]
        [DefaultValue(8)]
        Execute,
        [Description("Manage")]
        [DefaultValue(1024)]
        ManagePermissions
    }

    [Guid("7c7d32f7-0e86-4cd6-892e-b35dbba870bd")]
    [Description("ReleaseManagement")]
    public enum ReleaseManagement
    {
        [Description("View task editor")]
        [DefaultValue(1)]
        ViewTaskEditor,
        [Description("View CD work flow editor")]
        [DefaultValue(2)]
        ViewCDWorkflowEditor,
        [Description("Export release definition")]
        [DefaultValue(4)]
        ExportReleaseDefinition,
        [Description("View legacy UI")]
        [DefaultValue(8)]
        ViewLegacyUI,
        [Description("Deployment summary across projects")]
        [DefaultValue(16)]
        DeploymentSummaryAcrossProjects,
        [Description("View external artifact commits and work items")]
        [DefaultValue(32)]
        ViewExternalArtifactCommitsAndWorkItems
    }

    [Guid("c788c23e-1b46-4162-8f5e-d7585343b5de")]
    [Description("ReleaseManagement")]
    public enum ReleaseManagementEx
    {
        [Description("View release pipeline")]
        [DefaultValue(1)]
        ViewReleaseDefinition,
        [Description("Edit release pipeline")]
        [DefaultValue(2)]
        EditReleaseDefinition,
        [Description("Delete release pipeline")]
        [DefaultValue(4)]
        DeleteReleaseDefinition,
        [Description("Manage release approvers")]
        [DefaultValue(8)]
        ManageReleaseApprovers,
        [Description("Manage releases")]
        [DefaultValue(16)]
        ManageReleases,
        [Description("View releases")]
        [DefaultValue(32)]
        ViewReleases,
        [Description("Create releases")]
        [DefaultValue(64)]
        CreateReleases,
        [Description("Edit release stage")]
        [DefaultValue(128)]
        EditReleaseEnvironment,
        [Description("Delete release stage")]
        [DefaultValue(256)]
        DeleteReleaseEnvironment,
        [Description("Administer release permissions")]
        [DefaultValue(512)]
        AdministerReleasePermissions,
        [Description("Delete releases")]
        [DefaultValue(1024)]
        DeleteReleases,
        [Description("Manage deployments")]
        [DefaultValue(2048)]
        ManageDeployments,
        [Description("Manage release settings")]
        [DefaultValue(4096)]
        ManageReleaseSettings,
        [Description("Manage TaskHub Extension")]
        [DefaultValue(8192)]
        ManageTaskHubExtension
    }

    [Guid("a6cc6381-a1ca-4b36-b3c1-4e65211e82b6")]
    [Description("AuditLog")]
    public enum AuditLog
    {
        [Description("View audit log")]
        [DefaultValue(1)]
        Read,
        [Description("Write to the audit log")]
        [DefaultValue(2)]
        Write,
        [Description("Manage audit streams")]
        [DefaultValue(4)]
        Manage_Streams,
        [Description("Delete audit streams")]
        [DefaultValue(8)]
        Delete_Streams
    }

    [Guid("5a27515b-ccd7-42c9-84f1-54c998f03866")]
    [Description("Identity")]
    public enum Identity
    {
        [Description("View identity information")]
        [DefaultValue(1)]
        Read,
        [Description("Edit identity information")]
        [DefaultValue(2)]
        Write,
        [Description("Delete identity information")]
        [DefaultValue(4)]
        Delete,
        [Description("Manage group membership")]
        [DefaultValue(8)]
        ManageMembership,
        [Description("Create identity scopes")]
        [DefaultValue(16)]
        CreateScope,
        [Description("Restore identity scopes")]
        [DefaultValue(32)]
        RestoreScope
    }

    [Guid("445d2788-c5fb-4132-bbef-09c4045ad93f")]
    [Description("WorkItemTrackingAdministration")]
    public enum WorkItemTrackingAdministration
    {
        [Description("Manage permissions")]
        [DefaultValue(1)]
        ManagePermissions,
        [Description("Destroy attachments")]
        [DefaultValue(2)]
        DestroyAttachments
    }

    [Guid("101eae8c-1709-47f9-b228-0e476c35b3ba")]
    [Description("DistributedTask")]
    public enum DistributedTask
    {
        [Description("View")]
        [DefaultValue(1)]
        View,
        [Description("Manage")]
        [DefaultValue(2)]
        Manage,
        [Description("Listen")]
        [DefaultValue(4)]
        Listen,
        [Description("Administer Permissions")]
        [DefaultValue(8)]
        AdministerPermissions,
        [Description("Use")]
        [DefaultValue(16)]
        Use,
        [Description("Create")]
        [DefaultValue(32)]
        Create
    }

    [Guid("71356614-aad7-4757-8f2c-0fb3bff6f680")]
    [Description("")]
    public enum WorkItemQueryFolders
    {
        [Description("Read")]
        [DefaultValue(1)]
        Read,
        [Description("Contribute")]
        [DefaultValue(2)]
        Contribute,
        [Description("Delete")]
        [DefaultValue(4)]
        Delete,
        [Description("Manage Permissions")]
        [DefaultValue(8)]
        ManagePermissions,
        [Description("Full Control")]
        [DefaultValue(16)]
        FullControl,
        [Description("Record query execution information")]
        [DefaultValue(64)]
        RecordQueryExecutionInfo
    }

    [Guid("2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87")]
    [Description("Git Repositories")]
    public enum GitRepositories
    {
        [Description("Administer")]
        [DefaultValue(1)]
        Administer,
        [Description("Read")]
        [DefaultValue(2)]
        GenericRead,
        [Description("Contribute")]
        [DefaultValue(4)]
        GenericContribute,
        [Description("Force push (rewrite history, delete branches and tags)")]
        [DefaultValue(8)]
        ForcePush,
        [Description("Create branch")]
        [DefaultValue(16)]
        CreateBranch,
        [Description("Create tag")]
        [DefaultValue(32)]
        CreateTag,
        [Description("Manage notes")]
        [DefaultValue(64)]
        ManageNote,
        [Description("Bypass policies when pushing")]
        [DefaultValue(128)]
        PolicyExempt,
        [Description("Create repository")]
        [DefaultValue(256)]
        CreateRepository,
        [Description("Delete repository")]
        [DefaultValue(512)]
        DeleteRepository,
        [Description("Rename repository")]
        [DefaultValue(1024)]
        RenameRepository,
        [Description("Edit policies")]
        [DefaultValue(2048)]
        EditPolicies,
        [Description("Remove others' locks")]
        [DefaultValue(4096)]
        RemoveOthersLocks,
        [Description("Manage permissions")]
        [DefaultValue(8192)]
        ManagePermissions,
        [Description("Contribute to pull requests")]
        [DefaultValue(16384)]
        PullRequestContribute,
        [Description("Bypass policies when completing pull requests")]
        [DefaultValue(32768)]
        PullRequestBypassPolicy
    }

    [Guid("3c15a8b7-af1a-45c2-aa97-2cb97078332e")]
    [Description("VersionControlItems2")]
    public enum VersionControlItems2
    {
        [Description("Read")]
        [DefaultValue(1)]
        Read,
        [Description("Pend a change in a server workspace")]
        [DefaultValue(2)]
        PendChange,
        [Description("Check in")]
        [DefaultValue(4)]
        Checkin,
        [Description("Label")]
        [DefaultValue(8)]
        Label,
        [Description("Lock")]
        [DefaultValue(16)]
        Lock,
        [Description("Revise other users' changes")]
        [DefaultValue(32)]
        ReviseOther,
        [Description("Unlock other users' changes")]
        [DefaultValue(64)]
        UnlockOther,
        [Description("Undo other users' changes")]
        [DefaultValue(128)]
        UndoOther,
        [Description("Administer labels")]
        [DefaultValue(256)]
        LabelOther,
        [Description("Manage permissions")]
        [DefaultValue(1024)]
        AdminProjectRights,
        [Description("Check in other users' changes")]
        [DefaultValue(2048)]
        CheckinOther,
        [Description("Merge")]
        [DefaultValue(4096)]
        Merge,
        [Description("Manage branch")]
        [DefaultValue(8192)]
        ManageBranch
    }

    [Guid("2bf24a2b-70ba-43d3-ad97-3d9e1f75622f")]
    [Description("EventSubscriber")]
    public enum EventSubscriber
    {
        [Description("View")]
        [DefaultValue(1)]
        GENERIC_READ,
        [Description("Edit")]
        [DefaultValue(2)]
        GENERIC_WRITE
    }

    [Guid("5a6cd233-6615-414d-9393-48dbb252bd23")]
    [Description("WorkItemTrackingProvision")]
    public enum WorkItemTrackingProvision
    {
        [Description("Administer")]
        [DefaultValue(1)]
        Administer,
        [Description("Manage work item link types")]
        [DefaultValue(2)]
        ManageLinkTypes
    }

    [Guid("49b48001-ca20-4adc-8111-5b60c903a50c")]
    [Description("ServiceEndpoints")]
    public enum ServiceEndpoints
    {
        [Description("Use Service Connection")]
        [DefaultValue(1)]
        Use,
        [Description("Administer Service Connection")]
        [DefaultValue(2)]
        Administer,
        [Description("Create Service Connection")]
        [DefaultValue(4)]
        Create,
        [Description("View Authorization")]
        [DefaultValue(8)]
        ViewAuthorization,
        [Description("View Service Connection")]
        [DefaultValue(16)]
        ViewEndpoint
    }

    [Guid("cb594ebe-87dd-4fc9-ac2c-6a10a4c92046")]
    [Description("ServiceHooks")]
    public enum ServiceHooks
    {
        [Description("View Subscriptions")]
        [DefaultValue(1)]
        ViewSubscriptions,
        [Description("Edit Subscription")]
        [DefaultValue(2)]
        EditSubscriptions,
        [Description("Delete Subscriptions")]
        [DefaultValue(4)]
        DeleteSubscriptions,
        [Description("Publish Events")]
        [DefaultValue(8)]
        PublishEvents
    }

    [Guid("bc295513-b1a2-4663-8d1a-7017fd760d18")]
    [Description("Chat")]
    public enum Chat
    {
        [Description("Read Chat Room Metadata")]
        [DefaultValue(1)]
        ReadChatRoomMetadata,
        [Description("Update Chat Room Metadata")]
        [DefaultValue(2)]
        UpdateChatRoomMetadata,
        [Description("Create Chat Room")]
        [DefaultValue(4)]
        CreateChatRoom,
        [Description("Close Chat Room")]
        [DefaultValue(8)]
        CloseChatRoom,
        [Description("Delete Chat Room")]
        [DefaultValue(16)]
        DeleteChatRoom,
        [Description("Add/Remove Chat Room Member")]
        [DefaultValue(32)]
        AddRemoveChatRoomMember,
        [Description("Read Chat Room Message")]
        [DefaultValue(64)]
        ReadChatRoomMessage,
        [Description("Write Chat Room Message")]
        [DefaultValue(128)]
        WriteChatRoomMessage,
        [Description("Update Chat Room Message")]
        [DefaultValue(256)]
        UpdateChatRoomMessage,
        [Description("Delete Chat Room Message")]
        [DefaultValue(512)]
        DeleteChatRoomMessage,
        [Description("Read Chat Room Transcript")]
        [DefaultValue(1024)]
        ReadChatRoomTranscript,
        [Description("Manage Chat Permissions")]
        [DefaultValue(2048)]
        ManageChatPermissions
    }

    [Guid("3e65f728-f8bc-4ecd-8764-7e378b19bfa7")]
    [Description("Collection")]
    public enum Collection
    {
        [Description("View collection-level information")]
        [DefaultValue(1)]
        GENERIC_READ,
        [Description("Edit collection-level information")]
        [DefaultValue(2)]
        GENERIC_WRITE,
        [Description("Create new projects")]
        [DefaultValue(4)]
        CREATE_PROJECTS,
        [Description("Trigger events")]
        [DefaultValue(16)]
        TRIGGER_EVENT,
        [Description("Manage process template")]
        [DefaultValue(32)]
        MANAGE_TEMPLATE,
        [Description("Alter trace settings")]
        [DefaultValue(64)]
        DIAGNOSTIC_TRACE,
        [Description("View system synchronization information")]
        [DefaultValue(128)]
        SYNCHRONIZE_READ,
        [Description("Manage test controllers")]
        [DefaultValue(512)]
        MANAGE_TEST_CONTROLLERS,
        [Description("Delete field from organization")]
        [DefaultValue(1024)]
        DELETE_FIELD,
        [Description("Manage enterprise policies")]
        [DefaultValue(2048)]
        MANAGE_ENTERPRISE_POLICIES
    }

    [Guid("cb4d56d2-e84b-457e-8845-81320a133fbb")]
    [Description("Proxy")]
    public enum Proxy
    {
        [Description("Read proxies")]
        [DefaultValue(1)]
        Read,
        [Description("Manage proxies")]
        [DefaultValue(2)]
        Manage
    }

    [Guid("bed337f8-e5f3-4fb9-80da-81e17d06e7a8")]
    [Description("Plan")]
    public enum Plan
    {
        [Description("View")]
        [DefaultValue(1)]
        View,
        [Description("Edit")]
        [DefaultValue(2)]
        Edit,
        [Description("Delete")]
        [DefaultValue(4)]
        Delete,
        [Description("Manage")]
        [DefaultValue(8)]
        Manage
    }

    [Guid("2dab47f9-bd70-49ed-9bd5-8eb051e59c02")]
    [Description("Process")]
    public enum Process
    {
        [Description("Edit process")]
        [DefaultValue(1)]
        Edit,
        [Description("Delete process")]
        [DefaultValue(2)]
        Delete,
        [Description("Create process")]
        [DefaultValue(4)]
        Create,
        [Description("Administer process permissions")]
        [DefaultValue(8)]
        AdministerProcessPermissions,
        [Description("View Process")]
        [DefaultValue(16)]
        ReadProcessPermissions
    }

    [Guid("11238e09-49f2-40c7-94d0-8f0307204ce4")]
    [Description("AccountAdminSecurity")]
    public enum AccountAdminSecurity
    {
        [Description("Read account resource")]
        [DefaultValue(1)]
        Read,
        [Description("Create account resource")]
        [DefaultValue(2)]
        Create,
        [Description("Modify account resource")]
        [DefaultValue(4)]
        Modify
    }

    [Guid("b7e84409-6553-448a-bbb2-af228e07cbeb")]
    [Description("Library")]
    public enum Library
    {
        [Description("View library item")]
        [DefaultValue(1)]
        View,
        [Description("Administer library item")]
        [DefaultValue(2)]
        Administer,
        [Description("Create library item")]
        [DefaultValue(4)]
        Create,
        [Description("View library item secrets")]
        [DefaultValue(8)]
        ViewSecrets,
        [Description("Use library item")]
        [DefaultValue(16)]
        Use,
        [Description("Owner library item")]
        [DefaultValue(32)]
        Owner
    }

    [Guid("83d4c2e6-e57d-4d6e-892b-b87222b7ad20")]
    [Description("Environment")]
    public enum Environment
    {
        [Description("View")]
        [DefaultValue(1)]
        View,
        [Description("Manage")]
        [DefaultValue(2)]
        Manage,
        [Description("Manage history")]
        [DefaultValue(4)]
        ManageHistory,
        [Description("Administer Permissions")]
        [DefaultValue(8)]
        Administer,
        [Description("Use")]
        [DefaultValue(16)]
        Use,
        [Description("Create")]
        [DefaultValue(32)]
        Create
    }

    [Guid("52d39943-cb85-4d7f-8fa8-c6baac873819")]
    [Description("Project")]
    public enum Project
    {
        [Description("View project-level information")]
        [DefaultValue(1)]
        GENERIC_READ,
        [Description("Edit project-level information")]
        [DefaultValue(2)]
        GENERIC_WRITE,
        [Description("Delete team project")]
        [DefaultValue(4)]
        DELETE,
        [Description("Create test runs")]
        [DefaultValue(8)]
        PUBLISH_TEST_RESULTS,
        [Description("Administer a build")]
        [DefaultValue(16)]
        ADMINISTER_BUILD,
        [Description("Start a build")]
        [DefaultValue(32)]
        START_BUILD,
        [Description("Edit build quality")]
        [DefaultValue(64)]
        EDIT_BUILD_STATUS,
        [Description("Write to build operational store")]
        [DefaultValue(128)]
        UPDATE_BUILD,
        [Description("Delete test runs")]
        [DefaultValue(256)]
        DELETE_TEST_RESULTS,
        [Description("View test runs")]
        [DefaultValue(512)]
        VIEW_TEST_RESULTS,
        [Description("Manage test environments")]
        [DefaultValue(2048)]
        MANAGE_TEST_ENVIRONMENTS,
        [Description("Manage test configurations")]
        [DefaultValue(4096)]
        MANAGE_TEST_CONFIGURATIONS,
        [Description("Delete and restore work items")]
        [DefaultValue(8192)]
        WORK_ITEM_DELETE,
        [Description("Move work items out of this project")]
        [DefaultValue(16384)]
        WORK_ITEM_MOVE,
        [Description("Permanently delete work items")]
        [DefaultValue(32768)]
        WORK_ITEM_PERMANENTLY_DELETE,
        [Description("Rename team project")]
        [DefaultValue(65536)]
        RENAME,
        [Description("Manage project properties")]
        [DefaultValue(131072)]
        MANAGE_PROPERTIES,
        [Description("Manage system project properties")]
        [DefaultValue(262144)]
        MANAGE_SYSTEM_PROPERTIES,
        [Description("Bypass project property cache")]
        [DefaultValue(524288)]
        BYPASS_PROPERTY_CACHE,
        [Description("Bypass rules on work item updates")]
        [DefaultValue(1048576)]
        BYPASS_RULES,
        [Description("Suppress notifications for work item updates")]
        [DefaultValue(2097152)]
        SUPPRESS_NOTIFICATIONS,
        [Description("Update project visibility")]
        [DefaultValue(4194304)]
        UPDATE_VISIBILITY,
        [Description("Change process of team project.")]
        [DefaultValue(8388608)]
        CHANGE_PROCESS,
        [Description("Agile backlog management.")]
        [DefaultValue(16777216)]
        AGILETOOLS_BACKLOG
    }

    [Guid("58b176e7-3411-457a-89d0-c6d0ccb3c52b")]
    [Description("EventSubscription")]
    public enum EventSubscription
    {
        [Description("View")]
        [DefaultValue(1)]
        GENERIC_READ,
        [Description("Edit")]
        [DefaultValue(2)]
        GENERIC_WRITE,
        [Description("Unsubscribe")]
        [DefaultValue(4)]
        UNSUBSCRIBE,
        [Description("Create a SOAP subscription")]
        [DefaultValue(8)]
        CREATE_SOAP_SUBSCRIPTION
    }

    [Guid("83e28ad4-2d72-4ceb-97b0-c7726d5502c3")]
    [Description("CSS")]
    public enum CSS
    {
        [Description("View permissions for this node")]
        [DefaultValue(1)]
        GENERIC_READ,
        [Description("Edit this node")]
        [DefaultValue(2)]
        GENERIC_WRITE,
        [Description("Create child nodes")]
        [DefaultValue(4)]
        CREATE_CHILDREN,
        [Description("Delete this node")]
        [DefaultValue(8)]
        DELETE,
        [Description("View work items in this node")]
        [DefaultValue(16)]
        WORK_ITEM_READ,
        [Description("Edit work items in this node")]
        [DefaultValue(32)]
        WORK_ITEM_WRITE,
        [Description("Manage test plans")]
        [DefaultValue(64)]
        MANAGE_TEST_PLANS,
        [Description("Manage test suites")]
        [DefaultValue(128)]
        MANAGE_TEST_SUITES
    }

    [Guid("9e4894c3-ff9a-4eac-8a85-ce11cafdc6f1")]
    [Description("TeamLabSecurity")]
    public enum TeamLabSecurity
    {
        [Description("Read")]
        [DefaultValue(1)]
        Read,
        [Description("Create")]
        [DefaultValue(2)]
        Create,
        [Description("Write")]
        [DefaultValue(4)]
        Write,
        [Description("Edit")]
        [DefaultValue(8)]
        Edit,
        [Description("Delete")]
        [DefaultValue(16)]
        Delete,
        [Description("Start")]
        [DefaultValue(32)]
        Start,
        [Description("Stop")]
        [DefaultValue(64)]
        Stop,
        [Description("Pause")]
        [DefaultValue(128)]
        Pause,
        [Description("ManageSnapshots")]
        [DefaultValue(256)]
        ManageSnapshots,
        [Description("ManageLocation")]
        [DefaultValue(512)]
        ManageLocation,
        [Description("DeleteLocation")]
        [DefaultValue(1024)]
        DeleteLocation,
        [Description("ManagePermissions")]
        [DefaultValue(2048)]
        ManagePermissions,
        [Description("ManageChildPermissions")]
        [DefaultValue(4096)]
        ManageChildPermissions,
        [Description("ManageTestMachines")]
        [DefaultValue(8192)]
        ManageTestMachines
    }

    [Guid("fc5b7b85-5d6b-41eb-8534-e128cb10eb67")]
    [Description("ProjectAnalysisLanguageMetrics")]
    public enum ProjectAnalysisLanguageMetrics
    {
        [Description("View Project Analysis language metrics data")]
        [DefaultValue(1)]
        Read,
        [Description("Write Project Analysis language metrics data")]
        [DefaultValue(2)]
        Write
    }

    [Guid("bb50f182-8e5e-40b8-bc21-e8752a1e7ae2")]
    [Description("Tagging")]
    public enum Tagging
    {
        [Description("Enumerate tag definitions")]
        [DefaultValue(1)]
        Enumerate,
        [Description("Create tag definition")]
        [DefaultValue(2)]
        Create,
        [Description("Update tag definition")]
        [DefaultValue(4)]
        Update,
        [Description("Delete tag definition")]
        [DefaultValue(8)]
        Delete
    }

    [Guid("f6a4de49-dbe2-4704-86dc-f8ec1a294436")]
    [Description("MetaTask")]
    public enum MetaTask
    {
        [Description("Administer task group permissions")]
        [DefaultValue(1)]
        Administer,
        [Description("Edit task group")]
        [DefaultValue(2)]
        Edit,
        [Description("Delete task group")]
        [DefaultValue(4)]
        Delete
    }

    [Guid("bf7bfa03-b2b7-47db-8113-fa2e002cc5b1")]
    [Description("Iteration")]
    public enum Iteration
    {
        [Description("View permissions for this node")]
        [DefaultValue(1)]
        GENERIC_READ,
        [Description("Edit this node")]
        [DefaultValue(2)]
        GENERIC_WRITE,
        [Description("Create child nodes")]
        [DefaultValue(4)]
        CREATE_CHILDREN,
        [Description("Delete this node")]
        [DefaultValue(8)]
        DELETE
    }

    [Guid("fa557b48-b5bf-458a-bb2b-1b680426fe8b")]
    [Description("")]
    public enum Favorites
    {
        [Description("View instance-level information")]
        [DefaultValue(1)]
        GenericRead,
        [Description("Edit instance-level information")]
        [DefaultValue(2)]
        GenericWrite
    }

    [Guid("4ae0db5d-8437-4ee8-a18b-1f6fb38bd34c")]
    [Description("")]
    public enum Registry
    {
        [Description("Read registry entries")]
        [DefaultValue(1)]
        Read,
        [Description("Write registry entries")]
        [DefaultValue(2)]
        Write
    }

    [Guid("c2ee56c9-e8fa-4cdd-9d48-2c44f697a58e")]
    [Description("")]
    public enum Graph
    {
        [Description("Read by public identifier")]
        [DefaultValue(1)]
        ReadByPublicIdentifier,
        [Description("Read by personal identifier")]
        [DefaultValue(2)]
        ReadByPersonalIdentifier
    }

    [Guid("dc02bf3d-cd48-46c3-8a41-345094ecc94b")]
    [Description("")]
    public enum ViewActivityPaneSecurity
    {
        [Description("View only entities")]
        [DefaultValue(1)]
        Read
    }

    [Guid("2a887f97-db68-4b7c-9ae3-5cebd7add999")]
    [Description("")]
    public enum Job
    {
        [Description("View background job information")]
        [DefaultValue(1)]
        Read,
        [Description("Queue background jobs")]
        [DefaultValue(2)]
        Queue,
        [Description("Manage background jobs")]
        [DefaultValue(4)]
        Update
    }

    [Guid("73e71c45-d483-40d5-bdba-62fd076f7f87")]
    [Description("")]
    public enum WorkItemTracking
    {
        [Description("Read WorkItemTracking")]
        [DefaultValue(1)]
        Read,
        [Description("Cross Project Read Of WorkItemTracking Resources")]
        [DefaultValue(2)]
        CrossProjectRead,
        [Description("Track work item read and write for a user")]
        [DefaultValue(4)]
        TrackWorkItemActivity,
        [Description("Read rules only if permissions are avaliable")]
        [DefaultValue(8)]
        ReadRules,
        [Description("")]
        [DefaultValue(16)]
        ReadHistoricalWorkItemResources
    }

    [Guid("4a9e8381-289a-4dfd-8460-69028eaa93b3")]
    [Description("")]
    public enum StrongBox
    {
        [Description("Create a StrongBox Drawer.")]
        [DefaultValue(1)]
        CreateDrawer,
        [Description("Delete as StrongBox Drawer.")]
        [DefaultValue(2)]
        DeleteDrawer,
        [Description("Administer StrongBox Permissions.")]
        [DefaultValue(4)]
        Administer,
        [Description("Add Items to the StrongBox Drawer.")]
        [DefaultValue(16)]
        AddItem,
        [Description("Retrieve Items from the StrongBox Drawer.")]
        [DefaultValue(32)]
        GetItem,
        [Description("Delete Items from the StrongBox Drawer.")]
        [DefaultValue(64)]
        DeleteItem,
        [Description("Administer permissions for the StrongBox Drawer.")]
        [DefaultValue(128)]
        AdministerDrawer
    }

    [Guid("1f4179b3-6bac-4d01-b421-71ea09171400")]
    [Description("")]
    public enum Server
    {
        [Description("View instance-level information")]
        [DefaultValue(1)]
        GenericRead,
        [Description("Edit instance-level information")]
        [DefaultValue(2)]
        GenericWrite,
        [Description("Make requests on behalf of others")]
        [DefaultValue(4)]
        Impersonate,
        [Description("Trigger events")]
        [DefaultValue(16)]
        TriggerEvent
    }

    [Guid("e06e1c24-e93d-4e4a-908a-7d951187b483")]
    [Description("")]
    public enum TestManagement
    {
        [Description("Read TestManagement")]
        [DefaultValue(1)]
        Read
    }

    [Guid("6ec4592e-048c-434e-8e6c-8671753a8418")]
    [Description("")]
    public enum SettingEntries
    {
        [Description("Retrieve setting entries")]
        [DefaultValue(1)]
        Read,
        [Description("Write setting entries")]
        [DefaultValue(2)]
        Write
    }

    [Guid("302acaca-b667-436d-a946-87133492041c")]
    [Description("")]
    public enum BuildAdministration
    {
        [Description("View build resources")]
        [DefaultValue(1)]
        ViewBuildResources,
        [Description("Manage build resources")]
        [DefaultValue(2)]
        ManageBuildResources,
        [Description("Use build resources")]
        [DefaultValue(4)]
        UseBuildResources,
        [Description("Administer build resource permissions")]
        [DefaultValue(8)]
        AdministerBuildResourcePermissions
    }

    [Guid("2725d2bc-7520-4af4-b0e3-8d876494731f")]
    [Description("")]
    public enum Location
    {
        [Description("Read service definitions and/or access mappings.")]
        [DefaultValue(1)]
        Read,
        [Description("Write service definitions and/or access mappings.")]
        [DefaultValue(2)]
        Write
    }

    [Guid("83abde3a-4593-424e-b45f-9898af99034d")]
    [Description("")]
    public enum UtilizationPermissions
    {
        [Description("Query Others' Usage")]
        [DefaultValue(1)]
        QueryUsageSummary
    }

    [Guid("c0e7a722-1cad-4ae6-b340-a8467501e7ce")]
    [Description("")]
    public enum WorkItemsHub
    {
        [Description("View work items hub")]
        [DefaultValue(1)]
        View
    }

    [Guid("0582eb05-c896-449a-b933-aa3d99e121d6")]
    [Description("")]
    public enum WebPlatform
    {
        [Description("View web platform entities")]
        [DefaultValue(1)]
        Read
    }

    [Guid("66312704-deb5-43f9-b51c-ab4ff5e351c3")]
    [Description("")]
    public enum VersionControlPrivileges
    {
        [Description("Create a workspace")]
        [DefaultValue(2)]
        CreateWorkspace,
        [Description("Administer workspaces")]
        [DefaultValue(4)]
        AdminWorkspaces,
        [Description("Administer shelved changes")]
        [DefaultValue(8)]
        AdminShelvesets,
        [Description("Administer source control connections")]
        [DefaultValue(16)]
        AdminConnections,
        [Description("Administer source control configurations")]
        [DefaultValue(32)]
        AdminConfiguration
    }

    [Guid("93bafc04-9075-403a-9367-b7164eac6b5c")]
    [Description("")]
    public enum Workspaces
    {
        [Description("View workspace information")]
        [DefaultValue(1)]
        Read,
        [Description("Use the workspace")]
        [DefaultValue(2)]
        Use,
        [Description("Check in changes to the workspace")]
        [DefaultValue(4)]
        Checkin,
        [Description("Administer the workspace")]
        [DefaultValue(8)]
        Administer
    }

    [Guid("093cbb02-722b-4ad6-9f88-bc452043fa63")]
    [Description("")]
    public enum CrossProjectWidgetView
    {
        [Description("View instance-level information")]
        [DefaultValue(1)]
        GenericRead
    }

    [Guid("35e35e8e-686d-4b01-aff6-c369d6e36ce0")]
    [Description("")]
    public enum WorkItemTrackingConfiguration
    {
        [Description("View work item tracking configuration")]
        [DefaultValue(1)]
        Read
    }

    [Guid("0d140cae-8ac1-4f48-b6d1-c93ce0301a12")]
    [Description("")]
    public enum DiscussionThreads
    {
        [Description("Manage discussion permissions")]
        [DefaultValue(1)]
        Administer,
        [Description("View discussions")]
        [DefaultValue(2)]
        GenericRead,
        [Description("Contribute to discussions")]
        [DefaultValue(4)]
        GenericContribute,
        [Description("Moderate discussions")]
        [DefaultValue(8)]
        Moderate
    }

    [Guid("5ab15bc8-4ea1-d0f3-8344-cab8fe976877")]
    [Description("")]
    public enum BoardsExternalIntegration
    {
        [Description("View boards external integrations")]
        [DefaultValue(1)]
        Read,
        [Description("Write boards external integrations")]
        [DefaultValue(2)]
        Write
    }

    [Guid("7ffa7cf4-317c-4fea-8f1d-cfda50cfa956")]
    [Description("")]
    public enum DataProvider
    {
        [Description("View data provider entities")]
        [DefaultValue(1)]
        Read
    }

    [Guid("81c27cc8-7a9f-48ee-b63f-df1e1d0412dd")]
    [Description("")]
    public enum Social
    {
        [Description("View instance-level information")]
        [DefaultValue(1)]
        GenericRead,
        [Description("Edit instance-level information")]
        [DefaultValue(2)]
        GenericWrite
    }

    [Guid("9a82c708-bfbe-4f31-984c-e860c2196781")]
    [Description("")]
    public enum Security
    {
        [Description("Read")]
        [DefaultValue(1)]
        Read
    }

    [Guid("a60e0d84-c2f8-48e4-9c0c-f32da48d5fd1")]
    [Description("")]
    public enum IdentityPicker
    {
        [Description("Read basic Identity Picker properties")]
        [DefaultValue(1)]
        ReadBasic,
        [Description("Read restricted Identity Picker properties")]
        [DefaultValue(2)]
        ReadRestricted
    }

    [Guid("84cc1aa4-15bc-423d-90d9-f97c450fc729")]
    [Description("")]
    public enum ServicingOrchestration
    {
        [Description("View Servicing Orchestration information")]
        [DefaultValue(1)]
        Read,
        [Description("Queue Servicing Orchestration jobs")]
        [DefaultValue(2)]
        Queue,
        [Description("Cancel Servicing Orchestration jobs")]
        [DefaultValue(4)]
        Cancel
    }

    [Guid("33344d9c-fc72-4d6f-aba5-fa317101a7e9")]
    [Description("")]
    public enum Build
    {
        [Description("View builds")]
        [DefaultValue(1)]
        ViewBuilds,
        [Description("Edit build quality")]
        [DefaultValue(2)]
        EditBuildQuality,
        [Description("Retain indefinitely")]
        [DefaultValue(4)]
        RetainIndefinitely,
        [Description("Delete builds ")]
        [DefaultValue(8)]
        DeleteBuilds,
        [Description("Manage build qualities")]
        [DefaultValue(16)]
        ManageBuildQualities,
        [Description("Destroy builds")]
        [DefaultValue(32)]
        DestroyBuilds,
        [Description("Update build information")]
        [DefaultValue(64)]
        UpdateBuildInformation,
        [Description("Queue builds")]
        [DefaultValue(128)]
        QueueBuilds,
        [Description("Manage build queue")]
        [DefaultValue(256)]
        ManageBuildQueue,
        [Description("Stop builds")]
        [DefaultValue(512)]
        StopBuilds,
        [Description("View build pipeline")]
        [DefaultValue(1024)]
        ViewBuildDefinition,
        [Description("Edit build pipeline")]
        [DefaultValue(2048)]
        EditBuildDefinition,
        [Description("Delete build pipeline")]
        [DefaultValue(4096)]
        DeleteBuildDefinition,
        [Description("Override check-in validation by build")]
        [DefaultValue(8192)]
        OverrideBuildCheckInValidation,
        [Description("Administer build permissions")]
        [DefaultValue(16384)]
        AdministerBuildPermissions
    }

    [Guid("8adf73b7-389a-4276-b638-fe1653f7efc7")]
    [Description("")]
    public enum DashboardsPrivileges
    {
        [Description("Read")]
        [DefaultValue(1)]
        Read,
        [Description("Create dashboard")]
        [DefaultValue(2)]
        Create,
        [Description("Edit dashboard")]
        [DefaultValue(4)]
        Edit,
        [Description("Delete dashboard")]
        [DefaultValue(8)]
        Delete,
        [Description("ManagePermissions")]
        [DefaultValue(16)]
        ManagePermissions,
        [Description("Materialize Dashboards")]
        [DefaultValue(32)]
        MaterializeDashboards
    }

    [Guid("a39371cf-0841-4c16-bbd3-276e341bc052")]
    [Description("VersionControlItems")]
    public enum VersionControlItems
    {
        [Description("Read")]
        [DefaultValue(1)]
        Read,
        [Description("Pend a change in a server workspace")]
        [DefaultValue(2)]
        PendChange,
        [Description("Check in")]
        [DefaultValue(4)]
        Checkin,
        [Description("Label")]
        [DefaultValue(8)]
        Label,
        [Description("Lock")]
        [DefaultValue(16)]
        Lock,
        [Description("Revise other users' changes")]
        [DefaultValue(32)]
        ReviseOther,
        [Description("Unlock other users' changes")]
        [DefaultValue(64)]
        UnlockOther,
        [Description("Undo other users' changes")]
        [DefaultValue(128)]
        UndoOther,
        [Description("Administer labels")]
        [DefaultValue(256)]
        LabelOther,
        [Description("Manage permissions")]
        [DefaultValue(1024)]
        AdminProjectRights,
        [Description("Check in other users' changes")]
        [DefaultValue(2048)]
        CheckinOther,
        [Description("Merge")]
        [DefaultValue(4096)]
        Merge,
        [Description("Manage branch")]
        [DefaultValue(8192)]
        ManageBranch
    }

}
