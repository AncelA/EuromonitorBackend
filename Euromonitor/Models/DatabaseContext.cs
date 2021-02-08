using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Euromonitor.Models;

namespace Euromonitor.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Euromonitor.Models.Reseller> Reseller { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Book>().HasData(
        //        new Book
        //        {
        //            ID = 1,
        //            Name = "Harry Potter",
        //            Text = "The philosopher's stone",
        //            Price = 590
        //        }
        //    );
        //}
    }
}
