using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace smarttripsService.DataObjects
{
    public class User : EntityData
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public List<string> Devices { get; set; }
    }
}