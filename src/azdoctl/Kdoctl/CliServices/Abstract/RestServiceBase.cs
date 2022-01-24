using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.Abstract
{
    public abstract class RestServiceBase
    {
        private readonly IServiceProvider services;
        

        public RestServiceBase(IServiceProvider services)
        {
            this.services = services;
        }

        //#region Helper methods

        //protected string GetOrganizationName()
        //{
        //    return CoreApi().AbsolutePath.Replace("/", string.Empty);
        //}

        //protected Uri VsaexApi()
        //{
        //    var organizationName = GetOrganizationName();
        //    return new Uri($"https://vsaex.dev.azure.com/{organizationName}/");
        //}

        //protected Uri VsspsApi()
        //{
        //    var organizationName = GetOrganizationName();
        //    return new Uri($"https://vssps.dev.azure.com/{organizationName}/");
        //}

        //protected Uri VsrmApi()
        //{
            
        //    services.GetRequiredService
        //    var organizationName = GetOrganizationName();
        //    return new Uri($"https://vsrm.dev.azure.com/{organizationName}/");
        //}

        //protected Uri CoreApi()
        //{
        //    return new Uri(this.adoUrl);
        //}

        ////protected async Task<Action<HttpClient>> GetBearerTokenAsync()
        ////{
        ////    await Task.Delay(0);
        ////    return new Action<HttpClient>((httpClient) =>
        ////    {
        ////        var credentials =
        ////        Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(
        ////            string.Format("{0}:{1}", "", this.pat)));
        ////        httpClient.DefaultRequestHeaders.Accept.Clear();
        ////        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        ////        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        ////    });
        ////}
        //#endregion
    }
}
