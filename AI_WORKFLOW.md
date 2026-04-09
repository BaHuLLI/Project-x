# AI Workflow

## Working Rules

- Do not work directly in `main`.
- Start every task from a new branch created from `main`.
- Keep changes minimal and local.
- Do not change public interfaces or architecture without explicit approval.
- Do not do mass refactors unless explicitly requested.

## Required Local Validation

Run this after code changes:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\local-check.ps1
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
- any risk or follow-up the user should know about

