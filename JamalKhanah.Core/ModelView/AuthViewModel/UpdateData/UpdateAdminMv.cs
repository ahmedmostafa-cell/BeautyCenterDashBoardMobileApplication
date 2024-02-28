using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace JamalKhanah.Core.ModelView.AuthViewModel.UpdateData;

public class UpdateAdminMv
{
    [StringLength(50), MinLength(5)]
    [Display(Name = "الاسم الكامل")]
    public string FullName { get; set; }

    [Required, StringLength(128)]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "البريد الالكتروني غير صحيح")]
    [Display(Name = "البريد الالكتروني")]
    public string Email { get; set; }

    [Required, StringLength(128)]
    [DataType(DataType.PhoneNumber)]
    [Display(Name = "رقم الهاتف")]
    public string PhoneNumber { get; set; }
    
    

    public string UserId { get; set; }
    public IFormFile Img { get; set; }


}