using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace admin.Models
{
    public partial class ProductType
    {
        public ProductType()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }

        [InverseProperty(nameof(Product.ProductType))]
        public virtual ICollection<Product> Products { get; set; }
    }
}
