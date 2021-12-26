if ($IsMacOS) {
    $msbuild = "msbuild"
} else {
    $vswhere = 'C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe'
    $msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
    $msbuild = join-path $msbuild 'MSBuild\Current\Bin\MSBuild.exe'
}

#####################
#Build release config
$version="3.0.0"
$versionSuffix="-rc2"

del *.nupkg
& $msbuild "Vapolia.UserInteraction.sln" /restore /p:Configuration=Release /p:Platform="Any CPU" /p:Version="$version" /p:VersionSuffix="$versionSuffix" /p:Deterministic=false /p:PackageOutputPath="$PSScriptRoot" --% /t:Clean;Build;Pack
if ($lastexitcode -ne 0) { exit $lastexitcode; }

####################
# PUSH
#dotnet nuget push "Vapolia.UserInteraction*.nupkg" --interactive
copy "Vapolia.UserInteraction*.nupkg" ../../localnugets
