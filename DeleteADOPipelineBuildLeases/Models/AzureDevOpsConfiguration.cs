namespace DeleteADOPipelineBuildLeases.Models
{
    public class AzureDevOpsConfiguration
    {
        public string PersonalAccessToken { get; set; }
        public string BuildDefinitionId { get; set; }
        public string Organization { get; set; }
        public string Project { get; set; }
        public string DeletePipelineConfirmation { get; set; }
    }
}
