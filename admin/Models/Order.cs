using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace admin.Models
{
    [Index(nameof(DeliveryMethodId), Name = "IX_Orders_DeliveryMethodId")]
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        [Key]
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        [Column(TypeName = "timestamp with time zone")]
        public DateTime OrderDate { get; set; }
        [Column("ShipToAddress_FirstName")]
        public string ShipToAddressFirstName { get; set; }
        [Column("ShipToAddress_LastName")]
        public string ShipToAddressLastName { get; set; }
        [Column("ShipToAddress_Street")]
        public string ShipToAddressStreet { get; set; }
        [Column("ShipToAddress_City")]
        public string ShipToAddressCity { get; set; }
        [Column("ShipToAddress_State")]
        public string ShipToAddressState { get; set; }
        [Column("ShipToAddress_Zipcode")]
        public string ShipToAddressZipcode { get; set; }
        public int? DeliveryMethodId { get; set; }
        public decimal Subtotal { get; set; }
        [Required]
        public string Status { get; set; }
        public string PaymentIntentId { get; set; }

        [ForeignKey(nameof(DeliveryMethodId))]
        [InverseProperty("Orders")]
        public virtual DeliveryMethod DeliveryMethod { get; set; }
        [InverseProperty(nameof(OrderItem.Order))]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
