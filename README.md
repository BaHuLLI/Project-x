# Dashbord

Безопасный .NET/WPF репозиторий для `ProjectXProDash` с воспроизводимыми локальными проверками и обязательным CI перед merge в `main`.

## Цели репозитория

- Любое изменение должно быть проверяемым и воспроизводимым.
- Изменения не должны попадать в `main`, если ломают сборку, форматирование, анализаторы или тесты.
- Изменения должны быть минимальными и локальными. Массовый рефакторинг запрещен без отдельного запроса.
- Публичные интерфейсы и архитектурные решения нельзя менять без отдельного согласования.

## Структура

```text
.
|-- .github/
|   |-- workflows/ci.yml
|   |-- PULL_REQUEST_TEMPLATE.md
|   `-- branch-protection-main.json
|-- .githooks/
|   |-- pre-commit
|   `-- pre-commit.ps1
|-- app/
|-- docs/
|   `-- main-branch-policy.md
|-- scripts/
|   `-- apply-branch-protection.ps1
|-- tests/
|   |-- ProjectXProDash.UnitTests/
|   `-- ProjectXProDash.SmokeTests/
|-- .editorconfig
|-- .gitignore
|-- CONTRIBUTING.md
|-- Dashbord.sln
|-- Directory.Build.props
`-- global.json
```

## Инструменты и guardrails

- `git` с `pre-commit` hooks через `.githooks/`
- `.NET SDK 8.0.419` через `global.json`
- встроенные .NET analyzers + `TreatWarningsAsErrors`
- `dotnet format` для format check и analyzer check
- `xUnit` для unit и smoke tests
- GitHub Actions для обязательного CI на `push` и `pull_request`

## Локальный workflow

1. Создайте ветку от `main`.
2. Внесите минимальные локальные изменения.
3. Запустите локальные проверки.
4. Откройте PR.
5. Merge допускается только после зеленого CI.

## Локальные команды проверки

```powershell
dotnet restore .\Dashbord.sln
dotnet format .\Dashbord.sln whitespace --verify-no-changes --no-restore
dotnet format .\Dashbord.sln style --verify-no-changes --no-restore
dotnet format .\Dashbord.sln analyzers --verify-no-changes --no-restore
dotnet build .\Dashbord.sln -c Release --no-restore
dotnet test .\tests\ProjectXProDash.UnitTests\ProjectXProDash.UnitTests.csproj -c Release --no-build
dotnet test .\tests\ProjectXProDash.SmokeTests\ProjectXProDash.SmokeTests.csproj -c Release --no-build
```

## Политика изменений

- Не менять публичные интерфейсы и архитектуру без отдельного согласования.
- Не делать массовый рефакторинг без явного запроса.
- Любой риск регрессии должен быть отражен в PR.
- Для `main` подготовлена политика защиты в `.github/branch-protection-main.json` и инструкция в `docs/main-branch-policy.md`.

