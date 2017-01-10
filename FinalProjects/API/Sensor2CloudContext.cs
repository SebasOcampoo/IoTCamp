namespace API
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Linq;

    public class Sensor2CloudContext : DbContext
    {
        // Your context has been configured to use a 'Sensor2CloudContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'API.Sensor2CloudContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Sensor2CloudContext' 
        // connection string in the application configuration file.
        public Sensor2CloudContext()
            : base("name=Sensor2CloudContext")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<STDevice> Devices { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public DbGeography Location { get; set; }
        public DateTime Timestamp { get; set; }
        public string DeviceMacAddress { get; set; }
    }

    [Table("Devices")]
    public class STDevice
    {
        public int Id { get; set; }
        public string MacAddress { get; set; }
        public int MessagesCount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}