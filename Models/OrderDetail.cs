using System;
using System.Collections.Generic;

#nullable disable

namespace Book_Store.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int FkOrderId { get; set; }
        public int FkBookId { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }

        public virtual Book FkBook { get; set; }
        public virtual Order FkOrder { get; set; }
    }
}
