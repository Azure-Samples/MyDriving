Param ([string] $dnsName, [string] $certPassword, [string] $keyVault, [string] $keyName, [string] $resourceGroup, [string] $location, [string] $pfxPath)
$password = ConvertTo-SecureString -String $certPassword -AsPlainText –Force
New-SelfSignedCertificate -CertStoreLocation Cert:\CurrentUser\My -DnsName $dnsName -Provider 'Microsoft Enhanced Cryptographic Provider v1.0' | Export-PfxCertificate -File $pfxPath –Password $password
$vault=New-AzureRmKeyVault -VaultName $keyVault -ResourceGroupName $resourceGroup -Location $location -EnabledForDeployment
$key = Add-AzureKeyVaultKey -VaultName $keyVault -Name $keyName -KeyFilePath $pfxPath -KeyFilePassword $password
$bytes = [System.IO.File]::ReadAllBytes($pfxPath)
$base64 = [System.Convert]::ToBase64String($bytes)
$jsonBlob = @{
data = $base64
dataType = 'pfx'
password = $certPassword
} | ConvertTo-Json
$contentBytes = [System.Text.Encoding]::UTF8.GetBytes($jsonBlob)
$content = [System.Convert]::Tobase64String($contentBytes)
$secretValue = ConvertTo-SecureString -String $content -AsPlainText –Force
$result=Set-AzureKeyVaultSecret -VaultName $keyVault -Name $keyName -SecretValue $secretValue
$cert = new-object System.Security.Cryptography.X509Certificates.X509Certificate2 $pfxPath, $password


write-host "Vault Resource Id: "  $vault.ResourceId
write-host "Secret Id: " + $result.Id
write-host "Cert Thumbprint: " $cert.Thumbprint

