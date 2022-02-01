Write-Host "#####################################################"
Write-Host "### Step 1. Ensuring Docker Data context Clean..."
Write-Host "#####################################################"
Write-Host " "
#Ensure context is clean and clear
docker system prune -f
docker rm -f framework
docker rm -f core
docker rmi framework-img
docker rmi core-img

$DD_API_KEY=[System.Environment]::GetEnvironmentVariable('DD_API_KEY', 'Machine');
$DD_LOG_ENDPOINT="https://http-intake.logs.datadoghq.com"
Write-Host " "

Write-Host "#####################################################"
Write-Host "### Step 2. Building Docker Images for example projects..."
Write-Host "#####################################################"
Write-Host " "

#Build the framework image
cd C:\src\FrameworkEx\
docker build --tag framework-img . 

#Build the core image
cd C:\src\CoreEx\
docker build --tag core-img . 
cd C:\src\TaskScripts\1809\

Write-Host "#####################################################"
Write-Host "### Step 3. Starting Containers for new Images..."
Write-Host "#####################################################"
Write-Host " "

#Run the framework image
docker run -d --restart unless-stopped --name 'framework' -p 8085:80 `
 -e DD_API_KEY=$DD_API_KEY `
 -e DD_LOG_ENDPOINT=$DD_LOG_ENDPOINT `
 -e BuildNumber='1.0.0.1' `
  framework-img 

#Run the core image
docker run -d --restart unless-stopped --name 'core' -p 8086:80 `
 -e DD_API_KEY=$DD_API_KEY `
 -e DD_LOG_ENDPOINT=$DD_LOG_ENDPOINT `
 -e application_name='xtra_core_test' `
 -e BuildNumber='2.0.0.1' `
 core-img 

Write-Host " "

Write-Host "#####################################################"
Write-Host "### Step 4. Cleanup..."
Write-Host "#####################################################"
Write-Host " "
docker system prune -af

Write-Host " "
Write-Host "#####################################################"
Write-Host "### Example Environment Setup Complete!!!"
Write-Host "#####################################################"
Write-Host " "