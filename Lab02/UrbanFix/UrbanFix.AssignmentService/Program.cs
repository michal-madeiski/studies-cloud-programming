using MassTransit;
using Microsoft.EntityFrameworkCore;
using UrbanFix.AssignmentService.Consumers;
using UrbanFix.AssignmentService.Repository;
using UrbanFix.AssignmentService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AssignmentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConn")));

builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();

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

app.Run();
