namespace PVWI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Atualizacaodasentidadesecriacaodatabelafaturas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CreditCardBills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BillPaymentDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.CreditCardBillItems", "CreditCardBillId", c => c.Int(nullable: false));
            CreateIndex("dbo.CreditCardBillItems", "CreditCardBillId");
            AddForeignKey("dbo.CreditCardBillItems", "CreditCardBillId", "dbo.CreditCardBills", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CreditCardBillItems", "CreditCardBillId", "dbo.CreditCardBills");
            DropIndex("dbo.CreditCardBillItems", new[] { "CreditCardBillId" });
            DropColumn("dbo.CreditCardBillItems", "CreditCardBillId");
            DropTable("dbo.CreditCardBills");
        }
    }
}
