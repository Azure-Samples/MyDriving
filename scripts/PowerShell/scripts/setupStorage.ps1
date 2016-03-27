Param (
	[string] $StorageAccountName,
	[string] $StorageAccountKey,
	[string] $ContainerName 
)
try {
	Write-Output "Creating the '$ContainerName' blob container..."
	$ctx = New-AzureStorageContext $StorageAccountName -StorageAccountKey $StorageAccountKey
	New-AzureStorageContainer -Name $ContainerName -Permission Off -Context $ctx -ErrorAction Stop
}
catch {
    if ($Error[0].CategoryInfo.Category -ne "ResourceExists") {
        throw
    }

    Write-Warning "Blob container '$ContainerName' already exists..."
}
