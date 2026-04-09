# Contributing

## Базовые правила

- Работать только через feature-ветки.
- Не пушить напрямую в `main`.
- Merge в `main` выполнять только через Pull Request.
- Merge допускается только после успешного CI.
- Изменения должны быть минимальными и локальными.
- Массовый рефакторинг запрещен без отдельного запроса.
- Публичные интерфейсы и архитектуру нельзя менять без отдельного согласования.

## Минимальный набор локальных проверок

```powershell
dotnet restore .\Dashbord.sln
dotnet format .\Dashbord.sln whitespace --verify-no-changes --no-restore
dotnet format .\Dashbord.sln style --verify-no-changes --no-restore
dotnet format .\Dashbord.sln analyzers --verify-no-changes --no-restore
dotnet build .\Dashbord.sln -c Release --no-restore
dotnet test .\tests\ProjectXProDash.UnitTests\ProjectXProDash.UnitTests.csproj -c Release --no-build
dotnet test .\tests\ProjectXProDash.SmokeTests\ProjectXProDash.SmokeTests.csproj -c Release --no-build
```

## Перед открытием PR

- Обновить или добавить тесты на затронутый критичный функционал.
- Описать, что изменено и что потенциально может сломаться.
- Отдельно отметить любые изменения контрактов, если на них было получено согласование.

