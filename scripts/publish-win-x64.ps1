$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '..\src\Acbs.Vsdc.TestHub\Acbs.Vsdc.TestHub.csproj'
$output = Join-Path $PSScriptRoot '..\publish\win-x64'
dotnet restore $project
dotnet publish $project -c Release -r win-x64 --self-contained false -o $output
Write-Host "Published to $output"
