namespace API.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;
    using System.Globalization;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Sensor2CloudContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Sensor2CloudContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            context.Users.AddOrUpdate(
                l => l.Id,
                new User { Location = CreateLocation(45.064537, 7.660219), Timestamp = DateTime.Now, DeviceMacAddress = "Mac1" },
                new User { Location = CreateLocation(41.899632, 12.487785), Timestamp = DateTime.Now, DeviceMacAddress = "Mac2" }
            );

            context.Devices.AddOrUpdate(
                l => l.Id,
                new STDevice { MacAddress = "Name1", MessagesCount = 3, Timestamp = DateTime.Now },
                new STDevice { MacAddress = "Name2", MessagesCount = 5, Timestamp = DateTime.Now }
            );
        }
        
        private DbGeography CreateLocation(double latitude, double longitude)
        {
            var text = string.Format(CultureInfo.InvariantCulture.NumberFormat,"POINT({0} {1})", longitude, latitude);

            // 4326 is most common coordinate system used by GPS/Maps
            return DbGeography.PointFromText(text, 4326);
        }
    }
}
