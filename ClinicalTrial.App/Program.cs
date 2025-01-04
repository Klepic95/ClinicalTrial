using ClinicalTrial.App.Extensions;
using ClinicalTrial.Application.Extensions;
using ClinicalTrial.Infrastructure.DependencyExtension;
using ClinicalTrial.Infrastructure.MSSqlDatabase;
using ClinicalTrial.Presentation;
using Microsoft.AspNetCore.Http.Features;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.AddSerilog();

builder.Services.AddAppExtensions();
builder.Services.RegisterInfrastructureDependencies(builder.Configuration.GetConnectionString("Default"));
builder.Services
    .AddControllers().AddApplicationPart(typeof(PresentationAssembly).Assembly)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddMediatR();

builder.Services.Configure<FormOptions>(options =>
{
    // This will set maximum file size constraints to 10MB
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});

var app = builder.Build();

app.Services.ApplyEFMigrations();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinical Trial API");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
