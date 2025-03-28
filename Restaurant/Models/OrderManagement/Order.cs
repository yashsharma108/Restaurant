using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Restaurant.Models.Core;    // For Table
using Restaurant.Models.Users;  // For ApplicationUser
using Restaurant.Models.Orders; // For OrderDetail

namespace Restaurant.Models.Orders
{
    public class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Status = OrderStatus.Pending;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Required]
        [ForeignKey("Table")]
        public int TableId { get; set; }

        [ForeignKey("Staff")]
        public string StaffId { get; set; } // Links to IdentityUser

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "varchar(20)")] // Stores enum as string
        public OrderStatus Status { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 10000)]
        public decimal TotalAmount { get; set; }

        // Navigation properties
        public virtual Table Table { get; set; }
        public virtual ApplicationUser Staff { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        // Enum definition
        public enum OrderStatus
        {
            Pending,
            InProgress,
            ReadyToServe,
            Served,
            Completed,
            Cancelled,
            Refunded
        }

        // Computed property for UI
        [NotMapped]
        public string StatusDisplay => Status.ToString();
    }
}