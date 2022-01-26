


using Kdoctl.CliServices.AzDoServices;
using Kdoctl.CliServices.Supports;
using Kdoctl.Schema;
using Kdoctl.Schema.CliServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Kdoctl.CliServices
{
    public abstract class TaskBase
    {
        protected TaskBase(IServiceProvider services)
        {
            this.deserializer = (Deserializer)new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();
            this.serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithEmissionPhaseObjectGraphVisitor(args => new YamlIEnumerableSkipEmptyObjectGraphVisitor(args.InnerVisitor))
                .Build();            
            this.Logger = new ConsoleLogger();
            this.services = services ?? throw new ArgumentNullException(nameof(services));
        }

        protected ProjectService GetProjectService()
        {
            return services.GetRequiredService<ProjectService>();
        }

        protected WorkItemService GetWorkItemService()
        {
            return services.GetRequiredService<WorkItemService>();
        }

        protected RepositoryService GetRepositoryService()
        {
            return services.GetRequiredService<RepositoryService>();
        }

        protected GraphService GetGraphService()
        {
            return services.GetRequiredService<GraphService>();
        }

        protected BuildService GetBuildService()
        {
            return services.GetRequiredService<BuildService>();
        }

        protected ReleaseService GetReleaseService()
        {
            return services.GetRequiredService<ReleaseService>();
        }

        
        protected SecurityNamespaceService GetSecurityNamespaceService()
        {
            return services.GetRequiredService<SecurityNamespaceService>();
        }

        protected AclListService GetAclListService()
        {
            return services.GetRequiredService<AclListService>();
        }

        protected ServiceEndpointService GetServiceEndpointService()
        {
            return services.GetRequiredService<ServiceEndpointService>();
        }

        protected PipelineEnvironmentService GetPipelineEnvironmentService()
        {
            return services.GetRequiredService<PipelineEnvironmentService>();
        }        

        protected ConsoleLogger Logger { get; }
        protected TPayload Deserialize<TPayload>(string content)
        {
            return deserializer.Deserialize<TPayload>(content);
        }

        protected string Serialize(object graph)
        {
            return serializer.Serialize(graph);
        }

        public async Task ExecuteAsync()
        {
            var exponentialBackoffFactor = 5000;
            var retryCount = 1;
            await ExecutionSupports.Retry(async () =>
            {
                await ExecuteCoreAsync();
            },
            exception => { Logger.Error(exception.Message); }, exponentialBackoffFactor, retryCount);
        }

        public async Task ExecuteAsync(BaseSchema baseSchema, string manifest, string filePath)
        {
            var exponentialBackoffFactor = 5000;
            var retryCount = 1;
            await ExecutionSupports.Retry(async () =>
            {
                await ExecuteCoreAsync(baseSchema, manifest, filePath);
            },
            exception => { Logger.Error(exception.Message); }, exponentialBackoffFactor, retryCount);
        }

        protected abstract Task ExecuteCoreAsync();

        protected abstract Task ExecuteCoreAsync(BaseSchema baseSchema, string manifest, string filePath);


        private readonly Deserializer deserializer;
        private readonly ISerializer serializer;
        private readonly IServiceProvider services;
    }
}
