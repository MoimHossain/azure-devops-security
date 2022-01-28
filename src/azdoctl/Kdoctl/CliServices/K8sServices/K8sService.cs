

using k8s;
using k8s.Models;
using Kdoctl.CliServices.Supports.Instrumentations;
using Kdoctl.Schema;
using Kdoctl.Schema.CliServices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.K8sServices
{
    public class K8sService
    {
        private readonly Kubernetes k8s;
        private readonly InstrumentationClient Logger;

        public K8sService(InstrumentationClient instrumentationClient)
        {
            var configFilePath = System.Environment.GetEnvironmentVariable("K8S_CONFIG_FILEPATH");
            if (!string.IsNullOrWhiteSpace(configFilePath))
            {
                k8s = new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile(configFilePath));
            }
            else
            {
                k8s = new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile());
            }

            this.Logger = instrumentationClient;
        }
        public async Task EnsureNamespaceExistsAsync(
            KubernetesEndpointManifest clusterInfo)
        {
            using var operation = Logger.Begin($"Checking Kubernetes namespace '{clusterInfo.Namespace.Metadata.Name}' ...", "K8SNamespace");
            var found = await GetNamespaceAsync(clusterInfo);
            if (found == null)
            {   
                await k8s.CreateNamespaceAsync(clusterInfo.Namespace);
                operation.EndWithSuccess();
            }
            else
            {
                using var subOp = Logger.Begin($"Updating Kubernetes namespace '{clusterInfo.Namespace.Metadata.Name}' ...");                
                await k8s.ReplaceNamespaceAsync(clusterInfo.Namespace, clusterInfo.Namespace.Metadata.Name);
                subOp.EndWithSuccess("Namespace updated");
            }
        }

        public Uri GetClusterAPIUri()
        {
            return k8s.BaseUri;
        }

        public async Task<V1Secret> GetSecretAsync(k8s.Models.V1ServiceAccount account)
        {
            if(account != null && account.Secrets != null && account.Secrets.Any())
            {
                var secretName = account.Secrets[0].Name;
                var secret = await k8s.ReadNamespacedSecretAsync(secretName, account.Metadata.NamespaceProperty);
                return secret;
            }
            return null;
        }

        public async Task<k8s.Models.V1ServiceAccount> EnsureServiceAccountExists(            KubernetesEndpointManifest clusterInfo)
        {
            clusterInfo.ServiceAccount.Spec.Metadata.NamespaceProperty = clusterInfo.Namespace.Metadata.Name;
            clusterInfo.ServiceAccount.Role.Metadata.NamespaceProperty = clusterInfo.Namespace.Metadata.Name;
            clusterInfo.ServiceAccount.Binding.Metadata.NamespaceProperty = clusterInfo.Namespace.Metadata.Name;
            
            using var operation = Logger.Begin($"Preparing Kubernetes service account '{clusterInfo.ServiceAccount.Spec.Metadata.Name}' ...", "Ensure-K8S-ServiceAccount");

            foreach (var item in clusterInfo.ServiceAccount.Binding.Subjects)
            {
                item.NamespaceProperty = clusterInfo.Namespace.Metadata.Name;
            }
            var found = await GetSaFromNamespace(clusterInfo);
            if (found == null)
            {               
                _ = await k8s.CreateNamespacedServiceAccountAsync(clusterInfo.ServiceAccount.Spec,
                    clusterInfo.Namespace.Metadata.Name);
                operation.EndWithSuccess("SA Created");
            }
            else
            {
                using var op = Logger.Begin($"Updating Kubernetes service account '{clusterInfo.ServiceAccount.Spec.Metadata.Name}' ...", "Update-K8S-SA");
                _ = await k8s.ReplaceNamespacedServiceAccountAsync(clusterInfo.ServiceAccount.Spec,
                    clusterInfo.ServiceAccount.Spec.Metadata.Name,
                    clusterInfo.Namespace.Metadata.Name);
                operation.EndWithSuccess("SA Updated");
            }
            await EnsureRoleExistsAsync(clusterInfo);
            await EnsureRoleBindingExistsAsync(clusterInfo);
            // You need to reload this - so the secrets are also poplulated
            return await GetSaFromNamespace(clusterInfo);
        }

        public async Task EnsureRoleBindingExistsAsync(KubernetesEndpointManifest clusterInfo)
        {
            using var op = Logger.Begin($"Preparing Role binding '{clusterInfo.ServiceAccount.Binding.Metadata.Name}' ...", "RoleBinding");
            var all = await k8s.ListNamespacedRoleBindingAsync(clusterInfo.Namespace.Metadata.Name);
            var found = all.Items.FirstOrDefault(al => al.Metadata.Name.Equals(clusterInfo.ServiceAccount.Binding.Metadata.Name));
            if (found == null)
            {   
                await k8s.CreateNamespacedRoleBindingAsync(clusterInfo.ServiceAccount.Binding,
                    clusterInfo.Namespace.Metadata.Name);
                op.EndWithSuccess("Created");
            }
            else
            {                
                await k8s.ReplaceNamespacedRoleBindingAsync(clusterInfo.ServiceAccount.Binding,
                    clusterInfo.ServiceAccount.Binding.Metadata.Name,
                    clusterInfo.Namespace.Metadata.Name);
                op.EndWithSuccess("Updated");
            }
        }

        public async Task EnsureRoleExistsAsync(KubernetesEndpointManifest clusterInfo)
        {
            using var op = Logger.Begin($"Preparing Role '{clusterInfo.ServiceAccount.Role.Metadata.Name}' ...", "K8S-Role");
            var all = await k8s.ListNamespacedRoleAsync(clusterInfo.Namespace.Metadata.Name);
            var found = all.Items.FirstOrDefault(al => al.Metadata.Name.Equals(clusterInfo.ServiceAccount.Role.Metadata.Name));
            if (found == null)
            {   
                await k8s.CreateNamespacedRoleAsync(clusterInfo.ServiceAccount.Role,
                    clusterInfo.Namespace.Metadata.Name);
                op.EndWithSuccess("Created");
            }
            else
            {                
                await k8s.ReplaceNamespacedRoleAsync(clusterInfo.ServiceAccount.Role,
                    clusterInfo.ServiceAccount.Role.Metadata.Name,
                    clusterInfo.Namespace.Metadata.Name);
                op.EndWithSuccess("Updated");
            }
        }

        private async Task<k8s.Models.V1Namespace> GetNamespaceAsync(KubernetesEndpointManifest clusterInfo)
        {
            var all = await k8s.ListNamespaceAsync();
            var found = all.Items
                .FirstOrDefault(al => al.Metadata.Name.Equals(clusterInfo.Namespace.Metadata.Name,
                StringComparison.OrdinalIgnoreCase));
            return found;
        }

        private async Task<k8s.Models.V1ServiceAccount> GetSaFromNamespace(KubernetesEndpointManifest clusterInfo)
        {
            var all = await k8s.ListNamespacedServiceAccountAsync(clusterInfo.Namespace.Metadata.Name);
            var found = all.Items.FirstOrDefault(al => al.Metadata.Name.Equals(clusterInfo.ServiceAccount.Spec.Metadata.Name));

            return found;
        }
    }
}
