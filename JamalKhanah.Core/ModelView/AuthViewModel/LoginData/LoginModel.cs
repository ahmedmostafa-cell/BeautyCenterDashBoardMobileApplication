using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.ModelView.AuthViewModel.LoginData;

public class LoginModel
{

    [Required(ErrorMessage = "يجب أدخال رقم الهاتف ")]
    [Display(Name = "Phone Number")]
    [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "يجب أدخال كلمة السر ")]
    [Display(Name = "Password")]
    public string Password { get; set; }
    public string DeviceToken { get; set; }

    public bool IsPersist { get; set; }
}