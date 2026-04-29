using System;
using System.Collections.Generic;
using System.Text;

namespace CmsBackend.Domain.Dtos
{
    public class RegisterUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
