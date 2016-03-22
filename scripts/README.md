# Use auto-deploy scripts
Auto-deploy scripts allow you to deploy the entire starter kit service set on Azure under your own Azure subscriptions. You can choose to use either the PowerShell script of the Bash script. After running the deployment scripts, follow the manual configuration instructions to complete service configurations.

## Prerequisites

### If you use PowerShell

* [Azure PowerShell](http://aka.ms/webpi-azps)
* [A active Azure subscription](https://azure.microsoft.com) with at least 24 available cores (for on-demand HDInsight cluster)

### If you use Bash

* TBD

## Use PowerShell script
1. Launch **deploy.ps1** under the **scripts\PowerShell** folder:

		.\deploy.ps1 <location> <resource group name>

	* _< location >_ is the Azure datacenter where you want the services to be deployed, such as "West US".
	* _< resource group name >_ is the name of the deployed resource group. 
2. During deployment, the script will ask you to provide two SQL Datbase passwords: **sqlServerAdminPassword** and **sqlAnalyticsServerAdminPassword**. The first password is for the Mobile App back end database; the second password is for the analytic database that supports Power BI queries. 


## Use Bash script 

TBD

## Manual Configuration

### Configuring Power BI Outputs for Azure Streaming Analytics

1. In the [Azure classic portal](https://manage.windowsazure.com/), go to **Stream Analytics** and select **mydriving-hourlypbi**.

1. Click the **STOP** button at the bottom of the page. You need to stop the job in order to add a new output.

1. Click **OUTPUTS** at the top of the page, and then click **Add Output**.

1. In the **Add an Output** dialog box, select **Power BI** and then click next.

1. In the **Add a Microsoft Power BI output**, supply a work or school account for the Stream Analytics job output. If you already have a Power BI account, select **Authorize Now**. If not, choose **Sign up now**.

	![Adding Power BI output](Images/adding-powerbi-output.png?raw=true "Adding Power BI output")

	_Adding Power BI output_

1. Next, set the following values:

	- **Output Alias**: PowerBiSink
	- **Dataset Name**: ASA-HourlyData
	- **Table Name**: HourlyTripData
	- **Workspace**: You can use the default

1. Click the **START** button to restart the job.

1. Repeat the same steps to configure the **mydriving-sqlpbi** Stream Analytics Job using the following values:

	- **Output Alias**: PowerBiSink
	- **Dataset Name**: MyDriving-ASAdataset
	- **Table Name**: TripPointData
	- **Workspace**: You can use the default

### Power BI configuration (optional)
At the time of writing, Power Bi dashboard creation is not automated. To define Power BI reports and dashboards, follow the following steps.

### Visual Studio Online configuration (optional)

### Service Fabric cluster configuration (optional)
Service Fabric is used one of the possible extension processing unit hosts. In the starter kit, we provide a sample Service Fabric service that parses vehicle VIN numbers to corresponding make, mode, year and type info. The following steps show how to deploy a Service Fabric cluster, and how to publish the VIN look up service using Visual Studio.

It's recommended that you protect your Service Fabric with a certificate. We provide a PowerShell script that helps you to create a self-signed certificate for testing purposes. For production environments, you should use a certificate from a trusted CA.

1. In Windows PowerShell, sign in to your Azure subscription using the **Login-AzureRmAccount** cmdlet.
2. Launch **setupCert.ps1** under the **scripts\PowerShell\scripts** folder:

		.\setupCert.ps1 <DNS name> <private key password> <Key Vault name> <key name> <resource group name> <location> <full path to .pfx file>

	* _< DNS name >_ is the DNS name of your future Service Fabric cluster, in the format of [cluster name].[location].cloudapp.azure.com, for example: _mydriving-ext.westus.cloudapp.azure.com_.
	* _< private key password >_ is the password of your certificate private key.
	* _< Key Vault name >_ is the name of the Key Vault. Please note the script creates a new Key Vault each time. If you'd like to reuse an existing Key Vault, use $vault=Get-AzureRmKeyVault -VaultName $KeyVault to replace the New-AzureRmKeyVault cmdlet.
	* _< key name >_ is the name of the key in key vault.
	* _< resource group name>_ is the name of the resource group where the Key Vault is to be placed.
	* _< location >_ is the Azure data center location.
	* _< full path to .pfx file >_ is the full path to the .pfx file.

	For exmaple:

		.\setupCert.ps1 mytest.westus.cloudapp.azure.com P@ssword456 testVault1h testKey1h testgrp "West US" c:\src\abc.pfx
3. The above script generates several outputs, including the resource id of the Key Vault, the secret id and the certificate thumbprint. For example:

		Vault Resource Id:  /subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourceGroups/testgrp/providers/Microsoft.KeyVault/vaults/testVault1h
		Secret Id:  + https://testvault1h.vault.azure.net:443/secrets/testKeyh/xxxxxxxxxxxxxxxxx
		Cert Thumbprint:  xxxxxxxxxxx

4. Use Microsoft Azure Portal to create a new Service Fabric cluster. When configuring cluster security, enter the information items in step 3.
5. Import the certificate to the Trusted People store.

		Import-PfxCertificate -Exportable -CertStoreLocation Cert:\CurrentUser\TrustedPeople -FilePath '[path to the .pfx file]' -Password $password
6. Open **src\Extensions\ServiceFabirc\VINLookUpApplicaiton\VINLookUpApplicaiton.sln** in Visual Studio 2015.
7. Right-click on the **VINLookUpApplication** and select the **Publish** menu to publish the application. Select the Service Fabric cluster you provisioned.  


