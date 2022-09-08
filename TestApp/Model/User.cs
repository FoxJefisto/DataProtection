using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace TestApp.Model
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool RootAccess { get; set; }
        public bool IsBanned { get; set; }
        public bool HasConstraint { get; set; }

        public User()
        {

        }
        public User(string login, string password, bool rootAccess = false)
        {
            Login = login;
            Password = password;
            RootAccess = rootAccess;
            IsBanned = false;
            HasConstraint = true;
        }

        public User(string login, string password, bool rootAccess, bool isBanned, bool hasConstraint)
        {
            Login = login;
            Password = password;
            RootAccess = rootAccess;
            IsBanned = isBanned;
            HasConstraint = hasConstraint;
        }
    }
}
