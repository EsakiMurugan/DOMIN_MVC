using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOMIN_MVC.Models
{
    public class Customer
    {
        [Key]
        [Display(Name = "Customer ID")]
        public int CustomerID { get; set; }

        [Display(Name = "Customer Name")]
        [Required(ErrorMessage = "*Mandatory field")]
        [RegularExpression(@"^[A-Z][\sA-Z]*$", ErrorMessage = "Customer Name Invalid")]
       
        public string? CustomerName { get; set; }

        [Display(Name = "Mobile Number")]
        [Required(ErrorMessage = "*Mandatory field")]
        [RegularExpression(@"^[6-9]{1}[0-9]{9}$",ErrorMessage ="Enter Valid Number")]
        public long MobileNumber { get; set; }

        [Display(Name = "E-mail ID")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "*Mandatory field")]
        public string? EmailID { get; set; }

        [Required(ErrorMessage = "*Mandatory field")]
        public string? Address { get; set; }

        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
           ErrorMessage = "Password must contains one Uppercase,one Lowercase and one Specialcharacter")]
        [Required(ErrorMessage = "*Mandatory field")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [NotMapped]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password do not match.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string? CPassword { get; set; }

        [Display(Name = "Carttype ID")]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? CartTypeID { get; set; }
        public List<Cart>? cart { get; set; }
        public List<Payment>? Payment { get; set; }
        public List<Receipt>? Receipt { get; set; }
    }
}
