﻿

using Kdoctl.CliOptions;
using Kdoctl.CliServices.K8sServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

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

        public static void AddServicesFromClientLib(this IServiceCollection services, OptionBase opts)
        {
            var connection = new VssConnection(new Uri(opts.OrganizationURL), new VssBasicCredential(string.Empty, opts.PAT));

            services.AddSingleton(connection);
            services.AddSingleton(connection.GetClient<TeamHttpClient>());
            services.AddSingleton(connection.GetClient<WorkItemTrackingHttpClient>());
            services.AddSingleton(connection.GetClient<GitHttpClient>());               
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
            services.AddSingleton<K8sService>();

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
            services.AddTransient<WorkItemService>();
        }
    }
}
