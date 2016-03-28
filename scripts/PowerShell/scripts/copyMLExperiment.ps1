Param ([string] $subscriptionId, [string] $workspaceName, [string]$location, [string]$ownerId, [string] $storageAccountName, [string] $storageAccountKey, [string] $packageLocation, [string]$thumbprint)

$global:authResult = $null

function Authenticate()
{
    $adal = "${env:ProgramFiles(x86)}\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\Services\Microsoft.IdentityModel.Clients.ActiveDirectory.dll"
    $adalforms = "${env:ProgramFiles(x86)}\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\Services\Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll"
    [System.Reflection.Assembly]::LoadFrom($adal)
    [System.Reflection.Assembly]::LoadFrom($adalforms)

    $clientId = "1950a258-227b-4e31-a9cf-717495945fc2" 
    $redirectUri = "urn:ietf:wg:oauth:2.0:oob"
    $resourceAppIdURI = "https://management.core.windows.net/"
    $authority = "https://login.windows.net/common/oauth2/authorize"
    $authContext = New-Object "Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext" -ArgumentList $authority
    $global:authResult = $authContext.AcquireToken($resourceAppIdURI, $clientId, $redirectUri, "Always")
}

function InvokeManagementAPI([string]$subscriptionId, [string]$uri, [string]$thumbprint)
{
    $headerDate = '2014-10-01'
    if (!$thumbprint) {
        $authHeader = $global:authResult.CreateAuthorizationHeader()
        $headers = @{"x-ms-version"="$headerDate";"Authorization" = "$authHeader"; "Accept" = "application/json"}
    }
    else {
       $headers = @{"x-ms-version"="$headerDate"; "Accept" = "application/json"}
    }
    $method = "GET"
    $URI = "https://management.core.windows.net/$subscriptionId/$uri"
    if (!$thumbprint) {                                                                                                                                          
        $res = Invoke-RestMethod -Uri $URI -Method $method -Headers $headers
    } else {
       $res = Invoke-RestMethod -Uri $URI -Method $method -Headers $headers -CertificateThumbprint $thumbprint
    }
    return $res
}

function InvokeManagementAPI_PUT([string]$subscriptionId, [string]$uri, [string]$json, [string]$thumbprint)
{
    $headerDate = '2014-10-01'
    if (!$thumbprint) {
        $authHeader = $global:authResult.CreateAuthorizationHeader()
        $headers = @{"x-ms-version"="$headerDate";"Authorization" = "$authHeader"; "Accept" = "application/json"}
    } else {
        $headers = @{"x-ms-version"="$headerDate"; "Accept" = "application/json"}
    }
    $method = "PUT"
    $URI = "https://management.core.windows.net/$subscriptionId/$uri"
    if (!$thumbprint) {                                                                                                                                          
        $res = Invoke-RestMethod -Uri $URI -Method $method -Headers $headers -Body $json -ContentType 'application/json'
    }
    else {
       $res = Invoke-RestMethod -Uri $URI -Method $method -Headers $headers -Body $json -CertificateThumbprint $thumbprint -ContentType 'application/json'
    }
    return $res
}

function InvokeMLAPI([string]$key, [string]$uri, [string]$method)
{

    $URI = "https://studioapi.azureml.net/$uri"
    $headers = @{“x-ms-metaanalytics-authorizationtoken”= "$key"}

    $res = Invoke-RestMethod -Uri $URI -Method $method -Headers $headers
    return $res
}

function FindWorkspace([string]$subscriptionId, [string]$workspaceName, [string]$thumbprint)
{
    $res = InvokeManagementAPI $subscriptionId "cloudservices/$workspaceName/resources/machinelearning/~/workspaces" $thumbprint
    
    ForEach($wkSpace in $res)
    {
        if ($wkSpace.Name -eq $workspaceName)
        {
            return $wkSpace
        }
    }
    return $null
}

function GetWorkspace([string] $subscriptionId, [string]$workspaceName, [string]$workspaceId, [string]$thumbprint)
{
    $res = InvokeManagementAPI $subscriptionId "cloudservices/$workspaceName/resources/machinelearning/~/workspaces/$workspaceId" $thumbprint
    return $res
}

