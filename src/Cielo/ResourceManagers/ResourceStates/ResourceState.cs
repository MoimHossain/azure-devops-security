using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.ResourceManagers.ResourceStates
{
    public class ResourceState
    {
        public ResourceState()
        {
            Properties = new Dictionary<string, object>();
        }

        public bool Exists { get; set; }
        public bool Changed { get; set; }

        public IDictionary<string, object> Properties { get; private set; }
    }
}
