

using k8s;
using k8s.Models;
using Kdoctl.Schema;
using Kdoctl.Schema.CliServices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.K8sServices
{
    public class K8sService
    {   
        public async Task EnsureNamespaceExistsAsync(
            KubernetesEndpointManifest clusterInfo, ConsoleLogger Logger)
        {
            Logger.StatusBegin($"Checking Kubernetes namespace '{clusterInfo.Namespace.Metadata.Name}' ...");
            var found = await GetNamespaceAsync(clusterInfo);
            if (found == null)
            {   
                await k8s.CreateNamespaceAsync(clusterInfo.Namespace);
                Logger.StatusEndSuccess("Namespace Created");
            }
            else
            {
                Logger.StatusBegin($"Updating Kubernetes namespace '{clusterInfo.Namespace.Metadata.Name}' ...");
                await k8s.ReplaceNamespaceAsync(clusterInfo.Namespace, clusterInfo.Namespace.Metadata.Name);
                Logger.StatusEndSuccess("Namespace updated");
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

        public async Task<k8s.Models.V1ServiceAccount> EnsureServiceAccountExists(
            KubernetesEndpointManifest clusterInfo,
            ConsoleLogger Logger)
        {
            clusterInfo.ServiceAccount.Spec.Metadata.NamespaceProperty = clusterInfo.Namespace.Metadata.Name;
            clusterInfo.ServiceAccount.Role.Metadata.NamespaceProperty = clusterInfo.Namespace.Metadata.Name;
            clusterInfo.ServiceAccount.Binding.Metadata.NamespaceProperty = clusterInfo.Namespace.Metadata.Name;
            
            Logger.StatusBegin($"Preparing Kubernetes service account '{clusterInfo.ServiceAccount.Spec.Metadata.Name}' ...");

            foreach (var item in clusterInfo.ServiceAccount.Binding.Subjects)
            {
                item.NamespaceProperty = clusterInfo.Namespace.Metadata.Name;
            }
            var found = await GetSaFromNamespace(clusterInfo);
            if (found == null)
            {               
                _ = await k8s.CreateNamespacedServiceAccountAsync(clusterInfo.ServiceAccount.Spec,
                    clusterInfo.Namespace.Metadata.Name);
                Logger.StatusEndSuccess("SA Created");
            }
            else
            {
                Logger.StatusBegin($"Updating Kubernetes service account '{clusterInfo.ServiceAccount.Spec.Metadata.Name}' ...");
                _ = await k8s.ReplaceNamespacedServiceAccountAsync(clusterInfo.ServiceAccount.Spec,
                    clusterInfo.ServiceAccount.Spec.Metadata.Name,
                    clusterInfo.Namespace.Metadata.Name);
                Logger.StatusEndSuccess("SA Updated");
            }
            await EnsureRoleExistsAsync(clusterInfo, Logger);
            await EnsureRoleBindingExistsAsync(clusterInfo, Logger);
            // You need to reload this - so the secrets are also poplulated
            return await GetSaFromNamespace(clusterInfo);
        }

        public async Task EnsureRoleBindingExistsAsync(
            KubernetesEndpointManifest clusterInfo, 
            ConsoleLogger Logger)
        {
            Logger.StatusBegin($"Preparing Role binding '{clusterInfo.ServiceAccount.Binding.Metadata.Name}' ...");
            var all = await k8s.ListNamespacedRoleBindingAsync(clusterInfo.Namespace.Metadata.Name);
            var found = all.Items.FirstOrDefault(al => al.Metadata.Name.Equals(clusterInfo.ServiceAccount.Binding.Metadata.Name));
            if (found == null)
            {   
                await k8s.CreateNamespacedRoleBindingAsync(clusterInfo.ServiceAccount.Binding,
                    clusterInfo.Namespace.Metadata.Name);
                Logger.StatusEndSuccess("Created");
            }
            else
            {                
                await k8s.ReplaceNamespacedRoleBindingAsync(clusterInfo.ServiceAccount.Binding,
                    clusterInfo.ServiceAccount.Binding.Metadata.Name,
                    clusterInfo.Namespace.Metadata.Name);
                Logger.StatusEndSuccess("Updated");
            }
        }

        public async Task EnsureRoleExistsAsync(KubernetesEndpointManifest clusterInfo, ConsoleLogger Logger)
        {
            Logger.StatusBegin($"Preparing Role '{clusterInfo.ServiceAccount.Role.Metadata.Name}' ...");
            var all = await k8s.ListNamespacedRoleAsync(clusterInfo.Namespace.Metadata.Name);
            var found = all.Items.FirstOrDefault(al => al.Metadata.Name.Equals(clusterInfo.ServiceAccount.Role.Metadata.Name));
            if (found == null)
            {   
                await k8s.CreateNamespacedRoleAsync(clusterInfo.ServiceAccount.Role,
                    clusterInfo.Namespace.Metadata.Name);
                Logger.StatusEndSuccess("Created");
            }
            else
            {
                Logger.StatusBegin($"Updating Role '{clusterInfo.ServiceAccount.Role.Metadata.Name}' ...");
                await k8s.ReplaceNamespacedRoleAsync(clusterInfo.ServiceAccount.Role,
                    clusterInfo.ServiceAccount.Role.Metadata.Name,
                    clusterInfo.Namespace.Metadata.Name);
                Logger.StatusEndSuccess("Updated");
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

        private readonly Kubernetes k8s;
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static K8sService Cluster = new();
#pragma warning restore CA2211 // Non-constant fields should not be visible

        private K8sService()
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
        }
    }
}
