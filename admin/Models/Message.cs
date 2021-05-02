using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace admin.Models
{
    public partial class Message
    {
        [Key]
        public int Id { get; set; }
        [Column("firstName")]
        public string FirstName { get; set; }
        [Column("lastName")]
        public string LastName { get; set; }
        [Column("address")]
        public string Address { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("message")]
        public string Message1 { get; set; }
    }
}
