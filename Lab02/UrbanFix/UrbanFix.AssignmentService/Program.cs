using MassTransit;
using Microsoft.EntityFrameworkCore;
using UrbanFix.AssignmentService.Consumers;
using UrbanFix.AssignmentService.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AssignmentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConn")));

builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ReportVerifiedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitUrl = builder.Configuration.GetConnectionString("MqConn");
        cfg.Host(new Uri(rabbitUrl!));
        cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("assignment", false));
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
    var dbContext = scope.ServiceProvider.GetRequiredService<AssignmentDbContext>();
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
