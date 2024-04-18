using DistantionEducationWebApi.Implementations;
using DistantionEducationWebApi.Queries;
using DistantionEducationWebApi.Repositories;
using DistantionEducationWebApi.Services;
using Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add queries
builder.Services.AddSingleton<StudentWorksQueries>();

// Add repositories
builder.Services.AddTransient<StudentWorksRepository>();

// Add services
builder.Services.AddTransient<IStudentWorksService, StudentWorksService>();
builder.Services.AddTransient<ICheckWorkService, CheckWorkServcie>();
builder.Services.AddTransient<ITextAnalisysService, TextAnalisysService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
