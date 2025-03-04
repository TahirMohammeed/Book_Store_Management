using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Store.Models
{
    public class CustomerCartItems
    {
        [Key]
        public int cartId { get; set; }
        public int customerId { get; set; }
        public int bookId { get; set; }
        [DisplayName("Book Name")]
        public string bookName { get; set; }
        [DisplayName("Book Price")]
        public double bookPrice { get; set; }
        [DisplayName("Quantity")]
        public int cartQty { get; set; }
        [DisplayName("Total Price")]
        public double bookTotalPrice { get; set; }

    }
}