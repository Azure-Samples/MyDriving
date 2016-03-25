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
            break;
        }
    }
}

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

if ($deployment1 -and $deployment1.ProvisioningState -ne "Succeeded") {
	Write-Error "Failed to provision the prerequisites storage account."
	exit 1
}

# Upload the HQL queries to the storage account container
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Uploading files to blob storage..."
Write-Output "**************************************************************************************************"
. .\scripts\Copy-ArtifactsToBlobStorage.ps1 -StorageAccountName $deployment1.Outputs.storageAccountName.Value `
                                        -StorageAccountKey $deployment1.Outputs.storageAccountKey.Value `
                                        -StorageContainerName $deployment1.Outputs.assetsContainerName.Value

# Create required services
$templateParams = New-Object -TypeName Hashtable
if ($MobileAppRepositoryUrl) {
	Write-Warning "Overriding the mobile app repository URL..."
	$templateParams.Add("MobileAppRepositoryUrl", $MobileAppRepositoryUrl)
}

Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Deploying the resources in the ARM template. This operation may take several minutes..."
Write-Output "**************************************************************************************************"
$deployment2 = New-AzureRmResourceGroupDeployment -Name "$DeploymentName-1" `
													-ResourceGroupName $ResourceGroupName `
													-TemplateFile $TemplateFile `
													-TemplateParameterFile $ParametersFile `
													@templateParams `
													-Force -Verbose

if ($deployment2 -and $deployment2.ProvisioningState -ne "Succeeded") {
	Write-Warning "Skipping the storage and database initialization..."
	Write-Error "At least one resource could not be provisioned successfully. Review the output above to correct any errors and then run the deployment script again."
	exit 2;
}

# Configure blob storage
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Initializing blob storage..."
Write-Output "**************************************************************************************************"
$containerName = $deployment2.Outputs.rawdataContainerName.Value
Write-Output "Creating the '$containerName' blob container..."
try {
	$ctx = New-AzureStorageContext $deployment2.Outputs.storageAccountNameAnalytics.Value -StorageAccountKey $deployment2.Outputs.storageAccountKeyAnalytics.Value
	New-AzureStorageContainer -Name $containerName -Permission Off -Context $ctx -ErrorAction Stop
}
catch {
    if ($Error[0].CategoryInfo.Category -ne "ResourceExists") {
        throw
    }

    Write-Warning "Blob container already exists..."
}

# Initialize SQL databases
Write-Output ""
Write-Output "**************************************************************************************************"
Write-Output "* Preparing the SQL databases..."
Write-Output "**************************************************************************************************"
Write-Output "Initializing the '$deployment2.Outputs.sqlDBName.Value' database..."
. .\scripts\setupDb.ps1 -ServerName $deployment2.Outputs.sqlServerFullyQualifiedDomainName.Value `
						-AdminLogin $deployment2.Outputs.sqlServerAdminLogin.Value `
						-AdminPassword $deployment2.Outputs.sqlServerAdminPassword.Value `
						-DatabaseName $deployment2.Outputs.sqlDBName.Value `
						-ScriptPath $dbSchemaDB

Write-Output "Initializing the '$deployment2.Outputs.sqlAnalyticsDBName.Value' database..."
. .\scripts\setupDb.ps1 -ServerName $deployment2.Outputs.sqlAnalyticsFullyQualifiedDomainName.Value `
						-AdminLogin $deployment2.Outputs.sqlAnalyticsServerAdminLogin.Value `
						-AdminPassword $deployment2.Outputs.sqlAnalyticsServerAdminPassword.Value `
						-DatabaseName $deployment2.Outputs.sqlAnalyticsDBName.Value `
						-ScriptPath $dbSchemaSQLAnalytics

Write-Output ""
Write-Output "The deployment is complete!"
