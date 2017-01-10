namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeGeoLocationToUser : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.GeoLocations", newName: "Users");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Users", newName: "GeoLocations");
        }
    }
}
