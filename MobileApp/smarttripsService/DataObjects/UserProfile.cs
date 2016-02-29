using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace smarttripsService.DataObjects
{
    public class UserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUri { get; set; }
        public byte[] ProfilePicture { get; set; }
    }
}