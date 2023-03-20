

namespace Cielo.Manifests.Common
{
    public class Metadata
    {        
        public string? Name { get; set; }
        public string? Description { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
