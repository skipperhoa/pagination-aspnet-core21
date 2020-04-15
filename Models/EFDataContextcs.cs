using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace PaginationASPCore.Models
{
    public class EFDataContextcs : DbContext
    {
        public EFDataContextcs(DbContextOptions<EFDataContextcs> options): base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(s => s.idProduct);
        }
        public DbSet<Product> Products { get; set; }
    }
}
