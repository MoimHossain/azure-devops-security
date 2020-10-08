

using Didactic.ApiServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Didactic.Schema
{
    public enum ManifestKind
    {
        [MappedApiServiceAttribute(typeof(ProjectApiService))]
        Project,
        Repository,
        Group,
        BuildFolder,
        ReleaseFolder,
        AreaPath,
        IterationPath
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class MappedApiServiceAttribute : Attribute
    {
        private readonly Type targetType;

        public MappedApiServiceAttribute(Type targetType)
        {
            this.targetType = targetType;
        }

        public static BaseApiService GetApiServiceInstance(ManifestKind enm, string orgUri, string pat)
        {
            MemberInfo[] mi = enm.GetType().GetMember(enm.ToString());
            if (mi != null && mi.Length > 0)
            {
                MappedApiServiceAttribute attr = Attribute.GetCustomAttribute(mi[0],
                    typeof(MappedApiServiceAttribute)) as MappedApiServiceAttribute;
                if (attr != null)
                {
                    return Activator.CreateInstance(attr.targetType, new object[] { orgUri, pat }) as BaseApiService;
                }
            }
            throw new ArgumentOutOfRangeException($"No service mapped to {enm.ToString()}");
        }
    }
}
