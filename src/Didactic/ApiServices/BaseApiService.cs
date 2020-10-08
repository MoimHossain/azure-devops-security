

using Didactic.Schema;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Threading.Tasks;
using Waddle;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Didactic.ApiServices
{
    public abstract class BaseApiService
    {
        protected BaseApiService(string orgUri, string pat)
        {
            this.deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
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
