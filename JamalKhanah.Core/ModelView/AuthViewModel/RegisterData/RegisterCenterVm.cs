using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.ModelView.AuthViewModel.RegisterData;

public class RegisterCenterVm
{
    [Required, StringLength(50), MinLength(5)]
    public string FullName { get; set; }

    [Required, StringLength(128)]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "البريد الالكتروني غير صحيح")]
    public string Email { get; set; }

    [Display(Name = "رقم الهاتف")]
    [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
    [Required]
    public string PhoneNumber { get; set; }

    [Required, StringLength(256)]
    [Display(Name = "الوصف")]
    public string Description { get; set; }


    [Required, StringLength(256)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required, StringLength(256)]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [Display(Name = "الرقم الضريبي")]
    [DataType(DataType.PhoneNumber)]
    [Required, StringLength(50), MinLength(5)]
    public string TaxNumber { get; set; }

    public float? Lat { get; set; }
    public float? Lng { get; set; }

    [Required]
    [Display(Name = "المدينة")]
    public int CityId { get; set; }

    [Required]
    public string Img { get; set; } // user Img base64

}
