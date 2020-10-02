using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Waddle
{
    public class AdoConnectionFactory
    {
        private readonly Uri orgUrl;
        private readonly string pat;
        private readonly VssConnection connection;

        public AdoConnectionFactory(Uri orgUrl, string pat)
        {
            this.connection = new VssConnection(orgUrl, new VssBasicCredential(string.Empty, pat));
            this.orgUrl = orgUrl;
            this.pat = pat;
        }

        public RepositoryService GetRepositoryService()
        {
            var gitClient = connection.GetClient<GitHttpClient>();

            return new RepositoryService(gitClient);
        }

        public SecurityNamespaceService GetSecurityNamespaceService()
        {
            return new SecurityNamespaceService(this.orgUrl.ToString(), this.pat);
        }

        public AclListService GetAclListService()
        {
            return new AclListService(this.orgUrl.ToString(), this.pat);
        }
    }
}
