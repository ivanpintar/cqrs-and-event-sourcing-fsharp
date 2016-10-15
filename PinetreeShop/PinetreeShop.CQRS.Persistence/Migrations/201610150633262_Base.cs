namespace PinetreeShop.CQRS.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Base : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommandEntity",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AggregateId = c.Guid(nullable: false),
                        CommandId = c.Guid(nullable: false),
                        CausationId = c.Guid(nullable: false),
                        CorrelationId = c.Guid(nullable: false),
                        AggregateRootType = c.String(),
                        CommandPayload = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventEntity",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AggregateId = c.Guid(nullable: false),
                        EventId = c.Guid(nullable: false),
                        CausationId = c.Guid(nullable: false),
                        CorrelationId = c.Guid(nullable: false),
                        EventPayload = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EventEntity");
            DropTable("dbo.CommandEntity");
        }
    }
}
