param(
    $NuGetPackageUrl = '',
    $NuGetApiKey = '',
    $ReleaseVersion = '0.0.0'  
    )

# 
# TeamCity variables that may be set
# 

$BuildVersion = $ReleaseVersion
if ($env:BUILD_NUMBER -ne $null) {
    $BuildVersion = $env:BUILD_NUMBER
}

$BuildBranch = 'DevLocal'
if ($env:BUILD_BRANCH -ne $null) {
    $BuildBranch = $env:BUILD_BRANCH

    if ($BuildBranch -eq '<default>') {
        $BuildBranch = 'master'
    }
}   

$BuildIsPrivate = ($BuildBranch -ne 'master')

if ($NuGetPackageUrl -eq '' -and $env:NUGET_PACKAGE_URL -ne $null) {
    $NuGetPackageUrl = $env:NUGET_PACKAGE_URL
}

if ($NuGetApikey -eq '' -and $env:NUGET_API_KEY -ne $null) {
    $NuGetApiKey = $env:NUGET_API_KEY
}

if ($BuildIsPrivate -eq $true) {
    $NuGetVersion = "$BuildVersion-$BuildBranch"
}
else {
    $NuGetVersion = $BuildVersion
}

#
# Base directories
#
$IFSourceDirectory = Convert-Path $PWD
$BuildRootDir = "$IFSourceDirectory\Build"
$ToolsDir = "$IFSourceDirectory\tools"

# Nuget properties
$NuGetExe = "$BuildRootDir\nuget.exe"
$NuGetNuSpec = "$BuildRootDir\Default.Deploy.nuspec"

$ReleaseDir = "$IFSourceDirectory\release"
$BrokerOut = "$IFSourceDirectory\output\$BuildVersion\binaries\IronFoundry.ServiceBroker"

function CreateNuSpecs()
{
    Write-Host "Creating nuspec packages"
    mkdir $ReleaseDir -force
    & $NuGetExe pack "$NuGetNuSpec" -Version $NuGetVersion -Prop "Id=ironfoundry.brokerservice" -BasePath "$BrokerOut" -NoPackageAnalysis -NoDefaultExcludes -OutputDirectory "$ReleaseDir"
}

function NuGetPush 
{
    Write-Host "Pushing to nuget url: $NuGetPackageUrl"

    Get-ChildItem "$ReleaseDir\*.$NuGetVersion.nupkg" | ForEach-Object {
        . $NuGetExe push -Source $NuGetPackageUrl -ApiKey "$NuGetApiKey" "$($_.FullName)"
    }
}

if ($NuGetPackageUrl -ne '')
{
    CreateNuSpecs
    NuGetPush
}

Set-Location $IFSourceDirectory