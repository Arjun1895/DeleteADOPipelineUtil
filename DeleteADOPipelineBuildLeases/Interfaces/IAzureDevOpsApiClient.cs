using DeleteADOPipelineBuildLeases.Models;

namespace DeleteADOPipelineBuildLeases.Interfaces
{
    public interface IAzureDevOpsApiClient
    {
        Task<List<Build>> GetAllBuilds(string buildDefinitionId);
        Task<List<BuildLease>> GetBuildLeases(int buildId);
        Task DeleteBuildLease(int buildId, int leaseId);
        Task DeletePipeline(string buildDefinitionId);
    }
}