namespace TestMaster.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialOutbox : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OutboxMessages");
        }
    }
}
