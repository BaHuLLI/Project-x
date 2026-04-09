Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = (git rev-parse --show-toplevel).Trim()
$scriptPath = Join-Path $repoRoot "scripts\local-check.ps1"

& powershell -NoProfile -ExecutionPolicy Bypass -File $scriptPath

