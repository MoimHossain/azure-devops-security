using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Waddle
{
    public abstract class RestServiceBase
    {
        private readonly string pat;
        private readonly string adoUrl;

        public RestServiceBase(string uri, string pat)
        {
            this.adoUrl = uri;
            this.pat = pat;
        }

        #region Helper methods

        protected string GetOrganizationName()
        {
            return GetAzureDevOpsDefaultUri().AbsolutePath.Replace("/", string.Empty);
        }

        protected Uri GetAzureDevOpsVsspUri()
        {
            var organizationName = GetOrganizationName();
            return new Uri($"https://vssps.dev.azure.com/{organizationName}/");
        }

        protected Uri GetAzureDevOpsVsrmUri()
        {
            var organizationName = GetOrganizationName();
            return new Uri($"https://vsrm.dev.azure.com/{organizationName}/");
        }

        protected Uri GetAzureDevOpsDefaultUri()
        {
            return new Uri(this.adoUrl);
        }

        protected async Task<Action<HttpClient>> GetBearerTokenAsync()
        {
            await Task.Delay(0);
            return new Action<HttpClient>((httpClient) =>
            {
                var credentials =
                Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(
                    string.Format("{0}:{1}", "", this.pat)));
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            });
        }
        #endregion
    }
}
