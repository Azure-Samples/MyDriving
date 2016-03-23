Param ([string] $StorageAccountName, [string] $StorageAccountKey, [string] $StorageContainerName, [string] $ArtifactsPath = '..\..\..\src\HDInsight', [string] $AzCopyPath = '..\tools\AzCopy.exe')

$AzCopyPath = [System.IO.Path]::Combine($PSScriptRoot, $AzCopyPath)
$ArtifactsPath = [System.IO.Path]::Combine($PSScriptRoot, $ArtifactsPath)

& $AzCopyPath """$ArtifactsPath""", "https://$StorageAccountName.blob.core.windows.net/$StorageContainerName", "/DestKey:$StorageAccountKey", "/S", "/Y"