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
Write-Information "Creating the resource group..."
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Verbose -Force -ErrorAction Stop 

# Create Storage Account
Write-Output "Provisioning the prerequisites..."
$deployment1 = New-AzureRmResourceGroupDeployment -Name "$DeploymentName-0" `
                                                 -ResourceGroupName $ResourceGroupName `
                                                 -TemplateFile $PreReqTemplateFile `
                                                 -Force -Verbose

# Upload HQL Queries to Storage Account Continer
if ($deployment1 -and $deployment1.ProvisioningState -eq "Failed") {
	Write-Error "Failed to provision the prerequisites storage account."
	exit 1;
}

Write-Output "Uploading the prerequisites to blob storage..."
. .\scripts\Copy-ArtifactsToBlobStorage.ps1 -StorageAccountName $deployment1.Outputs.storageAccountName.Value `
                                        -StorageAccountKey $deployment1.Outputs.storageAccountKey.Value `
                                        -StorageContainerName $deployment1.Outputs.assetsContainerName.Value

# Create Required Services
$templateParams = New-Object -TypeName Hashtable
if ($MobileAppRepositoryUrl) {
	Write-Warning "Overriding the mobile app repository URL..."
	$templateParams.Add("MobileAppRepositoryUrl", $MobileAppRepositoryUrl)
}

Write-Output "Deploying the resources in the ARM template..."
$deployment2 = New-AzureRmResourceGroupDeployment -Name "$DeploymentName-1" `
													-ResourceGroupName $ResourceGroupName `
													-TemplateFile $TemplateFile `
													-TemplateParameterFile $ParametersFile `
													@templateParams `
													-Force -Verbose

# Initialize SQL databases
if ($deployment2 -and $deployment2.ProvisioningState -eq "Failed") {
	Write-Error "At least one resource could not be provisioned successfully. Review the output above to correct any errors and then run the deployment script again."
	Write-Warning "Skipped the database initialization..."
	exit 2;
}

Write-Output "Initializing the schema of the SQL databases..."

. .\scripts\setupDb.ps1 -ServerName $deployment2.Outputs.sqlServerFullyQualifiedDomainName.Value `
						-AdminLogin $deployment2.Outputs.sqlServerAdminLogin.Value `
						-AdminPassword $deployment2.Outputs.sqlServerAdminPassword.Value `
						-DatabaseName $deployment2.Outputs.sqlDBName.Value `
						-ScriptPath $dbSchemaDB

. .\scripts\setupDb.ps1 -ServerName $deployment2.Outputs.sqlAnalyticsFullyQualifiedDomainName.Value `
						-AdminLogin $deployment2.Outputs.sqlAnalyticsServerAdminLogin.Value `
						-AdminPassword $deployment2.Outputs.sqlAnalyticsServerAdminPassword.Value `
						-DatabaseName $deployment2.Outputs.sqlAnalyticsDBName.Value `
						-ScriptPath $dbSchemaSQLAnalytics

Write-Output ""
Write-Output "The deployment is complete!"
