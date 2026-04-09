param(
  [Parameter(Mandatory = $true)]
  [string]$Owner,

  [Parameter(Mandatory = $true)]
  [string]$Repository
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$policyFile = Join-Path $PSScriptRoot "..\.github\branch-protection-main.json"
$resolvedPolicyFile = (Resolve-Path $policyFile).Path

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
  throw "GitHub CLI (gh) is required to apply branch protection."
}

gh api `
  --method PUT `
  --header "Accept: application/vnd.github+json" `
  "/repos/$Owner/$Repository/branches/main/protection" `
  --input $resolvedPolicyFile

