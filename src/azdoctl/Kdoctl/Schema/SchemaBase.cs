using Kdoctl.CliServices;
using Kdoctl.Schema.CliServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kdoctl.Schema
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


    [AttributeUsage(AttributeTargets.Field)]
    public class MappedApiServiceAttribute : Attribute
    {
        private readonly Type targetType;

        public MappedApiServiceAttribute(Type targetType)
        {
            this.targetType = targetType;
        }

        public static TaskBase GetApiServiceInstance(ManifestKind enm, string orgUri, string pat)
        {
            MemberInfo[] mi = enm.GetType().GetMember(enm.ToString());
            if (mi != null && mi.Length > 0)
            {
                MappedApiServiceAttribute attr = Attribute.GetCustomAttribute(mi[0],
                    typeof(MappedApiServiceAttribute)) as MappedApiServiceAttribute;
                if (attr != null)
                {
                    return Activator.CreateInstance(attr.targetType, new object[] { orgUri, pat }) as TaskBase;
                }
            }
            throw new ArgumentOutOfRangeException($"No service mapped to {enm.ToString()}");
        }
    }
}
