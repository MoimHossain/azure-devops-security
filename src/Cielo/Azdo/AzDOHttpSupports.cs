using Cielo.CliSupports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Azdo
{
    public static class AzDOHttpSupports
    {
        public class API
        {
            public const string CORE = "CORE";
            public const string VSAEX = "vsaex";
            public const string VSSPS = "vssps";
            public const string VSRM = "vsrm";
        }

        private static string GetOrgName(string uri)
        {
            return new Uri(uri).AbsolutePath.Replace("/", string.Empty);
        }

        private static void ConfigureClient(string apiType, string orgUri, string pat, HttpClient httpClient)
        {
            var credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", string.Empty, pat)));

            httpClient.BaseAddress = apiType.Equals(API.CORE) ? new Uri(orgUri) : new Uri($"https://{apiType}.dev.azure.com/{GetOrgName(orgUri)}/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        }

        public static void AddServicesFromClientLib(this IServiceCollection services, OptionBase opts)
        {
            var connection = new VssConnection(new Uri($"{opts.OrganizationURL}"), new VssBasicCredential(string.Empty, opts.PAT));

            services.AddSingleton(connection);
            services.AddSingleton(connection.GetClient<TeamHttpClient>());
            services.AddSingleton(connection.GetClient<WorkItemTrackingHttpClient>());
            services.AddSingleton(connection.GetClient<GitHttpClient>());
        }

        public static void AddHttpClients(this IServiceCollection services, OptionBase opts)
        {
            var orgUri = $"{opts.OrganizationURL}";
            var pat = $"{opts.PAT}";
            services.AddHttpClient(API.CORE, httpClient => ConfigureClient(API.CORE, orgUri, pat, httpClient));
            services.AddHttpClient(API.VSAEX, httpClient => ConfigureClient(API.VSAEX, orgUri, pat, httpClient));
            services.AddHttpClient(API.VSSPS, httpClient => ConfigureClient(API.VSSPS, orgUri, pat, httpClient));
            services.AddHttpClient(API.VSRM, httpClient => ConfigureClient(API.VSRM, orgUri, pat, httpClient));
        }

        public static void AddAzdoServices(this IServiceCollection services)
        {
            services.AddTransient<AclService>();
            //services.AddTransient<BuildService>();
            //services.AddTransient<ClassificationService>();
            services.AddTransient<GraphService>();
            //services.AddTransient<PipelineEnvironmentService>();
            services.AddTransient<ProjectService>();
            services.AddTransient<TeamService>();
            
            //services.AddTransient<ReleaseService>();
            //services.AddTransient<RepositoryService>();
            services.AddTransient<SecurityNamespaceService>();
            //services.AddTransient<ServiceEndpointService>();
            //services.AddTransient<WorkItemService>();
        }
    }
}
