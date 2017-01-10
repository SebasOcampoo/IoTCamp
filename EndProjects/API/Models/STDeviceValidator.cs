using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class STDeviceValidator : IDeviceValidator
    {
        public bool isValid(string id)
        {
            return true;
        }
    }
}