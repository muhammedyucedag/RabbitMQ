﻿using Microsoft.EntityFrameworkCore;

namespace RabbitMQ.WaterMarkApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
