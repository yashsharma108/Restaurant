using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Restaurant.Models.Orders;
using Restaurant.Models.Reservations;

namespace Restaurant.Models.Core
{
    public class Table
    {
        public Table()
        {
            Orders = new HashSet<Order>();
            Reservations = new HashSet<Reservation>();
            Status = TableStatus.Available;
            LastUpdated = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TableId { get; set; }

        [Required]
        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string TableNumber { get; set; }

        [Required]
        [Range(1, 20)]
        [Display(Name = "Seating Capacity")]
        public int Capacity { get; set; }

        [StringLength(200)]
        [Display(Name = "Location Description")]
        public string LocationDescription { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public TableStatus Status { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; }

        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }

        // Enum definition
        public enum TableStatus
        {
            Available,
            Occupied,
            Reserved,
            Maintenance,
            CleaningInProgress
        }

        // Computed properties
        [NotMapped]
        [Display(Name = "Current Status")]
        public string StatusDisplay => Status.ToString();

        [NotMapped]
        public bool IsAvailable => Status == TableStatus.Available;
    }
}