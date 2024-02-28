using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.ModelView.AuthViewModel.LoginData;

public class ConfirmSms
{
    [Required(ErrorMessage = "رقم التحقق مطلوب")]
    [Display(Name = "رقم التحقق")]
    public string RandomCode { get; set; }
        
    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    [StringLength(50)]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }
}