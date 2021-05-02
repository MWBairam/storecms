using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace admin.Models
{
    [Index(nameof(ProductBrandId), Name = "IX_Products_ProductBrandId")]
    [Index(nameof(ProductTypeId), Name = "IX_Products_ProductTypeId")]
    public partial class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        [Required]
        public string PictureUrl { get; set; }
        public int ProductTypeId { get; set; }
        public int ProductBrandId { get; set; }

        [ForeignKey(nameof(ProductBrandId))]
        [InverseProperty("Products")]
        public virtual ProductBrand ProductBrand { get; set; }
        [ForeignKey(nameof(ProductTypeId))]
        [InverseProperty("Products")]
        public virtual ProductType ProductType { get; set; }
    }
}
