using FileCreateWorkerService;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

IConfiguration Configuration = builder.Configuration;

builder.Services.AddDbContext<AdventureWorks2019Context>(opt =>
{
    opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton(sp =>
 new ConnectionFactory() { Uri = new Uri(Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });

builder.Services.AddSingleton<RabbitMQClientService>();


var host = builder.Build();
host.Run();
