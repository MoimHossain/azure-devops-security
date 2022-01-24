


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using Kdoctl;
using Kdoctl.CliOptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http.Headers;
using System.Net.Http;

namespace Kdoctl.CliServices.AzDoServices.LowLevels
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

        public static void AddHttpClients(this IServiceCollection services, OptionBase opts)
        {
            services.AddHttpClient(API.CORE, httpClient => ConfigureClient(API.CORE, opts.OrganizationURL, opts.PAT, httpClient));
            services.AddHttpClient(API.VSAEX, httpClient => ConfigureClient(API.VSAEX, opts.OrganizationURL, opts.PAT, httpClient));
            services.AddHttpClient(API.VSSPS, httpClient => ConfigureClient(API.VSSPS, opts.OrganizationURL, opts.PAT, httpClient));
            services.AddHttpClient(API.VSRM, httpClient => ConfigureClient(API.VSRM, opts.OrganizationURL, opts.PAT, httpClient));
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<AclListService>();
            services.AddTransient<BuildService>();
            services.AddTransient<ClassificationService>();
            services.AddTransient<GraphService>();
            services.AddTransient<PipelineEnvironmentService>();
            services.AddTransient<ProjectService>();
            services.AddTransient<ReleaseService>();
            services.AddTransient<RepositoryService>();
            services.AddTransient<SecurityNamespaceService>();
            services.AddTransient<ServiceEndpointService>();
        }
    }
}
