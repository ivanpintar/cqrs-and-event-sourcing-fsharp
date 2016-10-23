using PinetreeShop.Domain.Products.ReadModel.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PinetreeShop.Domain.Products.ReadModel
{
    public class ProductContext : DbContext
    {
        public ProductContext() : base("PinetreeShop_ProductContext")
        {

        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
