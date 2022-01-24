using Kdoctl.CliServices.AzDoServices.LowLevels;
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
        private readonly IHttpClientFactory clientFactory;


        public RestServiceBase(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }



        //protected string GetOrganizationName()
        //{
        //    return CoreApi().AbsolutePath.Replace("/", string.Empty);
        //}

        protected HttpClient VsaexApi()
        {
            return clientFactory.CreateClient(AzDOHttpSupports.API.VSAEX);
        }

        protected HttpClient VsspsApi()
        {
            return clientFactory.CreateClient(AzDOHttpSupports.API.VSSPS);
        }

        protected HttpClient VsrmApi()
        {
            return clientFactory.CreateClient(AzDOHttpSupports.API.VSRM);
        }

        protected HttpClient CoreApi()
        {
            return clientFactory.CreateClient(AzDOHttpSupports.API.CORE);
        }
    }
}
