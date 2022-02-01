
Write-Host "#####################################################"
Write-Host "### Installing New Relic Agent"
Write-Host "#####################################################"
Write-Host " "

$fileName = 'install_nr.ps1' `
    Write-Host "## Downloading New Relic Agent"; `
    (New-Object System.Net.WebClient).DownloadFile("https://download.newrelic.com/install/newrelic-cli/scripts/install.ps1", $fileName); `
    Write-Host "## Installing New Relic Agent"; `
    & $fileName; `
    & Remove-Item $fileName;
    & 'C:\Program Files\New Relic\New Relic CLI\newrelic.exe' install

Write-Host " "
Write-Host "## New Relic Agent Setup Complete!!!"
Write-Host " "
Write-Host " "
