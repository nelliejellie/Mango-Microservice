using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ShoppingCart.Models
{
    public class CartHeader
    {
        [Key]
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }

        [NotMapped]  // This won't be stored in the database
        public double Discount { get; set; }
        [NotMapped]
        public double CartTotal { get; set; }
    }
}
