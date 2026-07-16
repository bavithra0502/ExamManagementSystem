<<<<<<< HEAD
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
=======
# Exam Management API (.NET Core Web API)

ASP.NET Core 8 Web API + Entity Framework Core (SQL Server) backing the Angular
Exam Management screen, organized in a clean layered architecture:

```
Controllers/    -> thin HTTP endpoints, no business logic
Interfaces/     -> contracts for both Repositories and Services
Services/       -> validation + business rules (TotalMark, PassOrFail, uniqueness checks)
Repositories/   -> EF Core data access only, no validation
Models/         -> EF Core entities (SubjectMst, StudentMst, ExamMaster, ExamDtls)
DTOs/           -> request/response shapes used by the controllers
Data/           -> ExamDbContext (EF Core configuration + seed data)
Helpers/        -> custom exceptions (NotFoundException, ValidationException, ConflictException)
Middleware/     -> ExceptionHandlingMiddleware - turns those exceptions into the same
                   { "message": "..." } JSON responses the app already used, with the
                   matching status code (404 / 400 / 409)
Program.cs      -> DI registrations, CORS, middleware pipeline
```

### Request flow
```
Controller -> Service (validates + computes TotalMark/PassOrFail) -> Repository (EF Core) -> DB
```
If a Service throws `ValidationException`, `ConflictException`, or `NotFoundException`,
`ExceptionHandlingMiddleware` catches it and writes the exact same response shape the
original single-file controllers used to build directly - no behavior changed, only the
code organization.

## Setup

1. **Connection string** - update `appsettings.json` -> `ConnectionStrings:ExamManagementDB`.
2. **Create the database** - run the SQL script from the database zip, or:
   ```bash
   cd ExamManagementAPI
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
3. **Run the API**
   ```bash
   cd ExamManagementAPI
   dotnet restore
   dotnet run
   ```
   Swagger UI is available at `/swagger` in development.

## Endpoints (unchanged)

| Method | Route                     | Purpose                                              |
|--------|---------------------------|--------------------------------------------------------|
| GET    | `/api/Students?search=`   | Student list for the dropdown                          |
| GET    | `/api/Students/{id}`      | Single student                                          |
| POST   | `/api/Students`           | Create a student (validates name length & unique mail)  |
| GET    | `/api/Subjects?search=`   | Subject list for the dropdown                           |
| GET    | `/api/Exams`              | All saved exam records (with subject breakdown)          |
| GET    | `/api/Exams/{id}`         | Single exam record                                       |
| POST   | `/api/Exams`              | Save a new exam (ExamMaster + ExamDtls)                   |

### Business rules enforced (in `ExamService`, same as before)
- Student must exist.
- At least one subject/mark row is required, no duplicate subjects in one submission.
- Marks must be between 0 and 100.
- `StudentID` + `ExamYear` combination must be unique -> `409 Conflict`.
- `TotalMark` = sum of all submitted marks (calculated server-side).
- `PassOrFail` = `"PASS"` only if every subject's marks are `>= 25`, otherwise `"FAIL"`.
- All failures return `{ "message": "..." }` with the matching status code.
>>>>>>> b11544926bd2fe43d88939eedf05a0734ccd16c5
