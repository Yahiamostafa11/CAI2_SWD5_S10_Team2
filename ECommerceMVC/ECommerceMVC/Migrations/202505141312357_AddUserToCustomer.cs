namespace ECommerceMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserToCustomer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "ApplicationUserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "ApplicationUserId");
        }
    }
}
