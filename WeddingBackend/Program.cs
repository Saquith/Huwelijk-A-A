using System.ComponentModel.DataAnnotations;
using Serilog;
using WeddingBackend.Models;
using WeddingBackend.Services;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/weddingbackend-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
    .CreateLogger();

try
{
    Log.Information("Starting up WeddingBackend");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog
    builder.Host.UseSerilog();

    // Configuration: Smtp settings come from appsettings.json with sensitive values (Password) injected from secrets/env.
    builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
    builder.Services.AddSingleton<SmtpEmailSender>();

    // CORS (restrict in production)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllLocal", policy =>
        {
            policy.AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowAnyOrigin();
        });
    });

    var app = builder.Build();

    // Serilog request logging (structured)
    app.UseSerilogRequestLogging();

    app.UseCors("AllowAllLocal");

    app.MapPost("/api/contact", async (ContactRequest req, SmtpEmailSender sender, ILogger<Program> logger) =>
    {
        // Honeypot anti-spam check
        if (!string.IsNullOrWhiteSpace(req.Hp))
        {
            logger.LogWarning("Contact submission rejected: honeypot filled.");
            return Results.BadRequest(new { error = "spam detected" });
        }

        // Server-side validation using DataAnnotations
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(req, serviceProvider: null, items: null);
        if (!Validator.TryValidateObject(req, ctx, validationResults, validateAllProperties: true))
        {
            var errors = validationResults
                .SelectMany(r => r.MemberNames.DefaultIfEmpty("").Select(m => new { Member = m, Error = r.ErrorMessage }))
                .GroupBy(x => x.Member)
                .ToDictionary(g => g.Key == "" ? "Request" : g.Key, g => g.Select(x => x.Error).ToArray());

            logger.LogInformation("Contact submission failed validation: {@Errors}", errors);
            return Results.ValidationProblem(errors);
        }

        try
        {
            await sender.SendEmailAsync(req);
            logger.LogInformation("Contact email sent from {Email} ({Name})", req.Email, req.Name);
            return Results.Ok(new { message = "sent" });
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "Failed to send contact email");
            return Results.StatusCode(500, new { error = "Failed to send message. Please try again later." });
        }
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}