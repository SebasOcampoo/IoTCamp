using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class UserInputDto
    {
        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public string DeviceMacAddress { get; set; }
    }
}