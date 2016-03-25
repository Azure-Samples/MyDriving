param(
    [Parameter(Mandatory=$true, HelpMessage="Your VSTS account (e.g. https://youraccount.visualstudio.com)")]
    [ValidateNotNullOrEmpty()]
    [string]$vstsUrl,
    
    [Parameter(Mandatory=$true, HelpMessage="Your VSTS PAT")]
    [ValidateNotNullOrEmpty()]
    [string]$vstsPat,

    [Parameter(Mandatory=$true, HelpMessage="VSTS Project name")]
    [ValidateNotNullOrEmpty()]
    [string]$projectName,

    [Parameter(Mandatory=$true, HelpMessage="Build definition package")]
    [ValidateNotNullOrEmpty()]
    [string]$buildDefinitionFilePath,

    [Parameter(Mandatory=$true, HelpMessage="Local folder. MyDriving source will be checked out to this folder and committed to your VSTS project.")]
    [ValidateNotNullOrEmpty()]
    [string]$workingPath
)

$ErrorActionPreference = "Stop"

############################################################################
#                           CONSTANTS                                      #
############################################################################

$extensionId = "colinsalmcorner.colinsalmcorner-buildtasks"

# do not change these
$agileTemplateId = "adcc42ab-9882-485e-a3ed-7678f01f66bc"
$createProjectUri = "defaultcollection/_apis/projects?api-version=2.0-preview"
$queryProjectUri = "defaultcollection/_apis/projects/$projectName" + "?api-version=1.0"
$buildDefUri = "defaultcollection/$projectName/_apis/build/definitions?api-version=2.0"
$queueBuildUri = "defaultcollection/$projectName/_apis/build/builds?api-version=2.0"

# create headers
$headers = @{
    Authorization = 'Basic ' + [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$($vstsPat)")) 
}

############################################################################
#                      START OF SCRIPT                                     #
############################################################################

############################################################################
Write-Host "Copying files" -ForegroundColor Yellow

if (!(Test-Path -Path $workingPath)) {
    mkdir $workingPath 
}

 xcopy $PSScriptRoot\..\..\..\Extensions $workingPath\Extensions /Y /E /I
 xcopy $PSScriptRoot\..\..\..\scripts $workingPath\scripts /Y /E /I
 xcopy $PSScriptRoot\..\..\..\src $workingPath\src /Y /E /I
 copy $PSScriptRoot\..\..\..\.gitignore $workingPath
############################################################################
Write-Host "Checking if VSTS Team Project $projectName exists" -ForegroundColor Yellow
$uri = "$vstsUrl/$queryProjectUri" -f $projectName
$projectExists = $true
try {
    $response = Invoke-RestMethod -Method Get -Uri $uri -Headers $headers
    Write-Host "Team project already exists" -ForegroundColor Cyan
} catch {
    $projectExists = $false
    Write-Host "Team project does not exist" -ForegroundColor Cyan
}

############################################################################
Push-Location
if ($projectExists) {
    Write-Host "Cloning existing repo" -ForegroundColor Yellow
    
    cd $workingPath
    
    git clone "$vstsUrl/DefaultCollection/_git/$projectName"
}
else {
    Write-Host "Creating VSTS Team Project $projectName" -ForegroundColor Yellow
    $uri = "$vstsUrl/$createProjectUri"

    $body = @{
        name = $projectName
        description = "Project created for Build Workshop"
        capabilities = @{
            versioncontrol = @{
                sourceControlType = "Git"
            }
            processTemplate = @{
            templateTypeId = $agileTemplateId
            }
        }
    }

    $response = Invoke-RestMethod -Method Post -Uri $uri -ContentType "application/json" `
                                -Headers $headers -Body (ConvertTo-Json $body)

    # wait for the project to be created
    $uri = "$vstsUrl/$queryProjectUri"
    $projectId = ""
    for ($i = 0; $i -lt 15; $i++) {
        Write-Host "   waiting for Team Project job to complete..."
        sleep 20
        
        $response = Invoke-RestMethod -Method Get -Uri $uri -Headers $headers
        if ($response.state -eq "wellFormed") {
            $projectId = $response.id
            break
        }
    }
    if ($projectId -ne "") {
        Write-Host "VSTS Team project was created successfully" -ForegroundColor Cyan
    } else {
        throw "Could not create VSTS project (timeout)"
    }

    ############################################################################
    Write-Host "Initializing local Git repo and committing code" -ForegroundColor Yellow

    cd "$workingPath"

    git init

    git add .

    git commit -m "Initial commit"

    ############################################################################
    Write-Host "Pushing code to VSTS" -ForegroundColor Yellow

    git remote add origin "$vstsUrl/DefaultCollection/_git/$projectName"
    git push -u origin --all
}
Pop-Location

############################################################################

$buildFiles = Get-ChildItem $buildDefinitionFilePath *.json

Foreach ($file in $buildFiles)
{
    $buildName = $file.Name.Substring(0, $file.Name.Length-5)

    Write-Host "Checking if team build $buildName exists" -ForegroundColor Yellow
    $uri = "$vstsUrl/$($buildDefUri)&name=$buildName"

    $response = Invoke-RestMethod -Method Get -Uri $uri -Headers $headers
    if ($response.count -eq 0) {
        Write-Host "Creating Team build $buildName" -ForegroundColor Yellow
        $uri = "$vstsUrl/$buildDefUri"

        
        $buildDef = (Get-Content $file.FullName) -join "`n" | ConvertFrom-Json

        # set some parameters for the build definition
        $buildDef.name = $buildName
        $buildDef.repository.url = "$vstsUrl/DefaultCollection/_git/$projectName"
        $buildDef.repository.name = $projectName

        $response = Invoke-RestMethod -Method Post -Uri $uri -ContentType "application/json" `
                                  -Headers $headers -Body (ConvertTo-Json $buildDef -Depth 10)
        $buildDefId = $response.Id
    } else {
        Write-Host "Build $buildName already exists" -ForegroundColor Cyan
        $buildDefId = $response.value.id
    }
}
Write-Host "Done!" -ForegroundColor Green