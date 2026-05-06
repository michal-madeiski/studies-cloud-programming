var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("https://localhost:7201/swagger/v1/swagger.json", "Reports API");
    options.SwaggerEndpoint("https://localhost:7202/swagger/v1/swagger.json", "Verifications API");
    options.SwaggerEndpoint("https://localhost:7203/swagger/v1/swagger.json", "Assignments API");
    options.SwaggerEndpoint("https://localhost:7204/swagger/v1/swagger.json", "Timelines API");
    options.SwaggerEndpoint("https://localhost:7205/swagger/v1/swagger.json", "Notifications API");
    options.RoutePrefix = string.Empty;
});

app.MapReverseProxy();

app.Run();