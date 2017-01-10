namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedNameToMacAddressDeviceTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.STDevices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MacAddress = c.String(),
                        MessagesCount = c.Int(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.Devices");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Devices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        MessagesCount = c.Int(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.STDevices");
        }
    }
}