function FindExperiment([string]$key, [string]$workspaceId, [string]$experimentName)
{
   $res = InvokeMLAPI $key "api/workspaces/$workspaceId/experiments" "GET"

   ForEach($exp in $res)
   {
        if ($exp.Description -eq $experimentName)
        {
            return $exp
        }
   }
   return $null
}


function PackageExperiment([string]$key, [string]$workspaceId, [string]$experimentId)
{
    $res = InvokeMLAPI $key "api/workspaces/$workspaceId/packages?api-version=2.0&experimentId=$experimentId&clearCredentials=true&includeAuthorId=false" POST

    $result = ""

    $URI = "api/workspaces/$workspaceId/packages?packageActivityId=" + $res.ActivityId
    do {
        try
        {
            $res = InvokeMLAPI $key $URI GET
            write-host "Packaging..."
        }
        catch
        {
            Write-Error $_.Exception.Response
            return
        }
    } 
    until ($res.Status -eq  "Complete")

    return $res.Location
}

function UnpackExperiment([string]$key, [string]$workspaceId, [string] $packageLocation)
{
    $encodedLocation = [System.Web.HttpUtility]::UrlEncode($packageLocation) 

    $res = InvokeMLAPI $key "api/workspaces/$workspaceId/packages?api-version=2.0&packageUri=$encodedLocation" PUT

    $result = ""

    $URI = "api/workspaces/$workspaceId/packages?packageActivityId=" + $res.ActivityId
    do {
        try
        {
            $res = InvokeMLAPI $key $URI GET
            write-host "Unpacking..."
        }
        catch
        {
            #Write-Error $_.Exception.Response
            return
        }
    } 
    until ($res.Status -eq  "Complete")
}

function CreateWorkspace([string]$subscriptionId, [string]$workspaceName, [string]$location, [string]$storageAccountName, [string]$storageAccountKey, [string]$ownerId, [string]$thumbprint)
{
   $guid=[guid]::NewGuid().ToString('N')
   $json = @{
        Name="$workspaceName"
        Location= "$location" 
        StorageAccountName="$storageAccountName"
        StorageAccountKey= "$storageAccountKey"
        OwnerId= "$ownerId"
        ImmediateActivation=$true
        Source= "$workpaceName"
   }|ConvertTo-Json

   $res = InvokeManagementAPI_PUT $subscriptionId "cloudservices/$workspaceName/resources/machinelearning/~/workspaces/$guid" $json $thumbprint
} 

function ImportExperiment([string]$subscriptionId, [string]$workspaceName, [string]$location, [string]$storageAccountName, [string]$storageAccountKey, [string]$ownerId, [string]$packageLocation, [string]$thumbprint)
{
    if (!$thumbprint) {
        Authenticate
    }

    # Find original expriemenbt

    $original = FindWorkspace $subscriptionId $workspaceName $thumbprint

    if ($original -eq $null)
    {
        CreateWorkspace $subscriptionId $workspaceName $location $storageAccountName $storageAccountKey $ownerId $thumbprint
        $original = FindWorkspace $subscriptionId $workspaceName $thumbprint
    }

    $workspace = GetWorkspace $subscriptionId $workspaceName $original.Id $thumbprint

    UnpackExperiment $workspace.AuthorizationToken.PrimaryToken $original.Id $packageLocation

}

function ExportExperiment([string]$subscriptionId, [string]$workspaceName, [string]$experimentName, [string]$mlKey, [string]$thumbprint)
{
    if (!$thumbprint) {
        Authenticate
    }
    # Find original expriemenbt

    $original = FindWorkspace $subscriptionId $workspaceName $thumbprint

    if ($original -eq $null)
    {
        return
    }

    $originalExperiment = FindExperiment $mlKey $original.Id $experimentName $original.Id

    write-host $originalExperiment

    # Package origianl experiment

    $packageLocation = PackageExperiment $mlKey $original.Id $originalExperiment.ExperimentId

    write-host $packageLocation
}

#ExportExperiment "[subscription id]" "[workspace name]" "MyDriving [Predictive Exp.]" "[ML key]"

ImportExperiment $subscriptionId $workspaceName $location $storageAccountName $storageAccountKey $ownerId $packageLocation $thumbprint 
