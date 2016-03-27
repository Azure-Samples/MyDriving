Param ([string] $subscriptionId, [string] $workspaceName, [string]$location, [string]$ownerId, [string] $storageAccountName, [string] $storageAccountKey, [string] $packageLocation)

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
    $global:authResult = $authContext.AcquireToken($resourceAppIdURI, $clientId, $redirectUri, "Auto")
}

function InvokeManagementAPI($subscriptionId, $uri)
{
    $headerDate = '2014-10-01'
    $authHeader = $global:authResult.CreateAuthorizationHeader()
    $headers = @{"x-ms-version"="$headerDate";"Authorization" = "$authHeader"; "Accept" = "application/json"}
    $method = "GET"
    $URI = "https://management.core.windows.net/$subscriptionId/$uri"
                                                                                                                                              
    $res = Invoke-RestMethod -Uri $URI -Method $method -Headers $headers
    return $res
}

function InvokeManagementAPI_PUT($subscriptionId, $uri, $json)
{
    $headerDate = '2014-10-01'
    $authHeader = $global:authResult.CreateAuthorizationHeader()
    $headers = @{"x-ms-version"="$headerDate";"Authorization" = "$authHeader"; "Accept" = "application/json"}
    $method = "PUT"
    $URI = "https://management.core.windows.net/$subscriptionId/$uri"
                                                                                                                                              
    $res = Invoke-RestMethod -Uri $URI -Method $method -Headers $headers -Body $json -ContentType 'application/json'
    return $res
}

function InvokeMLAPI([string]$key, [string]$uri, [string]$method)
{

    $URI = "https://studioapi.azureml.net/$uri"
    $headers = @{“x-ms-metaanalytics-authorizationtoken”= "$key"}

    $res = Invoke-RestMethod -Uri $URI -Method $method -Headers $headers
    return $res
}

function FindWorkspace([string]$subscriptionId, [string]$workspaceName)
{
    $res = InvokeManagementAPI $subscriptionId "cloudservices/$workspaceName/resources/machinelearning/~/workspaces"
    
    ForEach($wkSpace in $res)
    {
        if ($wkSpace.Name -eq $workspaceName)
        {
            return $wkSpace
        }
    }
    return $null
}

function GetWorkspace([string] $subscriptionId, [string]$workspaceName, [string]$workspaceId)
{
    $res = InvokeManagementAPI $subscriptionId "cloudservices/$workspaceName/resources/machinelearning/~/workspaces/$workspaceId"
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

function CreateWorkspace([string]$subscriptionId, [string]$workspaceName, [string]$location, [string]$storageAccountName, [string]$storageAccountKey, [string]$ownerId)
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

   $res = InvokeManagementAPI_PUT $subscriptionId "cloudservices/$workspaceName/resources/machinelearning/~/workspaces/$guid" $json
} 

function ImportExperiment()
{
    Authenticate

    # Find original expriemenbt

    $original = FindWorkspace $subscriptionId $workspaceName

    if ($original -eq $null)
    {
        CreateWorkspace $subscriptionId $workspaceName $location $storageAccountName $storageAccountKey $ownerId
        $original = FindWorkspace $subscriptionId $workspaceName
    }

    $workspace = GetWorkspace $subscriptionId $workspaceName $original.Id

    UnpackExperiment $workspace.AuthorizationToken.PrimaryToken $original.Id $packageLocation

}

function ExportExperiment([string]$subscriptionId, [string]$workspaceName, [string]$experimentName, [string]$mlKey)
{
    Authenticate

    # Find original expriemenbt

    $original = FindWorkspace $subscriptionId $workspaceName

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
ImportExperiment