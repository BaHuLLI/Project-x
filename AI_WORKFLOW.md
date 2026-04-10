# AI Workflow

## Working Rules

- Do not work directly in `main`.
- Start every task from a new branch created from `main`.
- Always keep changes in the new branch until the user explicitly says to push or merge into `main`.
- Never push directly to `main` on your own.
- Never merge into `main` on your own.
- Only the user decides whether changes should go to `main`.
- Keep changes minimal and local.
- Do not change public interfaces or architecture without explicit approval.
- Do not do mass refactors unless explicitly requested.
- If the task changes workflow, project context, setup instructions, or usage guidance, update the relevant repository docs as part of the same change.

## Required Local Validation

Run this after code changes:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\local-check.ps1
```

After successful validation, always close any already running old app instance and start the current build with:

```powershell
Stop-Process -Name ProjectXProDash -Force -ErrorAction SilentlyContinue
cmd /c .\app\build-and-run.bat
```

If validation fails:

- fix the issue first
- rerun validation
- do not propose merging into `main` until checks pass

## Normal Flow

```powershell
git checkout main
git pull
git checkout -b feature/my-change
```

Make the change, then run:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\local-check.ps1
```

If checks pass:

```powershell
Stop-Process -Name ProjectXProDash -Force -ErrorAction SilentlyContinue
cmd /c .\app\build-and-run.bat
```

Then:

```powershell
git add .
git commit -m "your message"
```

Then either:

```powershell
git checkout main
git merge feature/my-change
git push origin main
```

or leave the branch ready for the user to merge manually.

## What To Report Back

After finishing work, report:

- what changed
- whether local checks passed
- whether the old app instance was closed and `app\build-and-run.bat` was launched
- any risk or follow-up the user should know about
- whether any repository instruction or context files were updated

