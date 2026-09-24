$ErrorActionPreference = 'Stop'

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
& (Join-Path $projectRoot 'build.ps1')

$compilerPath = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$workPath = Join-Path $projectRoot 'work\tests'
$testOutput = Join-Path $workPath 'Vibestep.Tests.exe'

New-Item -ItemType Directory -Force -Path $workPath | Out-Null

$arguments = @(
    '/nologo',
    '/target:exe',
    '/codepage:65001',
    "/out:$testOutput",
    '/reference:System.dll',
    '/reference:System.Core.dll',
    '/reference:System.Net.Http.dll',
    '/reference:System.Web.Extensions.dll',
    (Join-Path $projectRoot 'src\Models.cs'),
    (Join-Path $projectRoot 'src\CommandSafety.cs'),
    (Join-Path $projectRoot 'src\OpenAiCommandExplainer.cs'),
    (Join-Path $projectRoot 'tests\TestHarness.cs')
)

& $compilerPath @arguments
if ($LASTEXITCODE -ne 0) {
    throw '測試程式編譯失敗。'
}

& $testOutput
if ($LASTEXITCODE -ne 0) {
    throw '測試未通過。'
}

Write-Output '所有本機檢查均已通過。'

