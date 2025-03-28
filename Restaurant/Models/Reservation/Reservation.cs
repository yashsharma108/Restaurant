using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Restaurant.Models.Core; // For Table model

namespace Restaurant.Models.Reservations
{
    public class Reservation
    {
        public Reservation()
        {
            Status = ReservationStatus.Confirmed; // Default enum value
            CreatedOn = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReservationId { get; set; }

        [Required]
        [ForeignKey("Table")]
        public int TableId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string CustomerPhone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Email Address")]
        public string CustomerEmail { get; set; }

        [Required]
        [Range(1, 20)]
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Reservation Date")]
        [FutureDate(ErrorMessage = "Reservation date must be in the future")]
        public DateTime ReservationDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Reservation Time")]
        [ValidBusinessHours(ErrorMessage = "Reservation must be during business hours")]
        public TimeSpan ReservationTime { get; set; }

        [StringLength(500)]
        [Display(Name = "Special Requests")]
        public string SpecialRequests { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public ReservationStatus Status { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime? ModifiedOn { get; set; }

        // Navigation property
        public virtual Table Table { get; set; }

        // Computed property
        [NotMapped]
        public DateTime ReservationDateTime => ReservationDate.Add(ReservationTime);

        // Enum definition
        public enum ReservationStatus
        {
            Confirmed,
            Seated,
            Completed,
            Cancelled,
            NoShow
        }

        // Custom validation attribute for future dates
        public class FutureDateAttribute : ValidationAttribute
        {
            public override bool IsValid(object? value)
            {
                return value is DateTime date && date.Date >= DateTime.Today;
            }
        }

        // Custom validation attribute for business hours
        public class ValidBusinessHoursAttribute : ValidationAttribute
        {
            private static readonly TimeSpan OpeningTime = new TimeSpan(11, 0, 0); // 11 AM
            private static readonly TimeSpan ClosingTime = new TimeSpan(22, 0, 0);  // 10 PM

            public override bool IsValid(object value)
            {
                if (value is TimeSpan time)
                {
                    return time >= OpeningTime && time <= ClosingTime;
                }
                return false;
            }
        }
    }
}