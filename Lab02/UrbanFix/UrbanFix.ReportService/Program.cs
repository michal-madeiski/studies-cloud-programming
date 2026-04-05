using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using UrbanFix.ReportService.Repository;
using UrbanFix.ReportService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<UrbanFixDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConn")));

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitUrl = builder.Configuration.GetConnectionString("MqConn");

        cfg.Host(new Uri(rabbitUrl!));

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
