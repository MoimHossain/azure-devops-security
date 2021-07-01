using k8s;
using k8s.Models;
using Kdoctl.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.CliServices.K8sServices
{
    public class K8sService
    {   
        public async Task EnsureNamespaceExistsAsync(KubernetesEndpointManifest clusterInfo)
        {
            var found = await GetNamespaceAsync(clusterInfo);
            if (found == null)
            {
                await k8s.CreateNamespaceAsync(clusterInfo.Namespace);
            }
            else
            {
                await k8s.ReplaceNamespaceAsync(clusterInfo.Namespace, clusterInfo.Namespace.Metadata.Name);
            }
        }

        public Uri GetClusterAPIUri()
        {
            return k8s.BaseUri;
        }

        public async Task<V1Secret> GetSecretAsync(k8s.Models.V1ServiceAccount account)
        {
            var secretName = account.Secrets[0].Name;
            var secret = await k8s.ReadNamespacedSecretAsync(secretName, account.Metadata.NamespaceProperty);
            return secret;
        }

        public async Task<k8s.Models.V1ServiceAccount> EnsureServiceAccountExists(KubernetesEndpointManifest clusterInfo)
        {
            clusterInfo.ServiceAccount.Spec.Metadata.NamespaceProperty = clusterInfo.Namespace.Metadata.Name;
            clusterInfo.ServiceAccount.Role.Metadata.NamespaceProperty = clusterInfo.Namespace.Metadata.Name;
            clusterInfo.ServiceAccount.Binding.Metadata.NamespaceProperty = clusterInfo.Namespace.Metadata.Name;
            foreach(var item in clusterInfo.ServiceAccount.Binding.Subjects)
            {
                item.NamespaceProperty = clusterInfo.Namespace.Metadata.Name;
            }
            var found = await GetSaFromNamespace(clusterInfo);
            if (found == null)
            {
                await k8s.CreateNamespacedServiceAccountAsync(clusterInfo.ServiceAccount.Spec,
                    clusterInfo.Namespace.Metadata.Name);
            }
            else
            {
                await k8s.ReplaceNamespacedServiceAccountAsync(clusterInfo.ServiceAccount.Spec,
                    clusterInfo.ServiceAccount.Spec.Metadata.Name,
                    clusterInfo.Namespace.Metadata.Name);
            }

            await EnsureRoleExistsAsync(clusterInfo);
            await EnsureRoleBindingExistsAsync(clusterInfo);

            return found;
        }

        public async Task EnsureRoleBindingExistsAsync(KubernetesEndpointManifest clusterInfo)
        {
            var all = await k8s.ListNamespacedRoleBindingAsync(clusterInfo.Namespace.Metadata.Name);
            var found = all.Items.FirstOrDefault(al => al.Metadata.Name.Equals(clusterInfo.ServiceAccount.Binding.Metadata.Name));
            if (found == null)
            {
                await k8s.CreateNamespacedRoleBindingAsync(clusterInfo.ServiceAccount.Binding,
                    clusterInfo.Namespace.Metadata.Name);
            }
            else
            {
                await k8s.ReplaceNamespacedRoleBindingAsync(clusterInfo.ServiceAccount.Binding,
                    clusterInfo.ServiceAccount.Binding.Metadata.Name,
                    clusterInfo.Namespace.Metadata.Name);
            }
        }

        public async Task EnsureRoleExistsAsync(KubernetesEndpointManifest clusterInfo)
        {
            var all = await k8s.ListNamespacedRoleAsync(clusterInfo.Namespace.Metadata.Name);
            var found = all.Items.FirstOrDefault(al => al.Metadata.Name.Equals(clusterInfo.ServiceAccount.Role.Metadata.Name));
            if (found == null)
            {
                await k8s.CreateNamespacedRoleAsync(clusterInfo.ServiceAccount.Role,
                    clusterInfo.Namespace.Metadata.Name);
            }
            else
            {
                await k8s.ReplaceNamespacedRoleAsync(clusterInfo.ServiceAccount.Role,
                    clusterInfo.ServiceAccount.Role.Metadata.Name,
                    clusterInfo.Namespace.Metadata.Name);
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

        private Kubernetes k8s;
        public static K8sService Cluster = new K8sService();

        private K8sService()
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();            
            k8s = new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile());
        }
    }
}
