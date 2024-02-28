using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.DTO.EntityDto;

public class UpdateEmployeeDto
{
    [Required]
    public int Id { get; set; }
    [Required(ErrorMessage = "يجب أدخال الاسم "), StringLength(50), MinLength(5,ErrorMessage = "يجب أن يكون الاسم أكبر من 5 حروف")]
    [Display(Name = "الاسم بالكامل")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "يجب أدخال البريد"), StringLength(128)]
    [EmailAddress(ErrorMessage = "البريد الالكتروني غير صحيح")]
    [Display(Name = "البريد الالكتروني")]
    public string Email { get; set; }

    [Display(Name = "رقم الهاتف")]
    [DataType(DataType.PhoneNumber)]
    [Required(ErrorMessage = "يجب أدخال رقم الهاتف")]
    public string PhoneNumber { get; set; }

        
    [Display(Name = " الصورة  ")]
    public string ImgUrl { get; set; }
}