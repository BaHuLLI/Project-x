# Contributing

## Базовый процесс

- Работайте в отдельной ветке, а не в `main`.
- Перед переносом изменений в `main` запускайте локальные проверки.
- Держите изменения минимальными и локальными.
- Не меняйте публичные интерфейсы и архитектуру без отдельного согласования.

## Локальные проверки

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\local-check.ps1
```

## Рекомендуемый поток

```powershell
git checkout main
git pull
git checkout -b feature/my-change

# ...изменения...

powershell -ExecutionPolicy Bypass -File .\scripts\local-check.ps1

git add .
git commit -m "your message"
git checkout main
git merge feature/my-change
git push origin main
```
