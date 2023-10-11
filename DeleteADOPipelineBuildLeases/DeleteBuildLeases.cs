using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace DeleteADOPipelineBuildLeases
{
    /// <summary>
    /// This C# console application streamlines the deletion process of build pipelines in Azure DevOps. 
    /// It addresses the issue where build leases on each build within the pipeline prevent you from deleting the build pipeline. Removing all these build leases manually is a time-consuming task, and this code is designed to automate the process.
    /// Once all build leases are removed from each build, you can delete the build pipeline as usual in Azure DevOps.
    /// 
    /// Before running the application, please ensure that you configure the following variables with appropriate values:
    /// - 'personalAccessToken': Your Azure DevOps Personal Access Token (PAT) for authentication. Go to user settings -> Personal Access Tokens in ADO and create a new token with appropriate permissions for modifiying pipelines. Copy the token and paste it below
    /// - 'buildDefinitionId': The unique identifier of the specific build pipeline you want to manage.
    /// - 'organization': The name of your Azure DevOps organization
    /// - 'project': The name of your Azure DevOps project
    /// </summary>

    class DeleteBuildLeases
    {
        static HttpClient client;

        static async Task Main(string[] args)
        {
            var configuration = LoadConfiguration();

            client = InitializeHttpClient(configuration);

            string buildDefinitionId = configuration["BuildDefinitionId"];
            List<Build> builds = await GetAllBuilds(buildDefinitionId);

            var deleteTasks = new List<Task>();
            foreach (Build build in builds)
            {
                if (build.RetainedByRelease)
                {
                    List<BuildLease> leases = await GetBuildLeases(build.Id);

                    foreach (BuildLease lease in leases)
                    {
                        Task deleteTask = DeleteBuildLease(build.Id, lease.LeaseId);
                        deleteTasks.Add(deleteTask);
                    }
                }
            }
            await Task.WhenAll(deleteTasks);
            Console.WriteLine("Successfully deleted all build leases. Now the pipelines can be deleted as usual in ADO");
        }

        static async Task<List<Build>> GetAllBuilds(string buildDefinitionId)
        {
            try
            {
                var response = await client.GetAsync($"_apis/build/builds?definitions={buildDefinitionId}&api-version=7.1-preview.7");
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<BuildModel>(responseBody).Value;
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



        static async Task<List<BuildLease>> GetBuildLeases(int buildId)
        {
            try
            {
                var response = await client.GetAsync($"_apis/build/builds/{buildId}/leases");
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<LeaseModel>(responseBody).Value;
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

        static async Task DeleteBuildLease(int buildId, int leaseId)
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

        static IConfiguration LoadConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
        }

        static HttpClient InitializeHttpClient(IConfiguration configuration)
        {
            string organization = configuration["Organization"];
            string project = configuration["Project"];
            string personalAccessToken = configuration["PersonalAccessToken"];

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"https://dev.azure.com/{organization}/{project}/");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalAccessToken}")));
            return httpClient;
        }
    }
}