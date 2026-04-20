$buildFilePath = "$PSScriptRoot\BuildNumber.txt"
$currentBuildNumber = [int](Get-Content $buildFilePath)
$currentBuildNumber++
$currentBuildNumber | Set-Content $buildFilePath
Write-Host "##vso[task.setvariable variable=BuildNumber]$currentBuildNumber"
