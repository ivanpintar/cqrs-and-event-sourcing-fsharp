using PinetreeShop.CQRS.Persistence.SQL.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PinetreeShop.CQRS.Persistence.SQL
{
    public class EventStoreContext : DbContext
    {
        public EventStoreContext() : base("PinetreeShop_EventStoreContext")
        {

        }
        
        public DbSet<EventEntity> Events { get; set; }
        public DbSet<CommandEntity> Commands { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
