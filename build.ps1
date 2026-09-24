$ErrorActionPreference = 'Stop'

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$compilerPath = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$sourcePath = Join-Path $projectRoot 'src'
$artifactPath = Join-Path $projectRoot 'artifacts'
$outputPath = Join-Path $artifactPath 'Vibestep.exe'

if (-not (Test-Path -LiteralPath $compilerPath)) {
    throw '找不到 Windows 內建編譯器。'
}

New-Item -ItemType Directory -Force -Path $artifactPath | Out-Null

$sourceFiles = Get-ChildItem -LiteralPath $sourcePath -Filter '*.cs' | Sort-Object Name | ForEach-Object FullName
$arguments = @(
    '/nologo',
    '/target:winexe',
    '/optimize+',
    '/codepage:65001',
    "/out:$outputPath",
    '/reference:System.dll',
    '/reference:System.Core.dll',
    '/reference:System.Drawing.dll',
    '/reference:System.Net.Http.dll',
    '/reference:System.Security.dll',
    '/reference:System.Web.Extensions.dll',
    '/reference:System.Windows.Forms.dll'
) + $sourceFiles

& $compilerPath @arguments
if ($LASTEXITCODE -ne 0) {
    throw 'Vibestep 編譯失敗。'
}

Write-Output "完成：$outputPath"

