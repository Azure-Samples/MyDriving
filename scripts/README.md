# Use auto-deploy scripts
Auto-deploy scripts allow you to deploy the entire starter kit service set on Azure under your own Azure subscriptions. You can choose to use either the PowerShell script or the Bash script. After running the deployment scripts, follow the manual configuration instructions to complete service configurations.

## Prerequisites

### If you use PowerShell

* [Azure PowerShell](http://aka.ms/webpi-azps)
* [An active Azure subscription](https://azure.microsoft.com) with at least 24 available cores (for on-demand HDInsight cluster)
* [Git](https://git-scm.com/) (if you want to set up VSTS CI pipelines)

### If you use Bash

* [Azure CLI](https://azure.microsoft.com/en-us/documentation/articles/xplat-cli-install/)
* [Node.js](http://nodejs.org)
* [An active Azure subscription](https://azure.microsoft.com) with at least 24 available cores (for on-demand HDInsight cluster)

### Azure Management Certificate
When copying Machine Learning workspaces, the script uses an Azure management certificate for authentication. You'll need to create and upload a management certificate and provide the certificate's thumbprint to the script when prompted. For details on creating a management certificate, please see [this article](https://azure.microsoft.com/en-us/documentation/articles/cloud-services-certs-create/). 

## Use PowerShell script
Follow these steps to deploy the starter kit using PowerShell:

1. Launch **deploy.ps1** under the **scripts\PowerShell** folder:

		.\deploy.ps1 <location> <resource group name>

	* _< location >_ is the Azure datacenter where you want the services to be deployed, such as "WestUS".
	* _< resource group name >_ is the name of the deployed resource group. 

2. During deployment, the script will ask you to provide two SQL Database passwords: **sqlServerAdminPassword** and **sqlAnalyticsServerAdminPassword**. The first password is for the Mobile App back end database; the second password is for the analytic database that supports Power BI queries. 

3. Before the Machine Learning workspaces are imported, you'll be asked to provide the thumbprint of your Azure management certificate. Paste in the thumbprint and press [Enter] to continue. If you press [Enter] directly, the script will attempt to sign in using AAD. You can use the latter option if you are the owner of your subscription.

4. Once the service deployment is complete, the script will ask if you want to provision the Visual Studio Team Services continuous integration pipelines. If you answer 'y' (for yes), you'll be prompted to enter the following values before it copies the MyDriving source code to the specified local folder, creates a new VSTS project, checks in the source to the project, and creates all build definitions:
	* **your VSTS account**: The name of your VSTS account. It should have format of _https://[account-name].visualstudio.com_.
	* **your PAT**:  The personal access token (see [http://blog.devmatter.com/personal-access-tokens-vsts/](http://blog.devmatter.com/personal-access-tokens-vsts/)).
	* **project name**: The name of the VSTS project to be created.
	* **local working folder**: The local folder where the MyDriving source code will be copied to.

>Note: ARM deployments may fail because of transient errors. 

## Use Bash script 

Follow these steps to deploy the starter kit using Bash:

1. Install [Node.js](http://nodejs.org).

1. Install the [Azure CLI](https://azure.microsoft.com/en-us/documentation/articles/xplat-cli-install/).

1. Open a Terminal window and go to the **scripts/bash** folder.

1. Install the required dependencies.

    ```
    npm install
    ```
1. Open **/scripts/ARM/scenario_complete.params.nocomments.json** in a text editor and update the **sqlServerAdminPassword** and **sqlAnalyticsServerAdminPassword** parameters. The first password is for the Mobile App back end database; the second password is for the analytic database that supports 
   Power BI queries. Choose a suitable password for each one. 
   
   Next, set **dataFactoryStartDate** and **dataFactoryEndDate** to specify the period during which you intend to run the Azure Data Factory pipelines. Save the updated file.
   
    ```
    "parameters": {
        "iotHubSku": { "value": { "name": "S1", "tier": "Standard", "capacity": 1 } },
        "storageAccountType": { "value": "Standard_LRS" },
     ...
        "sqlServerAdminPassword": { "value": "<CHOOSE-A-PASSWORD>" },
        "sqlAnalyticsServerAdminPassword": { "value": "<CHOOSE-A-PASSWORD>" },
        "dataFactoryStartDate": { "value": "<CHOOSE-A-START-DATE>" ],
        "dataFactoryEndDate": { "value": "<CHOOSE-AN-END-DATE>" ]
    }
    ```

1. Launch the deployment script.

   ``` 
   sh ./deploy.sh --location <location> --name <resource group name>
   ```
	* _< location >_ is the Azure datacenter where you want the services to be deployed, such as "WestUS".
	* _< resource group name >_ is the name of the deployed resource group.

>**Note:** Machine Learning and VSTS configuration are not supported by Bash. Please follow the instructions to use PowerShell to complete the configuration of these services.

## Manual Configuration

### Start Azure Stream Analytics jobs

1. In the [Azure classic portal](https://manage.windowsazure.com/), go to **Stream Analytics** and select the **mydriving-archive** job.

2. Click the **START** button at the bottom of the page. 

3. Similarly, start the **mydriving-vinlookup** job.

### Configure Azure Streaming Analytics Power BI Outputs 

1. In the [Azure classic portal](https://manage.windowsazure.com/), go to **Stream Analytics** and select the **mydriving-hourlypbi** job.

1. Click the **STOP** button at the bottom of the page. You need to stop the job in order to add a new output.

1. Click **OUTPUTS** at the top of the page, and then click **Add Output**.

1. In the **Add an Output** dialog box, select **Power BI** and then click next.

1. In the **Add a Microsoft Power BI output**, supply a work or school account for the Stream Analytics job output. If you already have a Power BI account, select **Authorize Now**. If not, choose **Sign up now**.

	![Adding a Power BI output](Images/adding-powerbi-output.png?raw=true "Adding a Power BI output")

	_Adding a Power BI output_

1. Next, set the following values and click the checkmark:

	- **Output Alias**: PowerBiSink
	- **Dataset Name**: ASA-HourlyData
	- **Table Name**: HourlyTripData
	- **Workspace**: You can use the default

	![Power BI output settings](Images/asa-powerbi-output-settings.png?raw=true "Power BI output settings")

	_Power BI output settings_

1. Click the **START** button to restart the job.

1. Repeat the same steps to configure the **mydriving-sqlpbi** job using the following values:

	- **Output Alias**: PowerBiSink
	- **Dataset Name**: MyDriving-ASAdataset
	- **Table Name**: TripPointData
	- **Workspace**: You can use the default

1. Make sure that **mydriving-archive**, **mydriving-sqlpbi**, **mydriving-hourlypbi**, and **mydriving-vinlookup** jobs are all running.

### Machine Learning Configuration
1. Before you can proceed, you need to obtain the credentials for the storage account and SQL databases in the solution. Go 
   to the [Azure portal](https://portal.azure.com), click **Resource Groups**, select the solution's resource group, then **All Settings**, and then **Resources**. 
    
1. In the list of resources, click the storage account whose name is prefixed with **"mydrivingstr"**, then **All Settings**, and then **Access Keys**. Make a note of the **Storage Account Name** and **Key1**.

   ![Storage Account Credentials](Images/storage-account-credentials.png?raw=true "Storage Account Credentials")
   
   _Storage Account Credentials_

1. Next, locate the **myDrivingAnalyticsDB** SQL Database in the solution, open its **All Settings** blade, and then its **Properties** blade. Make a note of the, database name, **Server Name** and **Server Admin Login** properties.
  
   ![SQL Database Credentials](Images/sql-database-credentials.png?raw=true "SQL Database Credentials")
   
   _SQL Database Credentials_
 
1. Repeat the previous step to obtain the database name, **Server Name** and **Server Admin Login** properties of the **myDrivingDB** SQL Database.

1. Now, go to the [Azure classic portal](https://manage.windowsazure.com/), select the **Machine Learning** service and then the **mydriving** workspace. Open the workspace by selecting **Sign-in to ML Studio**.

1. Click the **Reader** module at the top of the experiment diagram to select it and in the **Properties** pane, set the **Account Name** and **Account Key** properties to the storage account values obtained previously.

   ![Configuring the ML Reader](Images/ml-configure-reader.png?raw=true "Configuring the ML Reader")
   
   _Configuring the ML Reader_
   
1. Click **Run** at the bottom of the page to run the **MyDriving** experiment.

1. Once the run is complete, select the **Train Model** module in the diagram, click **Setup Web Service**, and then **Deploy Web Service**. Reply **yes** when prompted for confirmation.
   
   ![Deploying an ML Web Service](Images/ml-deploy-web-service.png?raw=true "Deploying an ML Web Service")
   
   _Deploying an ML Web Service_
   
1. Switch to the **Predictive Experiment** tab and configure the **Reader** module property by updating the **Account Name** and **Account Key** properties with the same storage account information that you used previously to configure the **Training Experiment**.
 
1. Select one of the two **Writer** modules in the diagram and in the **Properties** pane, update the **Database server name**, **Server user account name**, and **Server user account password** properties with the values obtained previously. Use the values corresponding to the database shown in the **Database name** property. For the **Server user account name** set the value as &lt;user name&gt;@&lt;server name&gt;. Use the password that you specified when you ran the deployment script.
 
  ![Configuring the ML Writer](Images/ml-configure-writer.png?raw=true "Configuring the ML Writer")
   
  _Configuring the ML Writer_
 
1. Repeat the previous step to configure the **Writer** module for the other database.
 
1. Now, click **Run**
 
1. After the run completes, click **Deploy Web Service**.

1. Go back to the [Azure classic portal](https://manage.windowsazure.com/), select the **Machine Learning** service and then the **mydriving** workspace. Now switch to the **Web Services** tab and select the **MyDriving [Predictive Exp.]** web service.

   ![Configuring ML Web Services](Images/ml-web-services.png?raw=true "Configuring ML Web Services")
   
   _Configuring ML Web Services_

1. Click **Add Endpoint**, enter **_retrain_** as the name of the new endpoint, and then click the checkmark.

   ![Adding an ML Web Service Endpoint](Images/ml-adding-endpoint.png?raw=true "Adding an ML Web Service Endpoint")
   
   _Adding an ML Web Service Endpoint_

1. Click **retrain** in the list of endpoints to shown its **Dashboard** and then copy the API key, under the **Quick Glance** section.

1. Click **BATCH EXECUTION** to open the API documentation page and copy the **Request URI** of the **Submit (but not start) a Batch Execution job** operation.

1. Return to the **retrain** endpoint dashboard, click **UPDATE RESOURCE** and copy the **Request URI** of the **Submit (but not start) a Batch Execution job** operation.

1. Keep a record of the API Key and the batch execution and update resource request URIs of the **retrain** endpoint. You'll need these values later to configure the Data Factory's **AzureMLScoringandUpdateLinkedService** linked service. 

1. Return to the **Web Services** list and select the **MyDriving** web service, then select the **default** endpoint to show its **Dashboard**, and then copy the API key, under the **Quick Glance** section.

1. Click **BATCH EXECUTION** to open the API documentation page and copy the **Request URI** of the **Submit (but not start) a Batch Execution job** operation.

1. Keep a record of the API Key and the batch execution request URI of the **default** endpoint. You'll need these values later to configure the Data Factory's **TrainingEndpoint-AMLLinkedService** linked service.

### Azure Data Factory configuration

1. In the [Azure portal](https://portal.azure.com), select the resource group where the solution is deployed and under **Resources**, select the Data Factory resource.

   ![Data Factory](Images/data-factory.png?raw=true "Data Factory")
   
   _Data Factory_

1. In the data factory blade, select the **Author and deploy** action.

1. In the authoring blade, expand **Linked Services** and then select **AzureMLScoringandUpdateLinkedService**.
   
   ![Configuring the Data Factory](Images/configuring-data-factory.png?raw=true "Configuring the Data Factory")
   
   _Configuring the Data Factory_
   
1. Update the linked service definition by entering the information that you obtained previously from the **retrain** endpoint of the **MyDriving [Predictive Exp.]** web service. 

  - **mlEndpoint**: request URI of the batch execution operation for the **retrain** endpoint of the **MyDriving [Predictive Exp.]** web service
  - **apiKey**: API key for the **retrain** endpoint of the **MyDriving [Predictive Exp.]** web service
  - **updateResourceEndpoint**: request URI of the update resource operation for the **retrain** endpoint of the **MyDriving [Predictive Exp.]** web service

   ![Configuring a Linked Service](Images/configuring-linked-service.png?raw=true "Configuring a Linked Service")

    _Configuring a Linked Service_

1. Click **Deploy**.

1. Next, under **Linked Services**, select **TrainingEndpoint-AMLLinkedService** and update its definition by entering the information that you obtained previously from the **default** endpoint of the **MyDriving** web service.

  - **mlEndpoint**: request URI of the batch execution operation of the **default** endpoint of the **MyDriving** web service
  - **apiKey**: API key for the **default** endpoint of the **MyDriving** web service

1. Click **Deploy**.

##### _Additional Information_
You can use the supplied **scripts\PowerShell\scripts\copyMLExperiment.ps1** to import previously packaged ML experiments at these locations:

* https://storage.azureml.net/directories/2e55da807f4a4273bfa99852d3d6e304/items (MyDriving)
* https://storage.azureml.net/directories/a9fb6aeb3a164eedaaa28da34f02c3b0/items (MyDriving [Predictive Exp.])

For example:

```
.\copyMLExperiment.ps1 "[your Azure subscription Id]" "[name of the workspace to be created]" "[location]" "[owner email]" "[storage account name]" "[storage account key]" "[experiement package URL (see above)]"
```

The PowerShell script also provides other useful functions with several other tasks, such as finding a workspace/experiment, and packaging an experiment. For example, to package an existing experiment (so that it can be unpacked to a new workspace), use the following cmdlet:

```
ExportExperiment "[subscription id]" "[workspace name]" "[experiment name]" "[ML key]"
```

### Visual Studio Team Services configuration (optional)
> Note: Visual Studio Team Services provisioning is an optional part in the master deployment script. If you choose not to deploy Visual Studio Team Services during the master deployment, you can use the following steps to provision it.

This script copies the MyDriving source code into a working folder, creates a new VSTS project, checks in the code into the new project, and then imports all build definitions under the **scripts\VSTS** folder. Some build steps such as downloading certificates are removed from the build pipelines. After you import the build pipelines, you should review them and make the necessary adjustments before using them.

1. Before using the following script to import the build definitions, you'll need to create a Personal Access Token (PAT) following the instructions on this page: [http://blog.devmatter.com/personal-access-tokens-vsts/](http://blog.devmatter.com/personal-access-tokens-vsts/).

2. Launch **importVSTSBuildDefinition.ps1** under the **scripts\PowerShell\scripts** folder:

  ```
  .\importVSTSBuildDefinition.ps1 "<your VSTS account>" "<your PAT>" "<project name>" "<folder to the build definition files>" "<local working folder>"
  ```
  
	* _< your VSTS account >_ is the name of your VSTS account. It should have format of _https://[account-name].visualstudio.com_.
	* _< your PAT >_ is the PAT you generated in step 1.
	* _< project name >_ is the name of the VSTS project to be created.
	* _< Folder to the build definition file >_ is the folder that holds exported build definition (JSON) files. You should point this to the **scripts\VSTS** folder.
	* _< local working folder >_ is the local folder where MyDriving source code will be copied to.

### Service Fabric cluster configuration (optional)
Service Fabric is used as one of the possible extension processing unit hosts. In the starter kit, we provide a sample Service Fabric service that parses vehicle VIN numbers to corresponding make, model, year and type info. The following steps show how to deploy a Service Fabric cluster, and how to publish the VIN look up service using Visual Studio.

It's recommended that you protect your Service Fabric with a certificate. We provide a PowerShell script that helps you to create a self-signed certificate for testing purposes. For production environments, you should use a certificate from a trusted CA.

1. In Windows PowerShell, sign in to your Azure subscription using the **Login-AzureRmAccount** cmdlet.

2. Launch **setupCert.ps1** under the **scripts\PowerShell\scripts** folder:

  ```
  .\setupCert.ps1 <DNS name> <private key password> <Key Vault name> <key name> <resource group name> <location> <full path to .pfx file>
  ```
  
	* _< DNS name >_ is the DNS name of your future Service Fabric cluster, in the format of [cluster name].[location].cloudapp.azure.com, for example: _mydriving-ext.westus.cloudapp.azure.com_.
	* _< private key password >_ is the password of your certificate private key.
	* _< Key Vault name >_ is the name of the Key Vault. Please note the script creates a new Key Vault each time. If you'd like to reuse an existing Key Vault, use $vault=Get-AzureRmKeyVault -VaultName $KeyVault to replace the New-AzureRmKeyVault cmdlet.
	* _< key name >_ is the name of the key in key vault.
	* _< resource group name>_ is the name of the resource group where the Key Vault is to be placed.
	* _< location >_ is the Azure data center location.
    * _< full path to .pfx file >_ is the full path to the .pfx file.

	For example:
  
  ```
  .\setupCert.ps1 mytest.westus.cloudapp.azure.com P@ssword456 testVault1h testKey1h testgrp "West US" c:\src\abc.pfx
  ```

3. The above script generates several outputs, including the resource id of the Key Vault, the secret id and the certificate thumbprint. For example:

  ```
    Vault Resource Id:  /subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourceGroups/testgrp/providers/Microsoft.KeyVault/vaults/testVault1h
    Secret Id:  + https://testvault1h.vault.azure.net:443/secrets/testKeyh/xxxxxxxxxxxxxxxxx
    Cert Thumbprint:  xxxxxxxxxxx
  ```

4. Use the Microsoft Azure Portal to create a new Service Fabric cluster. When configuring cluster security, enter the information items in step 3.

5. Import the certificate to the Trusted People store.
  
  ```
  Import-PfxCertificate -Exportable -CertStoreLocation Cert:\CurrentUser\TrustedPeople -FilePath '[path to the .pfx file]' -Password $password
  ```
6. Open **src\Extensions\ServiceFabirc\VINLookUpApplicaiton\VINLookUpApplicaiton.sln** in Visual Studio 2015.

7. Right-click **VINLookUpApplication** and select the **Publish** menu to publish the application. Select the Service Fabric cluster you provisioned.
