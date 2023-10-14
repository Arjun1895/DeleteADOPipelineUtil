using DeleteADOPipelineBuildLeases.Interfaces;
using DeleteADOPipelineBuildLeases.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace DeleteADOPipelineBuildLeases
{
    public class AzureDevOpsApiClient : IAzureDevOpsApiClient
    {
        private readonly HttpClient client;

        public AzureDevOpsApiClient(AzureDevOpsConfiguration config)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri($"https://dev.azure.com/{config.Organization}/{config.Project}/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{config.PersonalAccessToken}")));
        }

        public async Task<List<Build>> GetAllBuilds(string buildDefinitionId)
        {
            try
            {
                var response = await client.GetAsync($"_apis/build/builds?definitions={buildDefinitionId}&api-version=7.1-preview.7");
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<AzureDevopsBuildInfo>(responseBody).Value;
                }
                else
                {
                    Console.WriteLine($"Failed to get builds for build definition: {buildDefinitionId}. Status: {response.StatusCode}. Response Content: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception message: {ex.Message}");
            }
            return new List<Build>();
        }



        public async Task<List<BuildLease>> GetBuildLeases(int buildId)
        {
            try
            {
                var response = await client.GetAsync($"_apis/build/builds/{buildId}/leases");
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<AzureDevopsLeaseInfo>(responseBody).Value;
                }
                else
                {
                    Console.WriteLine($"Failed to get build leases for build id: ${buildId}. Status: ${response.StatusCode}. Response Content: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception message: {ex.Message}");
            }
            return new List<BuildLease>();
        }

        public async Task DeleteBuildLease(int buildId, int leaseId)
        {
            try
            {
                var response = await client.DeleteAsync($"_apis/build/retention/leases?ids={leaseId}&api-version=6.1-preview.1");
                var responseBody = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to delete lease with id: {leaseId} for build id: ${buildId}. Status: ${response.StatusCode}. Response Content: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception message: {ex.Message}");
            }
        }

        public async Task DeletePipeline(string buildDefinitionId)
        {
            try
            {
                var response = await client.DeleteAsync($"_apis/build/definitions/{buildDefinitionId}?api-version=7.2-preview.7");
                var responseBody = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to delete pipeline wit id: ${buildDefinitionId}. Status: ${response.StatusCode}. Response Content: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception message: {ex.Message}");
            }
        }
    }
}
