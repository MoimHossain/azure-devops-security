


namespace Cielo.ResourceManagers.ResourceStates
{
    public class ResourceState
    {
        private readonly List<string> errors;
        private List<(string, object, bool)> properties { get; init; }

        public ResourceState()
        {
            errors = new List<string>();
            properties = new List<(string, object, bool)>();
        }

        public bool Exists { get; set; }
        public bool Changed { get; set; }        

        public void AddProperty(string name, object value, bool changed = false) =>  properties.Add((name, value, changed));

        public IEnumerable<(string, object, bool)> GetProperties() => this.properties;

        public void AddError(string message) =>  this.errors.Add(message);

        public bool HasErrors { get { return errors.Count > 0; } }

        public IEnumerable<string> GetErrors() => this.errors;
    }
}
