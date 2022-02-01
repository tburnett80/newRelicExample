[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

Write-Host "#####################################################"
Write-Host "### Setup Host level requirements"
Write-Host "#####################################################"

Write-Host "## Removing Windows Defender"
Write-Host " "
Uninstall-WindowsFeature Windows-Defender
Write-Host " "
Write-Host " "

Write-Host "## Installing Chocolatey..."
Write-Host " "
cmd /c "powershell iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))"

#update vars from the choco install"
Start-Sleep -s 3
Import-Module "C:\ProgramData\chocolatey\helpers\chocolateyProfile.psm1"
Start-Sleep -s 3
Update-SessionEnvironment
Write-Host " "
Write-Host " "

Write-Host "## Installing GIT..."
Write-Host " "
choco install git.install -y
Start-Sleep -s 3
Update-SessionEnvironment
Write-Host " "
Write-Host " "

#install nuget provider
Write-Host "## Installing Nuget Provider for Powershell..."
Write-Host " "
$null = Install-PackageProvider -Name "Nuget" -Force
Write-Host " "
Write-Host " "

Write-Host "#####################################################"
Write-Host "### Download repo to ensure latest scripts"
Write-Host "#####################################################"
Write-Host " "
mkdir C:\src
git clone https://github.com/tburnett80/newRelicExample.git C:\src
Write-Host " "
Write-Host " "
Write-Host "#####################################################"
Write-Host "### Run task scripts to 'bootstrap' this new host"
Write-Host "#####################################################"
Write-Host " "

Write-Host "#### Installing Docker..."
Invoke-Expression -Command C:\src\TaskScripts\1809\setup_docker_host.ps1
Write-Host " "

Write-Host "#### Installing Monitoring Agent..."
Invoke-Expression -Command C:\src\TaskScripts\1809\setup_newrelic_agent.ps1
Write-Host " "

Write-Host "#### Creating Reboot tasks..."
Invoke-Expression -Command C:\src\TaskScripts\1809\add_reboot_tasks.ps1
Write-Host " "

Write-Host "#### Check for Updates..."
Invoke-Expression -Command C:\src\TaskScripts\1809\install_updates.ps1
Write-Host " "
Write-Host " "

Write-Host "#####################################################"
Write-Host "### Reboot to finish any setup before testing"
Write-Host "#####################################################"
Write-Host " "
Write-Host "New Host setup complete. Rebooting in 60 seconds."
Write-Host "Reboot may require updates to install and take a several minutes."
Write-Host "Docker may not be available for up to 10 minutes after reboot as a result."
shutdown -r -t 60
Write-Host " "