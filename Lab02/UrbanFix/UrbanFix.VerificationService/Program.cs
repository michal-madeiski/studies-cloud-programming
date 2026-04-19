using MassTransit;
using Microsoft.EntityFrameworkCore;
using UrbanFix.VerificationService.Consumers;
using UrbanFix.VerificationService.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<VerificationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConn")));

builder.Services.AddScoped<IVerificationRepository, VerificationRepository>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ReportCreatedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitUrl = builder.Configuration.GetConnectionString("MqConn");
        cfg.Host(new Uri(rabbitUrl!));
        cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("verification", false));
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<VerificationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        if (dbContext.Database.CanConnect())
        {
            logger.LogInformation("--- AWS RDS CONNECTION SUCCESS ---");
            logger.LogInformation($"Connected to database: {dbContext.Database.GetDbConnection().Database}");
        }
        else
        {
            logger.LogWarning("--- AWS RDS CONNECTION FAILED ---");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error while connecting to database.");
    }
}

app.Run();
