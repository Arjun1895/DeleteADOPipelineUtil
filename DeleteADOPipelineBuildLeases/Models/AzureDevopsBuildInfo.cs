using Newtonsoft.Json;

namespace DeleteADOPipelineBuildLeases.Models
{
    public class Build
    {
        public int Id { get; set; }
        public bool RetainedByRelease { get; set; }
    }

    public class BuildLease
    {
        public int LeaseId { get; set; }
        public bool ProtectPipeline { get; set; }
    }

    public class AzureDevopsBuildInfo
    {
        [JsonProperty("value")]
        public List<Build> Value { get; set; }
    }

    public class AzureDevopsLeaseInfo
    {
        [JsonProperty("value")]
        public List<BuildLease> Value { get; set; }
    }
}
