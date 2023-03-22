using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Azdo.Dtos
{
	public partial class Project
	{
		[JsonProperty("id")]
		public Guid Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	}

	public partial class ProjectCollection
	{
		[JsonProperty("count")]
		public long Count { get; set; }

		[JsonProperty("value")]
		public Project[] Value { get; set; }
	}
	public partial class ProjectProperty
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("value")]
		public string Value { get; set; }
	}

	public partial class ProjectPropertyCollection
	{
		private const string _CurrentTemplateId = "System.CurrentProcessTemplateId";
        private const string _OriginalProcessTemplateId = "System.OriginalProcessTemplateId";
        private const string _ProcessTemplateType = "System.ProcessTemplateType";
        private const string _Template = "System.Process Template";
        private const string _TeamCount = "System.Microsoft.TeamFoundation.Team.Count";
        private const string _DefaultTeam = "System.Microsoft.TeamFoundation.Team.Default";
        private const string _SourceControlCapabilityFlags = "System.SourceControlCapabilityFlags";
        private const string _SourceControlGitEnabled = "System.SourceControlGitEnabled";
        private const string _SourceControlGitPermissionsInitialized = "System.SourceControlGitPermissionsInitialized";

        [JsonProperty("count")]
		public long Count { get; set; }

		[JsonProperty("value")]
		public ProjectProperty[] Value { get; set; }


        public string CurrentTemplateId { get { return GetValueByNameCore(_CurrentTemplateId); } }
        public string OriginalProcessTemplateId { get { return GetValueByNameCore(_OriginalProcessTemplateId); } }
        public string ProcessTemplateType { get { return GetValueByNameCore(_ProcessTemplateType); } }
        public string Template { get { return GetValueByNameCore(_Template); } }
        public string TeamCount { get { return GetValueByNameCore(_TeamCount); } }
        public string DefaultTeam { get { return GetValueByNameCore(_DefaultTeam); } }
        public string SourceControlCapabilityFlags { get { return GetValueByNameCore(_SourceControlCapabilityFlags); } }
        public string SourceControlGitEnabled { get { return GetValueByNameCore(_SourceControlGitEnabled); } }
        public string SourceControlGitPermissionsInitialized { get { return GetValueByNameCore(_SourceControlGitPermissionsInitialized);
    }
}

private string GetValueByNameCore(string name) 
		{
			if(this.Value != null)
			{
				var literal = this.Value.FirstOrDefault(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
				if(literal != null )
				{
					return literal.Value;

                }
			}
			return string.Empty;
		}
	}
}