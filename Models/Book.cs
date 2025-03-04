using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Book_Store.Models
{
    public partial class Book
    {
        public Book()
        {
            Carts = new HashSet<Cart>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int BookId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select the category")]
        public int FkCategoryId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the book name")]
        public string BookName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the book price")]
        public double BookPrice { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the book quantity")]
        public int BookQty { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the book author")]
        public string BookAuthor { get; set; }
        public string BookImage { get; set; }
        [MaxLength(500, ErrorMessage = "The description is too long")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the Book description")]
        public string BookDesc { get; set; }
        [NotMapped]
        [DisplayName("Upload Image")]
        public IFormFile ImageFile { set; get; }

        public virtual Category FkCategory { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
