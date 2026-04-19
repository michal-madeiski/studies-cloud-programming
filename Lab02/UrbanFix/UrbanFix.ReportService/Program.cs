using Amazon.Runtime;
using Amazon.S3;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UrbanFix.ReportService.Repository;
using UrbanFix.ReportService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UrbanFixDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConn")));

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IS3FileStorageService, S3FileStorageService>();

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitUrl = builder.Configuration.GetConnectionString("MqConn");

        cfg.Host(new Uri(rabbitUrl!));

        cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("report", false));
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var awsOptions = builder.Configuration.GetAWSOptions("AWS");

awsOptions.Credentials = new SessionAWSCredentials(
    builder.Configuration["AWS:AccessKey"],
    builder.Configuration["AWS:SecretKey"],
    builder.Configuration["AWS:SessionToken"]
);

builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonS3>();


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
