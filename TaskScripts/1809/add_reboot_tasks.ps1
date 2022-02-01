

##########
# Function to write script file to disk
##########
function writeScriptFile {
    Param([Parameter(Mandatory=$true)][string]$thePath,
          [Parameter(Mandatory=$true)][string]$body)

   $body|  Out-File -FilePath $thePath
}

##########
# Function to create the run once task for reboot
##########
function addRebootScriptTask {
    Param([Parameter(Mandatory=$true)][string]$thePath,
          [Parameter(Mandatory=$true)][string]$tskName)

   $run = (Get-Date).AddMinutes(2) # Two minutes from now
   $act = New-ScheduledTaskAction -Execute "powershell.exe" -Argument "-ExecutionPolicy Bypass -File `"$($thePath)`""
   $trigger = New-ScheduledTaskTrigger -AtStartup -RandomDelay 00:00:30 
   $set = New-ScheduledTaskSettingsSet -StartWhenAvailable -DeleteExpiredTaskAfter 00:00:30 -Compatibility Win8
   $prnc = New-ScheduledTaskPrincipal -UserId SYSTEM -LogonType ServiceAccount -RunLevel Highest
   $def = New-ScheduledTask -Action $act -Principal $prnc -Trigger $trigger -Settings $set -Description "Updates the docker data root after the first time it starts"
   
   Register-Scheduledtask -TaskName $tskName -InputObject ($def | %{ $_.Triggers[0].EndBoundary = $run.AddMinutes(60).ToString('s') ; $_ } )
}

##########
# Function to make sure directory path exists, optionally deleting it first if it exists
##########
function ensureDir {
    Param([Parameter(Mandatory=$true)][string]$thePath,
          [bool]$clearIfExists = $false)

    if($clearIfExists -and (Test-Path -PathType Container -Path $thePath)) {
        $rs = Remove-Item $thePath -Recurse -Force
    }
            
    if(!(Test-Path -PathType Container -Path $thePath)) {
        $rs = New-Item -ItemType Directory -Force -Path $thePath
    }
}

#The new root dir of the docker images / files
$dockerRoot = "C:\Docker"

#the setting file needed for docker to override the storage location of images and logs.
$dockerSettingRoot = "C:\ProgramData\docker\config"
$dockerSetting = "$($dockerSettingRoot)\daemon.json"
$rebootScriptPath = "C:\temp\reboot_tasks"
$dockerDataRootTaskName = "PostInstalScript"

#####################################################
### Step Create script and run on reboot to 
###       set docker data root
#####################################################

ensureDir $dockerRoot
ensureDir $dockerSettingRoot
ensureDir $rebootScriptPath $true

#write the script body that will handle setting docker data root after reboot
Write-Host "Building script to change docker data root..."
$scriptFile = "$($rebootScriptPath)\PostInstalScript.ps1"

$scriptBody = @()
$scriptBody += "# Give docker time to start up"
$scriptBody += "`r`nStart-Sleep -s 180"
$scriptBody += "`r`n"
$scriptBody += "`r`n# move docker data-root"
$scriptBody += "`r`nnet stop docker"
$scriptBody += "`r`ndockerd --unregister-service"
$scriptBody += "`r`ndockerd --register-service --graph C:\Docker --experimental --storage-opt size=50GB"
$scriptBody += "`r`nnet start docker"
$scriptBody += "`r`n"
$scriptBody += "`r`nDisable-ScheduledTask -TaskName `"$($dockerDataRootTaskName)`""
$scriptBody += "`r`n"
$scriptBody += "`r`n# Build and start the containers"
#$scriptBody += "`r`nInvoke-Expression -Command C:\src\TaskScripts\1809\build_run_containers.ps1"
#$scriptBody += "`r`nStart-Sleep -s 180"
$scriptBody += "`r`n"
$scriptBody += "`r`n# Clean up the script"
$scriptBody += "`r`nRemove-Item -LiteralPath `"$($rebootScriptPath)`" -Force -Recurse"
$scriptBody += "`r`nStart-Sleep -s 10"
$scriptBody += "`r`n"

#Write the script file
writeScriptFile $scriptFile "$scriptBody"

#create scheduled reboot task
Write-Host "Created task to run script after reboot..."
addRebootScriptTask $scriptFile $dockerDataRootTaskName