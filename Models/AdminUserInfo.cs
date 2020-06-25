using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LefeWareLearning.Tenants.Models
{
    public class AdminUserInfo
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string HashedPassword { get; set; }

        public AdminUserInfo(string adminFirstName, string adminLastName, string adminEmail, string adminHashedPassword)
        {
            FirstName = adminFirstName;
            LastName = adminLastName;
            Email = adminEmail;
            HashedPassword = adminHashedPassword;
        }


    }
}
