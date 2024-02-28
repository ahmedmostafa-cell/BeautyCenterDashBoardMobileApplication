using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace JamalKhanah.Core.ModelView.AuthViewModel.UpdateData;

public class UpdateCenterMv
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
    [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
    [Required(ErrorMessage = "يجب أدخال الرقم الضريب"), StringLength(50), MinLength(5,ErrorMessage = "يجب علي الاقل أدخال 5 أحرف ")]
    public string TaxNumber { get; set; }


    public float? Lat { get; set; }
    public float? Lng { get; set; }
    public string Img { get; set; }

 
    public IFormFile ImgFile { get; set; }

}
