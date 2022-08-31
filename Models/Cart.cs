using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOMIN_MVC.Models
{
    public class Cart
    {
        [Key]
        [Display(Name = "Cart ID")]
        public int? CartID { get; set; }
        [Display(Name = "Carttype ID")]
        public string? CartTypeID { get; set; }
        public int? PizzaID { get; set; }
        [ForeignKey("PizzaID")]
        public virtual Pizza? pizza { get; set; }
        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public virtual Customer? customer { get; set; }
        public int? Quantity { get; set; }
        [Display(Name = "Unit price")]
        public float? UnitPrice { get; set; }

    }
}
