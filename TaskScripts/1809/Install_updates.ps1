Write-Host "#####################################################"
Write-Host "### Install Update Providers"
Write-Host "#####################################################"
Write-Host " "
$null = Install-Module PSWindowsUpdate -Force
Add-WUServiceManager -MicrosoftUpdate -Confirm:$false
Write-Host " "
Write-Host " "
Write-Host "#####################################################"
Write-Host "### Download and Install Updates"
Write-Host "#####################################################"
Write-Host " "
Get-WindowsUpdate
Install-WindowsUpdate -MicrosoftUpdate -AcceptAll -AutoReboot
Write-Host " "
Write-Host " "
