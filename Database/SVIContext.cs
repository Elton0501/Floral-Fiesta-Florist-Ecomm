using Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace SV.Database
{
    public class SVIContext : DbContext
    {
        public SVIContext() : base("SVIDB")
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<Newsletter> Newsletter { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Testemonial> Testemonials { get; set; }
        public DbSet<Notification> Notification { get; set; }
    }
}
