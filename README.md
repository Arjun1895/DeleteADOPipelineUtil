# Azure DevOps Build Pipeline Deletion Utility

## Overview

This C# console application automates the deletion of a build pipeline in Azure DevOps using the Azure DevOps REST API. It addresses the issue where build leases on each build within the pipeline prevent you from deleting the build pipeline. Manually removing all these build leases can be a time-consuming task, and this code is designed to automate the process.

## Prerequisites

Keep the following ready:

- **`PersonalAccessToken`**: Your Azure DevOps Personal Access Token (PAT) for authentication. Go to user settings -> Personal Access Tokens in Azure DevOps and create a new token with appropriate permissions for modifying pipelines. Copy the token

- **`BuildDefinitionId`**: The unique identifier of the specific build pipeline you want to manage

- **`Organization`**: The name of your Azure DevOps organization

- **`Project`**: The name of your Azure DevOps project

## Usage

To use this utility, follow these steps:

**Option 1: Running the executable**

1. Download the code as a zip file and extract it. Navigate to the folder (bin\Release\net6.0) containing the `DeleteADOPipelineBuildLeases.exe` file, then double-click it.
2. Follow the on-screen prompts to enter the prerequisites as mentioned.

![image](https://github.com/Arjun1895/DeleteADOPipelineUtil/assets/147662498/0eedca08-8dd2-4408-843d-ebd86986a706)


**Option 2: Using Visual Studio**

1. Clone the repository and open the `DeleteADOPipelineBuildLeases.sln` in Visual Studio.
2. Build and run the application.
