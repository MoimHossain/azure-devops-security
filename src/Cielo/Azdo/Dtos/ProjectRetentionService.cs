using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Azdo.Dtos
{
    public class ProjectRetentionSetting
    {
        public UpdateRetentionSettingSchema ArtifactsRetention { get; set; }
        public UpdateRetentionSettingSchema PullRequestRunRetention { get; set; }
        public UpdateRetentionSettingSchema RetainRunsPerProtectedBranch { get; set; }
        public UpdateRetentionSettingSchema RunRetention { get; set; }
    }

    public class UpdateRetentionSettingSchema
    {
        public int Value { get; set; }
    }
}
