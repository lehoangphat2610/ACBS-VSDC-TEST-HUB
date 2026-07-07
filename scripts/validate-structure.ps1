param(
    [string]$Root = (Resolve-Path (Join-Path $PSScriptRoot ".."))
)

$ErrorActionPreference = "Stop"
$project = Join-Path $Root "src/Acbs.Vsdc.TestHub"

Write-Host "Validating JSON configuration..."
Get-ChildItem $project -Recurse -Filter *.json | ForEach-Object {
    Get-Content $_.FullName -Raw | ConvertFrom-Json | Out-Null
}

Write-Host "Validating project XML..."
[xml](Get-Content (Join-Path $project "Acbs.Vsdc.TestHub.csproj") -Raw) | Out-Null

$program = Get-Content (Join-Path $project "Program.cs")
if ($program.Count -gt 20) {
    throw "Program.cs is too large ($($program.Count) lines). Keep it as bootstrap only."
}

$hardCoded = Get-ChildItem $project -Recurse -Filter *.cs |
    Select-String -SimpleMatch "\\CSD\" -ErrorAction SilentlyContinue
if ($hardCoded) {
    throw "UNC gateway path was found inside C# source. Move it to Config/*.json."
}

$csFiles = @(Get-ChildItem $project -Recurse -Filter *.cs)
$folders = @(Get-ChildItem $project -Recurse -Directory)
$samples = @(Get-ChildItem (Join-Path $Root "samples/MSP/Actual") -Recurse -File -ErrorAction SilentlyContinue)

Write-Host "C# files : $($csFiles.Count)"
Write-Host "Folders  : $($folders.Count)"
Write-Host "Samples  : $($samples.Count)"
Write-Host "Structural validation completed. Run dotnet build for compiler validation."
