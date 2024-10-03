using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;
using System;
using OrderManagement.OrderAPI.Models;

namespace OrderManagement.OrderAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
