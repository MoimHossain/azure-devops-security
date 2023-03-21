

using Cielo.ResourceManagers.Abstract;
using System.Reflection;

namespace Cielo.Manifests.Common
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ManifestResourceMapAttribute : Attribute
    {
        private readonly Type targetType;

        public ManifestResourceMapAttribute(Type targetType)
        {
            this.targetType = targetType;
        }

        public static ResourceManagerBase? GetManifestFromYaml(
            IServiceProvider serviceProvider,
            ManifestBase baseManifest, string rawManifest)
        {
            var enm = baseManifest.Kind;
            MemberInfo[] mi = enm.GetType().GetMember(enm.ToString());
            if (mi != null && mi.Length > 0)
            {
                ManifestResourceMapAttribute? attr = Attribute.GetCustomAttribute(mi[0],
                    typeof(ManifestResourceMapAttribute)) as ManifestResourceMapAttribute;
                if (attr != null)
                {
                    var manager = (ResourceManagerBase?)Activator.CreateInstance(attr.targetType, serviceProvider, rawManifest);

                    return manager;
                }
            }
            throw new ArgumentOutOfRangeException($"No resource mapped to {enm}");
        }
    }
}
