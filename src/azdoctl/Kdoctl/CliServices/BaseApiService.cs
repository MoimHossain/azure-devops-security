﻿

using Kdoctl.CliServices;
using System.Threading.Tasks;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Kdoctl.Schema.CliServices
{
    public abstract class BaseApiService
    {
        protected BaseApiService(string orgUri, string pat)
        {
            this.deserializer = (Deserializer)new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();
            this.Factory = new AdoConnectionFactory(orgUri, pat);
            this.Logger = new ConsoleLogger();
        }

        protected ConsoleLogger Logger { get; }
        protected TPayload Deserialize<TPayload>(string content)
        {
            return deserializer.Deserialize<TPayload>(content);
        }

        public async Task ExecuteAsync(BaseSchema baseSchema, string manifest)
        {
            await ExecuteCoreAsync(baseSchema, manifest);
        }

        protected abstract Task ExecuteCoreAsync(BaseSchema baseSchema, string manifest);

        protected AdoConnectionFactory Factory
        {
            get;
        }

        private readonly Deserializer deserializer;
    }
}
