using System.ComponentModel.DataAnnotations;

namespace ReactBikes.Models
{
    public class Bike
    {
        [Key]
        public int BikeId { get; set; }
        [Display(Name = "Model")]
        public string? ModelName { get; set; }
        [Display(Name = "Colour")]
        public string? Colour { get; set; }
        [Display(Name = "Location")]
        public string? LocationPostcode { get; set; }
        [Range(0,5)]
        [Display(Name = "Average Rating")]
        public int Rating { get; set; }
        [Display(Name = "Availability")]
        public bool Available { get; set; }

    }
}
