# Main Branch Policy

## Цель

`main` должен принимать изменения только через Pull Request и только после успешного CI.

## Рекомендуемая настройка GitHub Ruleset для `main`

- Target branch: `main`
- Restrict direct updates: enabled
- Require pull request before merging: enabled
- Require approvals: at least 1
- Dismiss stale approvals: enabled
- Require approval of the most recent reviewable push: enabled
- Require conversation resolution before merging: enabled
- Require status checks to pass before merging: enabled
- Required checks:
  - `CI / build`
  - `CI / lint`
  - `CI / format-check`
  - `CI / type-check`
  - `CI / unit-tests`
  - `CI / smoke-tests`
- Require linear history: enabled
- Block force pushes: enabled
- Block branch deletion: enabled
- Do not allow bypassing the above settings: enabled

## Подготовленные артефакты

- JSON для классической branch protection API: `.github/branch-protection-main.json`
- PowerShell-скрипт применения политики: `scripts/apply-branch-protection.ps1`

## Важно

GitHub branch protection нельзя полноценно применить локально без удаленного GitHub-репозитория и прав доступа. В этом репозитории политика подготовлена в виде воспроизводимых файлов и команд для последующего применения.

