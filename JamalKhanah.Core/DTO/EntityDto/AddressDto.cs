using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.DTO.EntityDto;

public class AddressDto
{
    [Required(ErrorMessage = "يجب أدخال المنطقة "), StringLength(50)]
    [Display(Name = "المنطقة")]
    public string Region { get; set; }

    [Required(ErrorMessage = "يجب أدخال الشارع "), StringLength(50)]
    [Display(Name = "الشارع")]
    public string Street { get; set; }

    [Required(ErrorMessage = "يجب أدخال رقم المبني "), StringLength(50)]
    [Display(Name = "رقم المبني")]
    public string BuildingNumber { get; set; }

    [Required(ErrorMessage = "يجب أدخال رقم الشقة "), StringLength(50)]
    [Display(Name = "رقم الشقة")]
    public string FlatNumber { get; set; }

    [Display(Name = "تفاصيل أكثر عن العنوان")]
    public string AddressDetails{ get; set; }
    [Display(Name = "المدينة")]
    [Required(ErrorMessage = "يجب أختيار المدينة ")]
    public int CityId { get; set; }
}