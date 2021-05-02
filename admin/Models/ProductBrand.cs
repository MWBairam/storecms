using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace admin.Models
{
    public partial class ProductBrand
    {
        public ProductBrand()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }

        [InverseProperty(nameof(Product.ProductBrand))]
        public virtual ICollection<Product> Products { get; set; }
    }
}
