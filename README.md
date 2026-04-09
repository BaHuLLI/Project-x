# Dashbord

Простой solo-репозиторий для `ProjectXProDash`: работа идёт в отдельной ветке, потом локально проверяется, и только после этого изменения попадают в `main`.

## Как работать

1. Обновите `main`.
2. Создайте отдельную ветку под изменение.
3. Внесите минимальные локальные правки.
4. Прогоните локальные проверки.
5. Если всё зелёное, переносите изменения в `main`.

Пример:

```powershell
git checkout main
git pull
git checkout -b feature/my-change
```

После изменений:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\local-check.ps1
```

Если проверки прошли:

```powershell
git add .
git commit -m "your message"
git checkout main
git merge feature/my-change
git push origin main
```

## Что проверяется локально

- форматирование
- analyzer checks
- сборка
- unit tests
- smoke tests

Все эти шаги запускаются через `pre-commit` и через [local-check.ps1](L:/Dashbord/scripts/local-check.ps1).

## Правила

- Не менять публичные интерфейсы и архитектуру без отдельного согласования.
- Не делать массовый рефакторинг без явного запроса.
- Все изменения делать минимально и локально.

## Полезные файлы

- [AI_WORKFLOW.md](L:/Dashbord/AI_WORKFLOW.md) — короткая инструкция для ИИ и рабочего процесса
- [PROJECT_CONTEXT.md](L:/Dashbord/PROJECT_CONTEXT.md) — краткий контекст проекта и MVP-границы
