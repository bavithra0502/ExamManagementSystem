# Exam Management App (Angular)

A single-screen Angular 20 (standalone, zoneless) app for recording exam results.

## Features
- Autofill (type-ahead) student selector.
- Exam Year textbox.
- Autofill (type-ahead) subject selector + Marks textbox, with an "Add Subject"
  button that appends a row to an on-screen table (repeat for multiple subjects).
- Live running **Total Mark**.
- **Save** posts `ExamMaster` + `ExamDtls` to the Web API in one call.
- After saving, the "Saved Exam Records" table (below the form) refreshes and
  shows every student's saved exam info, including subject-wise marks,
  total, and PASS/FAIL.

## Project layout
```
src/app/
  exams/exam-entry/     The one screen: entry form + saved records table
  models/               Student, Subject, ExamDtlRow, ExamResult
  services/exam-service.ts   All HTTP calls to the API
```

## Setup

1. Point the API URL at your running backend in
   `src/environments/environment.ts` (defaults to `https://localhost:7069/api/`,
   matching the included .NET Web API project).

2. Install dependencies and run:
   ```bash
   npm install
   npm start
   ```
   The app opens at `http://localhost:4200`.

> Note: the backend must be running with CORS allowing `http://localhost:4200`
> (already configured in the API's `Program.cs` / `appsettings.json`).
