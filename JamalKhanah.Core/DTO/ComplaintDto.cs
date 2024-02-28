
using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.DTO;

public class ComplaintDto
{
    [Display(Name = "عنوان الشكوي ")]
    [Required(ErrorMessage = "من فضلك ادخل عنوان الشكوي")]
    public string Title { get; set; }

    [Display(Name = "محتوي الشكوي ")]
    [Required(ErrorMessage = "من فضلك ادخل محتوي الشكوي")]
    public string Data { get; set; }
}
