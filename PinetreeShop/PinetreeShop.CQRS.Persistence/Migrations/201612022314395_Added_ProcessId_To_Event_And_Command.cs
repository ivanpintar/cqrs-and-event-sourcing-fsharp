namespace PinetreeShop.CQRS.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_ProcessId_To_Event_And_Command : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommandEntity", "ProcessId", c => c.Guid(nullable: false));
            AddColumn("dbo.EventEntity", "ProcessId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EventEntity", "ProcessId");
            DropColumn("dbo.CommandEntity", "ProcessId");
        }
    }
}
