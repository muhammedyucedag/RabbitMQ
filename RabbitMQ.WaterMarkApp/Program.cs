using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.WaterMarkApp.Models;
using RabbitMQ.WaterMarkApp.Services;
using System.CodeDom;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase(databaseName: "product");
});


builder.Services.AddSingleton(sp => 
new ConnectionFactory() { Uri = new Uri(configuration.GetConnectionString("RabbitMQ")) });

builder.Services.AddSingleton<RabbitMQClientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
