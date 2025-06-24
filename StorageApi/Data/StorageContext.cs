using Microsoft.EntityFrameworkCore;
using StorageApi.Models;
using System.Collections.Generic;

namespace StorageApi.Data
{
    public class StorageContext : DbContext
    {
        public StorageContext(DbContextOptions<StorageContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
