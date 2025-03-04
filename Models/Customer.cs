using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Book_Store.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Carts = new HashSet<Cart>();
            Orders = new HashSet<Order>();
        }

        public int CustomerId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your full name")]
        public string CustomerName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your email")]
        public string CustomerEmail { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your phone")]
        public string CustomerPhone { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your password")]
        public string CustomerPassword { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your address")]
        public string CustomerAddress { get; set; }
        public string Role { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
