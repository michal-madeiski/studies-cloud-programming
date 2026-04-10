using MassTransit;
using Microsoft.EntityFrameworkCore;
using UrbanFix.NotificationService.Consumers;
using UrbanFix.NotificationService.Repository;
using UrbanFix.NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConn")));

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TaskAssignedEventConsumer>();
    x.AddConsumer<ReportVerifiedEventConsumer>();
    x.AddConsumer<TaskCompletedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitUrl = builder.Configuration.GetConnectionString("MqConn");
        cfg.Host(new Uri(rabbitUrl!));
        cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("notification", false));
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
