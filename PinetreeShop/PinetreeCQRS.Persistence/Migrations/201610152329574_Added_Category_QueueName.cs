namespace PinetreeCQRS.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Category_QueueName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommandEntity", "QueueName", c => c.String());
            AddColumn("dbo.EventEntity", "Category", c => c.String());
            DropColumn("dbo.CommandEntity", "AggregateRootType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CommandEntity", "AggregateRootType", c => c.String());
            DropColumn("dbo.EventEntity", "Category");
            DropColumn("dbo.CommandEntity", "QueueName");
        }
    }
}
