using ExamManagementAPI.Data;
using ExamManagementAPI.Interfaces;
using ExamManagementAPI.Middleware;
using ExamManagementAPI.Repositories;
using ExamManagementAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---- Services ----
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    }); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core - SQL Server
builder.Services.AddDbContext<ExamDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ExamManagementDB")));

// ---- Repository layer (data access only) ----
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();

// ---- Service layer (validation + business logic) ----
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IExamService, ExamService>();

// CORS - allow the Angular dev server to call this API
const string AngularAppCors = "AllowAngularApp";
var allowedOrigin = builder.Configuration["AllowedCorsOrigin"] ?? "http://localhost:4200";

builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularAppCors, policy =>
    {
        policy.WithOrigins(allowedOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ---- Middleware pipeline ----

// Placed first so it can catch anything thrown further down the pipeline
// (mainly the ValidationException/ConflictException/NotFoundException thrown by the Service layer).
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(AngularAppCors);

app.UseAuthorization();

app.MapControllers();

app.Run();
