#Requires -Module AzureRM.Resources

Param(
    #[string] [Parameter(Mandatory=$true)] $ResourceGroupLocation,
    #[string] [Parameter(Mandatory=$true)] $ResourceGroupName
)

$ResourceGroupLocation = "westus"
$ResourceGroupName = "nbeni-rg"

# Variables
[string] $TemplateFile = '..\ARM\scenario_complete.json'
[string] $TemplateFile = '..\ARM\test.json'
[string] $TemplateParametersFile = '..\ARM\scenario_complete.params.json'
[string] $DeploymentName = ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm'))

Import-Module Azure -ErrorAction Stop

$TemplateFile = [System.IO.Path]::Combine($PSScriptRoot, $TemplateFile)
$TemplateParametersFile = [System.IO.Path]::Combine($PSScriptRoot, $TemplateParametersFile)

# verify if user is logged in by querying his subscriptions.
# if none is found assume he is not
$Subscriptions = Get-AzureRmSubscription -ErrorAction SilentlyContinue
if (!($Subscriptions)) {
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

New-AzureRmResourceGroupDeployment -Name $DeploymentName `
                                   -ResourceGroupName $ResourceGroupName `
                                   -TemplateFile $TemplateFile `
                                   -TemplateParameterFile $TemplateParametersFile `
                                   -Force -Verbose