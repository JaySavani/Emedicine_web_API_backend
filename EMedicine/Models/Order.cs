using System.ComponentModel.DataAnnotations.Schema;

namespace EMedicine.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Total { get; set; }
        public DateTime DateCreated { get; set; }
    }
}