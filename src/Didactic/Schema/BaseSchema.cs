using System;
using System.Collections.Generic;
using System.Text;

namespace Didactic.Schema
{
    public class BaseSchema
    {
        public string ApiVersion { get; set; }
        public ManifestKind Kind { get; set; }
        public MetadataBase Metadata { get; set; }

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
    }

    public class MetadataBase
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}