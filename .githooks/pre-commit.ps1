Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$solution = ".\Dashbord.sln"
$unitTests = ".\tests\ProjectXProDash.UnitTests\ProjectXProDash.UnitTests.csproj"
$smokeTests = ".\tests\ProjectXProDash.SmokeTests\ProjectXProDash.SmokeTests.csproj"

function Run-Step {
  param(
    [Parameter(Mandatory = $true)]
    [string]$Name,

    [Parameter(Mandatory = $true)]
    [scriptblock]$Action
  )

  Write-Host "==> $Name" -ForegroundColor Cyan
  & $Action
}

$repoRoot = (git rev-parse --show-toplevel).Trim()
Push-Location $repoRoot

try {
  Run-Step "restore" { dotnet restore $solution }
  Run-Step "format-whitespace" { dotnet format $solution whitespace --verify-no-changes --no-restore }
  Run-Step "format-style" { dotnet format $solution style --verify-no-changes --no-restore }
  Run-Step "lint" { dotnet format $solution analyzers --verify-no-changes --no-restore }
  Run-Step "build" { dotnet build $solution -c Release --no-restore }
  Run-Step "unit-tests" { dotnet test $unitTests -c Release --no-build }
  Run-Step "smoke-tests" { dotnet test $smokeTests -c Release --no-build }
}
finally {
  Pop-Location
}

