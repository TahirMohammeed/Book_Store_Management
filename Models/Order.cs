using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Book_Store.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public int FkCustomerId { get; set; }
        public double TotalPrice { get; set; }
        public string OrderDate { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your full name")]
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your phone number")]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the complete address detail")]
        [DisplayName("Address Detail")]
        public string AddressDetail { get; set; }
        public int OrderStatus { get; set; }

        public virtual Customer FkCustomer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
