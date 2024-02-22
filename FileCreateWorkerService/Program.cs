using FileCreateWorkerService;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

IConfiguration Configuration = builder.Configuration;

builder.Services.AddDbContext<AdventureWorks2019Context>(opt =>
{
    opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<RabbitMQClientService>();

builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });

builder.Services.AddHostedService<Worker>();


var host = builder.Build();
host.Run();
