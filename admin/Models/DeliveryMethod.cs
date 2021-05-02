using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace admin.Models
{
    public partial class DeliveryMethod
    {
        public DeliveryMethod()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string DeliveryTime { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        [InverseProperty(nameof(Order.DeliveryMethod))]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
