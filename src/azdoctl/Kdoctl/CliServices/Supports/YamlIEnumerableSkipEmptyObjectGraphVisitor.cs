

using System.Collections;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;

namespace Kdoctl.CliServices.Supports
{
    public sealed class YamlIEnumerableSkipEmptyObjectGraphVisitor : ChainedObjectGraphVisitor
    {
        public YamlIEnumerableSkipEmptyObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor) : base(nextVisitor)
        {
        }

        private bool IsEmptyCollection(IObjectDescriptor value)
        {
            if (value.Value == null)
                return true;

            if (typeof(IEnumerable).IsAssignableFrom(value.Value.GetType()))
                return !((IEnumerable)value.Value).GetEnumerator().MoveNext();

            return false;
        }

        public override bool Enter(IObjectDescriptor value, IEmitter context)
        {
            if (IsEmptyCollection(value))
                return false;

            return base.Enter(value, context);
        }

        public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
        {
            if (IsEmptyCollection(value))
                return false;

            return base.EnterMapping(key, value, context);
        }
    }

}
