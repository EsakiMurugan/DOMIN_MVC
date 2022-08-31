using System.ComponentModel.DataAnnotations;

namespace DOMIN_MVC.Models
{
    public class Admin
    {
        [Key]
        [Display(Name = "Admin ID")]
        public int AdminID { get; set; }
        [Display(Name = "Admin Name")]
        public string? AdminName { get; set; }
        [Display(Name = "E-mail ID")]
        [DataType(DataType.EmailAddress)]
        public string? EmailID { get; set; }
        public string? Password { get; set; }
    }
}
