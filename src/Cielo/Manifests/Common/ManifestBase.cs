

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Manifests.Common
{
    public class ManifestBase
    {
        public string? ApiVersion { get; set; }
        public ManifestKind Kind { get; set; }
        public Metadata? Metadata { get; set; }

        protected virtual bool OnValidateCore()
        {
            return Metadata != null && !string.IsNullOrWhiteSpace(Metadata.Name);
        }

        public bool Validate()
        {
            return OnValidateCore();
        }

        public override string ToString()
        {
            return $"{Kind}: {this.Metadata}";
        }

        /*
        public static ProjectManifest GetEmpty(string projectName, ManifestKind kind)
        {
            return new ProjectManifest
            {
                ApiVersion = "apps/v1",
                Kind = kind,
                Metadata = new MetadataBase
                {
                    Name = projectName
                }
            };
        }*/
    }
}
