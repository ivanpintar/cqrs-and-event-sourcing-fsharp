using PinetreeShop.Domain.Orders.ReadModel.Entitites;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PinetreeShop.Domain.Orders.ReadModel
{
    public class OrderContext : DbContext
    {
        public OrderContext() : base("PinetreeShop_OrderContext")
        {

        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Order>().HasMany(x => x.Lines).WithRequired();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
