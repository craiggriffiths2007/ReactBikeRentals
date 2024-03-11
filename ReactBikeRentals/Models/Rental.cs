using ReactBikes.Data;
using System.ComponentModel.DataAnnotations;

namespace ReactBikes.Models
{
    public class Rental
    {
        [Key]
        public int RentalId { get; set; }
        public string UserID { get; set; }
        public int? BikeID { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Date From")]
        public DateTime DateFrom { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Date To")]
        public DateTime DateTo { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Date Returned")]
        public DateTime DateReturned { get; set; }
        [Required]
        public bool Returned {  get; set; }
        [Required]
        [Range(1, 5)]
        [Display(Name = "My Rating")]
        public int Rating { get; set; }
        [Required]
        public string Comments { get; set; }
        public Bike? Bike { get; set; }
        public ReactBikesUser? User { get; set; }
    }
}
