
Write-Host "#####################################################"
Write-Host "### Installing New Relic Agent"
Write-Host "#####################################################"
Write-Host " "

#$fileName = 'install_nr.ps1'; `
#    Write-Host "## Downloading New Relic Agent"; `
#    (New-Object System.Net.WebClient).DownloadFile("https://download.newrelic.com/install/newrelic-cli/scripts/install.ps1", $fileName); `
#    Write-Host "## Installing New Relic Agent"; `
#    Invoke-Expression -Command .\$fileName;
#    & Remove-Item $fileName;
#    & 'C:\Program Files\New Relic\New Relic CLI\newrelic.exe' install

$LICENSE_KEY=[System.Environment]::GetEnvironmentVariable('NEW_RELIC_API_KEY', 'Machine'); `
	(New-Object System.Net.WebClient).DownloadFile("https://download.newrelic.com/infrastructure_agent/windows/newrelic-infra.msi", "newrelic-infra.msi"); `
	msiexec.exe /qn /i newrelic-infra.msi GENERATE_CONFIG=true LICENSE_KEY="$LICENSE_KEY" | Out-Null; `
	net start newrelic-infra

Write-Host " "
Write-Host "## New Relic Agent Setup Complete!!!"
Write-Host " "
Write-Host " "
