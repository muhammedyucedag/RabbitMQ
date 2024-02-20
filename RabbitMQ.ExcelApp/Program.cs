using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.ExcelApp.Entity;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

        builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
        {
            opt.User.RequireUniqueEmail = true;

        }).AddEntityFrameworkStores<ApplicationDbContext>();

        var app = builder.Build();

        using (var scoped = app.Services.CreateScope())
        {
            var dbContext = scoped.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userManager = scoped.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            dbContext.Database.Migrate();    

            if (!userManager.Users.Any())
            {
                userManager.CreateAsync(new IdentityUser()
                {
                    UserName = "muhammed",
                    Email = "muhammedyucedag@outlook.com"
                }, "Password12*").Wait();

                userManager.CreateAsync(new IdentityUser()
                {
                    UserName = "cem",
                    Email = "cemyucedag@outlook.com"
                }, "Password12*").Wait();
            }
        }

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

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}