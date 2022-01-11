

using Kdoctl.CliServices;
using Kdoctl.CliServices.Supports;
using Kdoctl.Schema;
using Kdoctl.Schema.CliServices;
using System.Threading.Tasks;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Kdoctl.CliServices
{
    public abstract class TaskBase
    {
        protected TaskBase(string orgUri, string pat)
        {
            this.deserializer = (Deserializer)new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();
            this.serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithEmissionPhaseObjectGraphVisitor(args => new YamlIEnumerableSkipEmptyObjectGraphVisitor(args.InnerVisitor))
                .Build();
            this.Factory = new AdoConnectionFactory(orgUri, pat);
            this.Logger = new ConsoleLogger();
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

        protected AdoConnectionFactory Factory
        {
            get;
        }

        private readonly Deserializer deserializer;
        private readonly ISerializer serializer;
    }
}
