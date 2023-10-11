# Azure DevOps Build Pipeline Deletion Utility

## Overview

This C# console application helps in the deletion process of build pipelines in Azure DevOps. It addresses the issue where build leases on each build within the pipeline prevent you from deleting the build pipeline. Removing all these build leases manually is a time-consuming task. 
Once all build leases are removed from each build, you can delete the build pipeline as usual in Azure DevOps.

## Prerequisites

Before running the application, please ensure that you configure the following variables in the DeleteBuildLeases.cs file with appropriate values:

- **`personalAccessToken`**: Your Azure DevOps Personal Access Token (PAT) for authentication. Go to user settings -> Personal Access Tokens in Azure DevOps and create a new token with appropriate permissions for modifying pipelines. Copy the token and paste it below.

- **`buildDefinitionId`**: The unique identifier of the specific build pipeline you want to manage.

- **`organization`**: The name of your Azure DevOps organization.

- **`project`**: The name of your Azure DevOps project.

## Usage

To use this utility, follow these steps:

1. Configure the variables mentioned in the "Prerequisites" section.

2. Build and run the application.

3. The application will retrieve the build leases for the specified build pipeline and remove them automatically.

4. Once all build leases are removed, you can proceed to delete the build pipeline in Azure DevOps.
