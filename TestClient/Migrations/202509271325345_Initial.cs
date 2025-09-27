namespace TestClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        LastModified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OutboxMessages",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Type = c.String(nullable: false, maxLength: 100),
                        Data = c.String(nullable: false, maxLength: 2147483647),
                        CreatedAt = c.DateTime(nullable: false),
                        Processed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SyncInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastSyncTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SyncInfoes");
            DropTable("dbo.OutboxMessages");
            DropTable("dbo.Items");
        }
    }
}
