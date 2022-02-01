
#####################################################
### Script shared variables and functions
#####################################################

#The new root dir of the docker images / files
$dockerRoot = "C:\Docker"

#the setting file needed for docker to override the storage location of images and logs.
$dockerSettingRoot = "C:\ProgramData\docker\config"
$dockerSetting = "$($dockerSettingRoot)\daemon.json"


Write-Host "#####################################################"
Write-Host "###  Download and install docker"
Write-Host "#####################################################"

Write-Host "## Installing Microsoft Docker Provider for Powershell..."
Install-Module -Name DockerMsftProvider -Repository PSGallery -Force

Write-Host "## Installing Docker For Windows Enterprise..."
Install-Package -Name docker -ProviderName DockerMsftProvider -RequiredVersion 20.10.0 -verbose -Force 

Write-Host " "
Write-Host "## Docker Setup Complete!!!"
Write-Host " "
