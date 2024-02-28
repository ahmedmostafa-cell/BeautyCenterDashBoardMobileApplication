using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace JamalKhanah.Core.ModelView.AuthViewModel.UpdateData;

public class UpdateFreeAgentMv
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

    public float? Lat { get; set; }
    public float? Lng { get; set; }
    
    public string FreelanceFormImg { get; set; }

    public string Img { get; set; }



}
