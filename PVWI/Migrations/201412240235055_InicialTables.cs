// --------------------------------------------------------------------------------------------------------------------
// <copyright file="201412240235055_InicialTables.cs" company="PVWI Family">
//   Todos os direitos reservados.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace PVWI.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// The inicial tables.
    /// </summary>
    public partial class InicialTables : DbMigration
    {
        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            CreateTable(
                "dbo.CreditCardBillItems", 
                c => new
                    {
                        Id = c.Int(false, true), 
                        Description = c.String(), 
                        DateTime = c.DateTime(false), 
                        ItemValue = c.Double(false), 
                        UserId = c.Int(false)
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users", 
                c => new
                    {
                        Id = c.Int(false, true), 
                        Name = c.String(false), 
                        Email = c.String(false)
                    })
                .PrimaryKey(t => t.Id);
            
        }

        /// <summary>
        /// The down.
        /// </summary>
        public override void Down()
        {
            DropForeignKey("dbo.CreditCardBillItems", "UserId", "dbo.Users");
            DropIndex("dbo.CreditCardBillItems", new[] { "UserId" });
            DropTable("dbo.Users");
            DropTable("dbo.CreditCardBillItems");
        }
    }
}
