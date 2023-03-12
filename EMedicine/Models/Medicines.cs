using System.ComponentModel.DataAnnotations.Schema;

namespace EMedicine.Models
{
    public class Medicines
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public string ImageUrl { get; set; }
        public DateTime ExpDate { get; set; }
        public Boolean Status { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        public string ImageSrc { get; set; }
    }
}
