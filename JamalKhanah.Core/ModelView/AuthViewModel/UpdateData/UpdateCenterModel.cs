using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace JamalKhanah.Core.ModelView.AuthViewModel.UpdateData;

public class UpdateCenterModel
{
    [Required (ErrorMessage = "يجب أدخال اسم المركز"), StringLength(50), MinLength(5)]
    [Display(Name = "اسم المركز")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "يجب أدخال البريد الالكتروني"), StringLength(128)]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "البريد الالكتروني غير صحيح")]
    [Display(Name = "البريد الالكتروني")]
    public string Email { get; set; }

    [Display(Name = "رقم الهاتف")]
    [DataType(DataType.PhoneNumber)]
    [Required(ErrorMessage = "يجب أدخال رقم الهاتف")]
    public string PhoneNumber { get; set; }
   
    [Required(ErrorMessage = "يجب أدخال الوصف "), StringLength(256), MinLength(5,ErrorMessage = "يجب علي الاقل أدخال 5 أحرف ")]
    [Display(Name = "الوصف")]
    public string Description { get; set; }
    
    [Display(Name = "الرقم الضريبي")]
    [DataType(DataType.PhoneNumber)]
    [Required(ErrorMessage = "يجب أدخال الرقم الضريب"), StringLength(50), MinLength(5,ErrorMessage = "يجب علي الاقل أدخال 5 أحرف ")]
    public string TaxNumber { get; set; }
    public string UserId { get; set; }

    [Display(Name = "المدينة")]
    [Required(ErrorMessage = "يجب أختيار المدينة")]
    public int CityId { get; set; }

    [Display(Name = "الصورة")]
    public IFormFile ImgFile { get; set; }

}
