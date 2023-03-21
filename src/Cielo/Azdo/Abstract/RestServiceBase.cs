using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Azdo.Abstract
{
    public abstract class RestServiceBase
    {
        private readonly IHttpClientFactory clientFactory;

        protected IHttpClientFactory ClientFactory { get { return clientFactory; } }


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
