namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGeolocationTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GeoLocations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Location = c.Geography(),
                        Timestamp = c.DateTime(nullable: false),
                        DeviceMacAddress = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GeoLocations");
        }
    }
}
