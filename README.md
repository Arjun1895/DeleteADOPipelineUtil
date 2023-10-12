# Azure DevOps Build Pipeline Deletion Utility

## Overview

This C# console application helps in the deletion process of build pipelines in Azure DevOps. It addresses the issue where build leases on each build within the pipeline prevent you from deleting the build pipeline. Removing all these build leases manually is a time-consuming task. 
Once all build leases are removed from each build, you can delete the build pipeline as usual in Azure DevOps.

## Prerequisites

Before running the application, please ensure that you configure the following variables in the appsettings.json file with appropriate values:

- **`PersonalAccessToken`**: Your Azure DevOps Personal Access Token (PAT) for authentication. Go to user settings -> Personal Access Tokens in Azure DevOps and create a new token with appropriate permissions for modifying pipelines. Copy the token and paste it below.

- **`BuildDefinitionId`**: The unique identifier of the specific build pipeline you want to manage.

- **`Organization`**: The name of your Azure DevOps organization.

- **`Project`**: The name of your Azure DevOps project.

## Usage

To use this utility, follow these steps:

**Option 1: Using Visual Studio**

1. Clone the repository and open the `DeleteADOPipelineBuildLeases.sln` in Visual Studio.
2. Configure the variables in the `applicationsettings.json` file as mentioned in the prerequisites.
3. Build and run the application.
4. The application will retrieve the build leases for the specified build pipeline and remove them automatically.
5. Once all build leases are removed, you can proceed to delete the build pipeline in Azure DevOps.

**Option 2: Using Executable**

1. Clone the repository and open the `bin\Release\net6.0` folder where the `DeleteADOPipelineBuildLeases.exe` file is present.
2. Edit the `applicationsettings.json` file in a text editor and update the variables as mentioned in the prerequisites.
3. Open a command line in the `bin\Release\net6.0` folder where the `DeleteADOPipelineBuildLeases.exe` file is present.
4. Run the executable by typing `.\DeleteADOPipelineBuildLeases.exe` and press Enter.
5. The application will retrieve the build leases for the specified build pipeline and remove them automatically.
6. Once all build leases are removed, you can proceed to delete the build pipeline in Azure DevOps.
