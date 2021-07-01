using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.Schema
{
    public class ServiceEndpointManifest
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ServiceEndpointKind Kind { get; set; }

        public KubernetesEndpointManifest ClusterInfo { get; set; }
    }

    public enum ServiceEndpointKind
    {
        AzureResourceManager,
        Kubernetes
    }

    public class KubernetesEndpointManifest
    {
        public k8s.Models.V1Namespace Namespace { get; set; }
        public KubernetesServiceAccount ServiceAccount { get; set; }
    }

    public class KubernetesServiceAccount
    {
        public k8s.Models.V1ServiceAccount Spec { get; set; }
        public k8s.Models.V1Role Role { get; set; }
        public k8s.Models.V1RoleBinding Binding { get; set; }
    }
}
