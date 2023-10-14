using DeleteADOPipelineBuildLeases.Models;

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

    class DeleteBuildPipeline
    {
        static async Task Main(string[] args)
        {
            try
            {
                var config = GetAzureDevOpsConfigurationFromUser();
                await DeleteADOBuildPipeline(config);

                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static async Task DeleteADOBuildPipeline(AzureDevOpsConfiguration config)
        {
            var azureDevOpsApiClient = new AzureDevOpsApiClient(config);
            List<Build> builds = await azureDevOpsApiClient.GetAllBuilds(config.BuildDefinitionId);

            var deleteTasks = new List<Task>();

            foreach (Build build in builds)
            {
                if (build.RetainedByRelease)
                {
                    List<BuildLease> leases = await azureDevOpsApiClient.GetBuildLeases(build.Id);

                    foreach (BuildLease lease in leases)
                    {
                        Task deleteTask = azureDevOpsApiClient.DeleteBuildLease(build.Id, lease.LeaseId);
                        deleteTasks.Add(deleteTask);
                    }
                }
            }

            await Task.WhenAll(deleteTasks);
            Console.WriteLine($"Successfully deleted leases for all builds within the pipeline: {config.BuildDefinitionId} in ADO");

            if (config.DeletePipelineConfirmation == "y")
            {
                await azureDevOpsApiClient.DeletePipeline(config.BuildDefinitionId);
                Console.WriteLine($"Successfully deleted the build pipeline with id: {config.BuildDefinitionId} in ADO");
            }
        }

        private static AzureDevOpsConfiguration GetAzureDevOpsConfigurationFromUser()
        {
            return new AzureDevOpsConfiguration
            {
                PersonalAccessToken = GetInput("Enter your ADO Personal Access Token: ").Trim(),
                BuildDefinitionId = GetInput("Enter the Build Definition ID (from the URL in ADO): ").Trim(),
                Organization = GetInput("Enter your Azure DevOps organization: ").Trim(),
                Project = GetInput("Enter your Azure DevOps project name: ").Trim(),
                DeletePipelineConfirmation = GetInput("Do you want to delete the pipeline (y/n)? If 'n' is entered, it will only delete all build leases but will not delete the pipeline: ").Trim().ToLower()
            };
        }

        private static string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}