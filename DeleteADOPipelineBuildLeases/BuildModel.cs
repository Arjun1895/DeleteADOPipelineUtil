using Newtonsoft.Json;

namespace DeleteADOPipelineBuildLeases
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

    public class BuildModel
    {
        [JsonProperty("value")]
        public List<Build> Value { get; set; }
    }

    public class LeaseModel
    {
        [JsonProperty("value")]
        public List<BuildLease> Value { get; set; }
    }
}
