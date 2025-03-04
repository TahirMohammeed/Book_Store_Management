using System;
using System.Collections.Generic;

#nullable disable

namespace Book_Store.Models
{
    public partial class Cart
    {
        public int CartId { get; set; }
        public int FkCustomerId { get; set; }
        public int FkBookId { get; set; }
        public int Qty { get; set; }
        public double TotalPrice { get; set; }

        public virtual Book FkBook { get; set; }
        public virtual Customer FkCustomer { get; set; }
    }
}
