using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace JamalKhanah.Core.ModelView.AuthViewModel.RegisterData;

public class RegisterAdminMv
{
    [Required(ErrorMessage = "يجب أدخال الاسم "), StringLength(50), MinLength(5,ErrorMessage = "يجب أن يكون الاسم أكبر من 5 حروف")]
    [Display(Name = "الاسم بالكامل")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "يجب أدخال البريد"), StringLength(128)]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "البريد الالكتروني غير صحيح")]
    [Display(Name = "البريد الالكتروني")]
    public string Email { get; set; }

    [Display(Name = "رقم الهاتف")]
    [DataType(DataType.PhoneNumber)]
    [Required(ErrorMessage = "يجب أدخال رقم الهاتف")]
    public string PhoneNumber { get; set; }

    [Required (ErrorMessage = "يجب أدخال كلمة السر "), StringLength(256)]
    [DataType(DataType.Password)]
    [Display(Name = "كلمة السر")]
    public string Password { get; set; }

    [Required(ErrorMessage = "يجب أدخال تأكيد كلمة السر "), StringLength(256)]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    [Display(Name = "تأكيد كلمة السر")]
    public string ConfirmPassword { get; set; }

    [Display(Name = "الصورة الشخصية")]
    public IFormFile Img { get; set; }





}
