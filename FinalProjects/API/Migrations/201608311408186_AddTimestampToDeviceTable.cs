namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTimestampToDeviceTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "Timestamp", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Devices", "Timestamp");
        }
    }
}
