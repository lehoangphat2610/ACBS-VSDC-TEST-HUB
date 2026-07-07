$ErrorActionPreference = 'Stop'
$root = Resolve-Path (Join-Path $PSScriptRoot '..')
$project = Join-Path $root 'src\Acbs.Vsdc.TestHub\Acbs.Vsdc.TestHub.csproj'
$output = Join-Path $root 'publish\DCRSM_UAT_win-x64_framework-dependent'
$config = Join-Path $root 'deploy-configs\DCRSM_UAT'

if (Test-Path $output) { Remove-Item $output -Recurse -Force }

dotnet restore $project
dotnet publish $project -c Release -r win-x64 --self-contained false -o $output

Copy-Item (Join-Path $config 'appsettings.json') (Join-Path $output 'appsettings.json') -Force
if (Test-Path (Join-Path $output 'Config')) {
    Copy-Item (Join-Path $config 'Config.appsettings.database.json') (Join-Path $output 'Config\appsettings.database.json') -Force
    Copy-Item (Join-Path $config 'Config.appsettings.gateway.json') (Join-Path $output 'Config\appsettings.gateway.json') -Force
    Copy-Item (Join-Path $config 'Config.appsettings.hosting.json') (Join-Path $output 'Config\appsettings.hosting.json') -Force
}

Write-Host "Published framework-dependent package to: $output"
Write-Host "Server must have the matching .NET ASP.NET Core Runtime installed."
