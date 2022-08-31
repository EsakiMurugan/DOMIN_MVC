using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOMIN_MVC.Models
{
    public class Receipt
    {
        [Key]
        public int ReceiptID { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ReceiptDate { get; set; }
        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public virtual Customer? customer { get; set; }
        public int? PizzaID { get; set; }
        [ForeignKey("PizzaID")]
        public virtual Pizza? pizza { get; set; }
        public int? Quantity { get; set; }
        [Display(Name = "Unit price")]
        public float? UnitPrice { get; set; }


    }
}
