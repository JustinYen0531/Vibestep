$ErrorActionPreference = 'Stop'

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$compilerPath = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$workPath = Join-Path $projectRoot 'work\live-test'
$testOutput = Join-Path $workPath 'Vibestep.LiveTest.exe'

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
    (Join-Path $projectRoot 'tests\LiveApiHarness.cs')
)

& $compilerPath @arguments
if ($LASTEXITCODE -ne 0) {
    throw 'API 測試程式編譯失敗。'
}

& $testOutput
if ($LASTEXITCODE -ne 0) {
    throw 'API 連線測試未通過。'
}

