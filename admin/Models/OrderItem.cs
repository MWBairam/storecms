using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace admin.Models
{
    [Table("OrderItem")]
    [Index(nameof(OrderId), Name = "IX_OrderItem_OrderId")]
    public partial class OrderItem
    {
        [Key]
        public int Id { get; set; }
        [Column("ItemOrdered_ProductItemId")]
        public int? ItemOrderedProductItemId { get; set; }
        [Column("ItemOrdered_ProductName")]
        public string ItemOrderedProductName { get; set; }
        [Column("ItemOrdered_PictureUrl")]
        public string ItemOrderedPictureUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int? OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        [InverseProperty("OrderItems")]
        public virtual Order Order { get; set; }
    }
}
