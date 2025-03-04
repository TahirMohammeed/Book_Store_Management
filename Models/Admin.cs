using System;
using System.Collections.Generic;

#nullable disable

namespace Book_Store.Models
{
    public partial class Admin
    {
        public int AdminId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
