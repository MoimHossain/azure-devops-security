using Cielo.Azdo.Abstract;
using Cielo.Azdo.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Azdo
{
    public class SecurityNamespaceService : RestServiceBase
    {
        public enum SecurityNamespaceConstants
        {
            [Description("CSS")]
            Classifications,
            [Description("Git Repositories")]
            Git_Repositories,
            [Description("ReleaseManagement")]
            ReleaseManagement,
            [Description("Build")]
            Build,
            [Description("Identity")]
            Identity
        }

        public SecurityNamespaceService(IHttpClientFactory clientFactory) : base(clientFactory)
        {

        }

        public async Task<VstsSecurityNamespace> GetNamespaceAsync(SecurityNamespaceConstants securityNamespace)
        {
            var sv = securityNamespace.GetStringValue();
            var all = await GetAllNamespacesAsync();
            var ns = all.Value.FirstOrDefault(sn => sn.Name.Equals(sv, StringComparison.OrdinalIgnoreCase));

            return ns;
        }


        public async Task<VstsSecurityNamespace> GetNamespaceAsync(
            SecurityNamespaceConstants securityNamespace, string action)
        {
            var sv = securityNamespace.GetStringValue();
            var all = await GetAllNamespacesAsync();
            var ns = all.Value.FirstOrDefault(sn => sn.Name.Equals(sv, StringComparison.OrdinalIgnoreCase)
                        && sn.Actions.Any(ac => ac.Name.Equals(action, StringComparison.OrdinalIgnoreCase)));
            return ns;
        }



        public async Task<VstsSecurityNamespaceCollection> GetAllNamespacesAsync()
        {
            var path = "_apis/securitynamespaces?api-version=6.0";
            var namespaces = await CoreApi()
                .GetRestAsync<VstsSecurityNamespaceCollection>(path);

            return namespaces;
        }

        public async Task<string> GenerateEnumerationsAsync(string codeNamespace)
        {
            var sb = new StringBuilder();
            var all = await GetAllNamespacesAsync();

            var distincts = new List<string>();
            var safeName = new Func<string, string>(name =>
            {
                var alterName = $"{name.Replace(" ", string.Empty)}{(distincts.Contains(name) ? "Ex" : string.Empty)}";
                distincts.Add(name);
                return alterName;
            });

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.ComponentModel;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine("using System.Runtime.InteropServices;");
            sb.AppendLine();
            sb.AppendLine($"namespace {codeNamespace}");
            sb.AppendLine("{");

            foreach (var item in all.Value)
            {
                sb.AppendLine($"\t[Guid(\"{item.NamespaceId}\")]");
                sb.AppendLine($"\t[Description(\"{item.DisplayName}\")]");
                sb.AppendLine($"\tpublic enum {safeName(item.Name)}");
                sb.AppendLine("\t{");
                for (var i = 0; i < item.Actions.Length; ++i)
                {
                    var action = item.Actions[i];
                    sb.AppendLine($"\t\t[Description(\"{action.DisplayName}\")]");
                    sb.AppendLine($"\t\t[DefaultValue({action.Bit})]");
                    sb.AppendLine($"\t\t{action.Name}{(i + 1 == item.Actions.Length ? string.Empty : ",")}");
                }
                sb.AppendLine("\t}");
                sb.AppendLine();
            }
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
