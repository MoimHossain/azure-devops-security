

using System;
using System.Collections.Generic;
using System.Text;

namespace Didactic.Schema
{
    public class TeamSchemaManifest
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<string> Admins { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
