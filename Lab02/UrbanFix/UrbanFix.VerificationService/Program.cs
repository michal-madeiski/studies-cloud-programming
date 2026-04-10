using MassTransit;
using Microsoft.EntityFrameworkCore;
using UrbanFix.VerificationService.Consumers;
using UrbanFix.VerificationService.Repository;
using UrbanFix.VerificationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<VerificationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConn")));

builder.Services.AddScoped<IVerificationRepository, VerificationRepository>();
builder.Services.AddScoped<IVerificationService, VerificationService>();

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

app.Run();
