using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.ModelView.AuthViewModel.UpdateData;

public class UpdateUserMv
{
    [Required,StringLength(50), MinLength(5)]
    public string FullName { get; set; }

    [Required, StringLength(128)]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "البريد الالكتروني غير صحيح")]
    public string Email { get; set; }

    [Required, StringLength(128)]
    [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
    [Display(Name = "رقم الهاتف")]
    public string PhoneNumber { get; set; }
    
    [Display(Name = "المدينة")]
    [Required(ErrorMessage = "يجب أختيار المدينة")]
    public int CityId { get; set; }
    public float? Lat { get; set; }
    public float? Lng { get; set; }
    public string Img { get; set; }

    



}