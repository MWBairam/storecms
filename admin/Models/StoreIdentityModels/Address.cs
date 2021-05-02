using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace admin.Models.StoreIdentityModels
{
    [Table("Address")]
    [Index(nameof(AppUserId), Name = "IX_Address_AppUserId", IsUnique = true)]
    public partial class Address
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string AppUserId { get; set; }

        [ForeignKey(nameof(AppUserId))]
        [InverseProperty(nameof(AspNetUser.Address))]
        public virtual AspNetUser AppUser { get; set; }
    }
}
