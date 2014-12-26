using System.Data.Entity.Migrations;

namespace PVWI.Migrations
{
    public partial class InicialTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CreditCardBillItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        DateTime = c.DateTime(nullable: false),
                        ItemValue = c.Double(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CreditCardBillItems", "UserId", "dbo.Users");
            DropIndex("dbo.CreditCardBillItems", new[] { "UserId" });
            DropTable("dbo.Users");
            DropTable("dbo.CreditCardBillItems");
        }
    }
}
