using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace DeleteADOPipelineBuildLeases
{
    /// <summary>
    /// This C# console application automates the deletion of a build pipeline in Azure DevOps using the Azure DevOps REST API.
    /// It addresses the issue where build leases on each build within the pipeline prevent you from deleting the build pipeline. Removing all these build leases manually is a time-consuming task, and this code is designed to automate the process.
    /// 
    /// Please make sure to enter the following values while running the application:
    /// - 'Personal Access Token': Your Azure DevOps Personal Access Token (PAT) for authentication. Go to user settings -> Personal Access Tokens in ADO and create a new token with appropriate permissions for modifiying pipelines. Copy the token
    /// - 'Build Definition Id': The unique identifier of the specific build pipeline you want to manage. You can find it in the url of the build pipeline in ADO
    /// - 'organization': The name of your Azure DevOps organization. Example: MicrosoftIT
    /// - 'project': The name of your Azure DevOps project. Example: OneITVSO
    /// </summary>
    ///

    class DeleteBuildLeases
    {
        static HttpClient client;

        static async Task Main(string[] args)
        {
            try
            {
                var personalAccessToken = GetInput("Enter your ADO Personal Access Token: ").Trim();
                var buildDefinitionId = GetInput("Enter the Build Definition ID (from the url in ado): ").Trim();
                var organization = GetInput("Enter your Azure DevOps organization: ").Trim();
                var project = GetInput("Enter your Azure DevOps project name: ").Trim();
                var deletePipelineConfirmation = GetInput("Do you want to delete the pipeline (y/n)? If n is entered, it will only delete all build leases but will not delete the pipeline: ").Trim().ToLower();

                client = InitializeHttpClient(personalAccessToken, organization, project);

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
                Console.WriteLine($"Successfully deleted leases for all builds within the pipeline: {buildDefinitionId} in ADO");

                if (deletePipelineConfirmation == "y") {
                    await DeletePipeline(buildDefinitionId);
                    Console.WriteLine($"Successfully deleted the build pipeline with id: {buildDefinitionId} in ADO");
                }

                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
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

        static async Task DeletePipeline(string buildDefinitionId)
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

        static string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        static HttpClient InitializeHttpClient(string personalAccessToken, string organization, string project)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri($"https://dev.azure.com/{organization}/{project}/");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalAccessToken}")));
            return httpClient;
        }
    }
}