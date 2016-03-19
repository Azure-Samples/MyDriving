#Requires -Module AzureRM.Resources

Param(
    [string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
    [string] [Parameter(Mandatory=$true)] $ResourceGroupName
)

# Variables
[string] $PreReqTemplateFile = '..\ARM\prerequisites.json'

[string] $TemplateFile = '..\ARM\scenario_complete.json'
[string] $ParametersFile = '..\ARM\scenario_complete.params.json'

[string] $dbSchemaDB = "..\..\src\SQLDatabase\MyDrivingDB.sql" 
[string] $dbSchemaSQL = "..\..\src\SQLDatabase\MyDrivingAnalyticsDB.sql"

[string] $DeploymentName = ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm'))

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
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Verbose -Force -ErrorAction Stop 

# Create Storage Account
$deployment = New-AzureRmResourceGroupDeployment -Name "$DeploymentName-0" `
                                                 -ResourceGroupName $ResourceGroupName `
                                                 -TemplateFile $PreReqTemplateFile `
                                                 -Force -Verbose

# Upload HQL Queries to Storage Account Continer
if ($deployment -ne $null) {
. .\scripts\Copy-ArtifactsToBlobStorage.ps1 -StorageAccountName $deployment.Outputs.storageAccountName.Value `
                                            -StorageAccountKey $deployment.Outputs.storageAccountKey.Value `
                                            -StorageContainerName $deployment.Outputs.assetsContainerName.Value
}

# Create Required Services
$deployment = New-AzureRmResourceGroupDeployment -Name "$DeploymentName-1" `
							                     -ResourceGroupName $ResourceGroupName `
											     -TemplateFile $TemplateFile `
												 -TemplateParameterFile $ParametersFile `
											     -Force -Verbose

# Initialize SQL databases
if ($deployment -ne $null) {
. .\scripts\setupDb.ps1 $deployment.Outputs.databaseConnectionDB.Value $dbSchemaDB
. .\scripts\setupDb.ps1 $deployment.Outputs.databaseConnectionSQL.Value $dbSchemaSQL
}