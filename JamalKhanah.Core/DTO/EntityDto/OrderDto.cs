
using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.DTO.EntityDto;

public class OrderDto
{
    [Display(Name = "موعد التنفيذ")]
    [Required (ErrorMessage = " يجب ادخال موعد التنفيذ ")]
    public DateTime StartingOn { get; set; }
    
    [Display(Name = "الخدمة")]
    [Required(ErrorMessage = "الخدمة")]
    public int ServiceId { get; set; }

    [Display(Name = "العنوان")]
    [Required(ErrorMessage = "العنوان")]
    public int AddressId { get; set; }

    public bool InHome { get; set; }= false;
}