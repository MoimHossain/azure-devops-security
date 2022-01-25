

using Kdoctl.CliServices.AzDoServices.LowLevels;
using System.Net.Http;

namespace Kdoctl.CliServices.Abstract
{
    public abstract class RestServiceBase
    {
        private readonly IHttpClientFactory clientFactory;


        public RestServiceBase(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

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
