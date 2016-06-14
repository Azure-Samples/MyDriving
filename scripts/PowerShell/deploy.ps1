#Requires -Module AzureRM.Resources
Param(
   [string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
   [string] [Parameter(Mandatory=$true)] $ResourceGroupName,
   [string] [Parameter(Mandatory=$false)] $MobileAppRepositoryUrl = $null
)

# Variables
[string] $PreReqTemplateFile = '..\ARM\prerequisites.json'

[string] $TemplateFile = '..\ARM\scenario_complete.json'
[string] $ParametersFile = '..\ARM\scenario_complete.params.json'

[string] $dbSchemaDB = "..\..\src\SQLDatabase\MyDrivingDB.sql" 
[string] $dbSchemaSQLAnalytics = "..\..\src\SQLDatabase\MyDrivingAnalyticsDB.sql"

[string] $DeploymentName = ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm'))

$deployment1 = $null
$deployment2 = $null

Import-Module Azure -ErrorAction Stop

$PreReqTemplateFile = [System.IO.Path]::Combine($PSScriptRoot, $PreReqTemplateFile)
$TemplateFile = [System.IO.Path]::Combine($PSScriptRoot, $TemplateFile)
$ParametersFile = [System.IO.Path]::Combine($PSScriptRoot, $ParametersFile)

# verify if user is logged in by querying his subscriptions.
# if none is found assume he is not
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Retrieving Azure subscription information..."
Write-Output "**************************************************************************************************"
try
{
	$Subscriptions = Get-AzureRmSubscription
	if (!($Subscriptions)) {
		Login-AzureRmAccount 
	}
}
catch
{
    Login-AzureRmAccount 
}

# fail if we still can retrieve any subscription
$Subscriptions = Get-AzureRmSubscription
if (!($Subscriptions)) {
    Write-Host "Login failed or there are no subscriptions available with your account." -ForegroundColor Red
    Write-Host "Please logout using the command azure Remove-AzureAccount -Name [username] and try again." -ForegroundColor Red
    exit
}

$subscription

# if the user has more than one subscriptions force the user to select one
if ($Subscriptions.Length -gt 1) {
    $i = 1
    $Subscriptions | % { Write-Host "$i) $($_.SubscriptionName)"; $i++ }

    while($true)
    {
        $input = Read-Host "Please choose which subscription to use (1-$($Subscriptions.Length))"
        $intInput = -1

        if ([int]::TryParse($input, [ref]$intInput) -and ($intInput -ge 1 -and $intInput -le $Subscriptions.Length)) {
            Select-AzureRmSubscription -SubscriptionId $($Subscriptions.Get($intInput-1).SubscriptionId)
            $subscription = $Subscriptions.Get($intInput-1)
            break;
        }
    }
} else {
    $subscription = $Subscriptions
}

$numberOfDays = 0
$inputOK = $false
do
{
  try
  {
    [int]$numberOfDays = Read-Host "Enter the number of days you want to run the ADF pipeline"
    if ($numberOfDays -gt 0) {
        $inputOK = $true
    }
    else {
        Write-Host -ForegroundColor red "Please enter a number greater than 0."
    }
  }
  catch
  {
    Write-Host -ForegroundColor red "Please enter a numeric value."
  } 
}
until ($inputOK)

# Create or update the resource group using the specified template file and template parameters file
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Creating the resource group..."
Write-Output "**************************************************************************************************"
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Verbose -Force -ErrorAction Stop 

# Create storage account
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Deploying the prerequisites..."
Write-Output "**************************************************************************************************"
$deployment1 = New-AzureRmResourceGroupDeployment -Name "$DeploymentName-0" `
                                                 -ResourceGroupName $ResourceGroupName `
                                                 -TemplateFile $PreReqTemplateFile `
                                                 -Force -Verbose

if ($deployment1.ProvisioningState -ne "Succeeded") {
	Write-Error "Failed to provision the prerequisites storage account."
	exit 1
}

# Upload the HQL queries to the storage account container
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Uploading files to blob storage..."
Write-Output "**************************************************************************************************"
Write-Output "Uploading hive scripts..."
. .\scripts\Copy-ArtifactsToBlobStorage.ps1 -StorageAccountName $deployment1.Outputs.storageAccountName.Value `
											-StorageAccountKey $deployment1.Outputs.storageAccountKey.Value `
											-StorageContainerName $deployment1.Outputs.assetsContainerName.Value `
											-ArtifactsPath '..\..\..\src\HDInsight'

# Create required services
$templateParams = New-Object -TypeName Hashtable
$TemplateParams.Add(("dataFactoryStartDate"), (Get-Date).ToUniversalTime().AddDays(1).ToString("s"))
$TemplateParams.Add(("dataFactoryEndDate"), (Get-Date).ToUniversalTime().AddDays($numberOfDays + 1).ToString("s"))
if ($MobileAppRepositoryUrl) {
	Write-Warning "Overriding the mobile app repository URL..."
	$templateParams.Add("mobileAppRepositoryUrl", $MobileAppRepositoryUrl)
}

Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Deploying the resources in the ARM template. This operation may take several minutes..."
Write-Output "**************************************************************************************************"

Write-Warning "If asked for SQL Server password use strong password as defined here: https://msdn.microsoft.com/library/ms161962.aspx"
Write-Warning "Is at least 8 characters long and combines letters, numbers, and symbol characters within the password"

$deployment2 = New-AzureRmResourceGroupDeployment -Name "$DeploymentName-1" `
													-ResourceGroupName $ResourceGroupName `
													-TemplateFile $TemplateFile `
													-TemplateParameterFile $ParametersFile `
													@templateParams `
													-Force -Verbose

if ($deployment2.ProvisioningState -ne "Succeeded") {
	Write-Warning "Skipping the storage and database initialization..."
	Write-Error "At least one resource could not be provisioned successfully. Review the output above to correct any errors and then run the deployment script again."
	exit 2
}

# Configure blob storage
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Initializing blob storage..."
Write-Output "**************************************************************************************************"
$storageAccountName = $deployment2.Outputs.storageAccountNameAnalytics.Value
$storageAccountKey = $deployment2.Outputs.storageAccountKeyAnalytics.Value
. .\scripts\setupStorage.ps1 -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey -ContainerName $deployment2.Outputs.rawdataContainerName.Value
. .\scripts\setupStorage.ps1 -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey -ContainerName $deployment2.Outputs.tripdataContainerName.Value
. .\scripts\setupStorage.ps1 -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey -ContainerName $deployment2.Outputs.referenceContainerName.Value

Write-Output "Uploading sample data..."
. .\scripts\Copy-ArtifactsToBlobStorage.ps1 -StorageAccountName $storageAccountName `
											-StorageAccountKey $storageAccountKey `
											-StorageContainerName $deployment2.Outputs.tripdataContainerName.Value `
											-ArtifactsPath '..\..\Assets'

# Initialize SQL databases
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Preparing the SQL databases..."
Write-Output "**************************************************************************************************"
$databaseName = $deployment2.Outputs.sqlDBName.Value
Write-Output "Initializing the '$databaseName' database..."
. .\scripts\setupDb.ps1 -ServerName $deployment2.Outputs.sqlServerFullyQualifiedDomainName.Value `
						-AdminLogin $deployment2.Outputs.sqlServerAdminLogin.Value `
						-AdminPassword $deployment2.Outputs.sqlServerAdminPassword.Value `
						-DatabaseName $deployment2.Outputs.sqlDBName.Value `
						-ScriptPath $dbSchemaDB

$databaseName = $deployment2.Outputs.sqlAnalyticsDBName.Value
Write-Output "Initializing the '$databaseName' database..."
. .\scripts\setupDb.ps1 -ServerName $deployment2.Outputs.sqlAnalyticsFullyQualifiedDomainName.Value `
						-AdminLogin $deployment2.Outputs.sqlAnalyticsServerAdminLogin.Value `
						-AdminPassword $deployment2.Outputs.sqlAnalyticsServerAdminPassword.Value `
						-DatabaseName $deployment2.Outputs.sqlAnalyticsDBName.Value `
						-ScriptPath $dbSchemaSQLAnalytics

Write-Output ""

# Provision ML experiments
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Configuring ML..."
Write-Output "**************************************************************************************************"
	
$context = Get-AzureRmContext
$thumbprint = Read-Host "Please provide the thumbprint of your Azure management certificate. Press [Enter] directly to sign in using AAD."

.\scripts\CopyMLExperiment.ps1 $subscription.SubscriptionId 'MyDriving' $ResourceGroupLocation $context.Account.Id $deployment1.Outputs.mlStorageAccountName.Value $deployment1.Outputs.mlStorageAccountKey.Value 'https://storage.azureml.net/directories/2e55da807f4a4273bfa99852d3d6e304/items' 'MyDriving' 'https://storage.azureml.net/directories/a9fb6aeb3a164eedaaa28da34f02c3b0/items' 'MyDriving [Predictive Exp.]' $thumbprint

# Deploy VSTS build definitions
$confirmation = Read-Host "Do you want to deploy VSTS CI? [y/n]"
if ($confirmation -eq 'y') {
	Write-Output ""
	Write-Output "**************************************************************************************************"
	Write-Output "* Configuring VSTS CI..."
	Write-Output "**************************************************************************************************"
	$vstsAccount = Read-Host "Enter your VSTS account (http://<your account>.visualstudio.com)" 
	$vstsPAT = Read-Host "Enter your VSTS PAT: (see http://blog.devmatter.com/personal-access-tokens-vsts/)"
	$vstsProjectName = Read-Host "Enter the VSTS project name to be created"
	$buildFiles = "..\VSTS"
	$localFolder = Read-Host "Enter a local folder where MyDriving code will be checked out"
	.\scripts\importVSTSBuildDefinition.ps1 $vstsAccount $vstsPAT $vstsProjectName $buildFiles $localFolder
}

Write-Output "The deployment is complete!"
