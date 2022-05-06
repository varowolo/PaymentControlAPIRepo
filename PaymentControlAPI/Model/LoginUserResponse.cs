using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PaymentControlAPI.Model
{

    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
    }
    public class Permit
    {
        public bool Authorized { get; set; }
        public string Icons { get; set; }
        public string PageName { get; set; }
        public int roleId { get; set; }
    }
    public class LoginUserResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string token { get; set; }
        public User userinfo { get; set; }
        public IEnumerable<Permit> permissions { get; set; }
    }

}