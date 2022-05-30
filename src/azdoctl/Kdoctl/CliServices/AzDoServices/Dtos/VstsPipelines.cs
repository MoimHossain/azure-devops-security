

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.AzDoServices.Dtos
{
    public class VstsPipelineCollection
    {
        public int count { get; set; }
        public List<VstsPipeline> value { get; set; }
    }

    public class VstsPipeline
    {
        public string url { get; set; }
        public int id { get; set; }
        public int revision { get; set; }
        public string name { get; set; }
        public string folder { get; set; }
    }


    
    public class VstsPipelineAuthor
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("uniqueName")]
        public string UniqueName { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("descriptor")]
        public string Descriptor { get; set; }
    }

   


   

    public class VstsBuildDefinitionCollection
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public List<VstsBuildDefinition> Value { get; set; }
    }


 

    public class VstsBuildDefinitionExecutionOptions
    {
        [JsonProperty("type")]
        public int Type { get; set; }
    }

    public class VstsBuildDefinitionInputs
    {
        [JsonProperty("branchFilters")]
        public string BranchFilters { get; set; }

        [JsonProperty("additionalFields")]
        public string AdditionalFields { get; set; }

        [JsonProperty("workItemType")]
        public string WorkItemType { get; set; }

        [JsonProperty("assignToRequestor")]
        public string AssignToRequestor { get; set; }

        [JsonProperty("targetType")]
        public string TargetType { get; set; }

        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("arguments")]
        public string Arguments { get; set; }

        [JsonProperty("script")]
        public string Script { get; set; }

        [JsonProperty("workingDirectory")]
        public string WorkingDirectory { get; set; }

        [JsonProperty("failOnStderr")]
        public string FailOnStderr { get; set; }

        [JsonProperty("bashEnvValue")]
        public string BashEnvValue { get; set; }
    }

    public class VstsBuildDefinitionOption
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }


        [JsonProperty("inputs")]
        public VstsBuildDefinitionInputs Inputs { get; set; }
    }

    public class VstsBuildDefinitionPhase
    {
        [JsonProperty("steps")]
        public List<VstsBuildDefinitionStep> Steps { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("refName")]
        public string RefName { get; set; }

        [JsonProperty("condition")]
        public string Condition { get; set; }

        [JsonProperty("jobAuthorizationScope")]
        public string JobAuthorizationScope { get; set; }
    }

    public class VstsBuildDefinitionProcess
    {
        [JsonProperty("phases")]
        public List<VstsBuildDefinitionPhase> Phases { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("yamlFilename")]
        public string YamlFilename { get; set; }
    }

    public class VstsBuildDefinitionProperties
    {
        [JsonProperty("cleanOptions")]
        public string CleanOptions { get; set; }

        [JsonProperty("labelSources")]
        public string LabelSources { get; set; }

        [JsonProperty("labelSourcesFormat")]
        public string LabelSourcesFormat { get; set; }

        [JsonProperty("reportBuildStatus")]
        public string ReportBuildStatus { get; set; }

        [JsonProperty("gitLfsSupport")]
        public string GitLfsSupport { get; set; }

        [JsonProperty("skipSyncSource")]
        public string SkipSyncSource { get; set; }

        [JsonProperty("checkoutNestedSubmodules")]
        public string CheckoutNestedSubmodules { get; set; }

        [JsonProperty("fetchDepth")]
        public string FetchDepth { get; set; }

        [JsonProperty("cloneUrl")]
        public string CloneUrl { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("defaultBranch")]
        public string DefaultBranch { get; set; }

        [JsonProperty("isFork")]
        public string IsFork { get; set; }

        [JsonProperty("safeRepository")]
        public string SafeRepository { get; set; }
    }


    public class VstsPipelineRepositoryRef
    {
        [JsonProperty("properties")]
        public VstsBuildDefinitionProperties Properties { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("defaultBranch")]
        public string DefaultBranch { get; set; }

        [JsonProperty("clean")]
        public object Clean { get; set; }

        [JsonProperty("checkoutSubmodules")]
        public bool CheckoutSubmodules { get; set; }
    }

    public class VstsBuildDefinition
    {
        [JsonProperty("properties")]
        public VstsBuildDefinitionProperties Properties { get; set; }

        [JsonProperty("tags")]
        public List<object> Tags { get; set; }

        [JsonProperty("jobAuthorizationScope")]
        public string JobAuthorizationScope { get; set; }

        [JsonProperty("jobTimeoutInMinutes")]
        public int JobTimeoutInMinutes { get; set; }

        [JsonProperty("jobCancelTimeoutInMinutes")]
        public int JobCancelTimeoutInMinutes { get; set; }

        [JsonProperty("quality")]
        public string Quality { get; set; }

        [JsonProperty("drafts")]
        public List<object> Drafts { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("queueStatus")]
        public string QueueStatus { get; set; }

        [JsonProperty("revision")]
        public int Revision { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("options")]
        public List<VstsBuildDefinitionOption> Options { get; set; }


        [JsonProperty("process")]
        public VstsBuildDefinitionProcess Process { get; set; }

        [JsonProperty("repository")]
        public VstsPipelineRepositoryRef Repository { get; set; }

        [JsonProperty("authoredBy")]
        public VstsPipelineAuthor AuthoredBy { get; set; }


        [JsonProperty("project")]
        public Project Project { get; set; }
    }

    public class VstsBuildDefinitionStep
    {

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("continueOnError")]
        public bool ContinueOnError { get; set; }

        [JsonProperty("alwaysRun")]
        public bool AlwaysRun { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("timeoutInMinutes")]
        public int TimeoutInMinutes { get; set; }

        [JsonProperty("retryCountOnTaskFailure")]
        public int RetryCountOnTaskFailure { get; set; }

        [JsonProperty("condition")]
        public string Condition { get; set; }

        [JsonProperty("task")]
        public VstsPipelineTask Task { get; set; }

        [JsonProperty("inputs")]
        public VstsBuildDefinitionInputs Inputs { get; set; }
    }

    public class VstsPipelineTask
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("versionSpec")]
        public string VersionSpec { get; set; }

        [JsonProperty("definitionType")]
        public string DefinitionType { get; set; }

        public override string ToString()
        {
            return $"{Id} - ({DefinitionType})";
        }
    }


}
