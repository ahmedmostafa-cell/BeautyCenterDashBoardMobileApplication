using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.DTO.EntityDto;

public class PrizeDto
{
    [Required(ErrorMessage = "يجب أدخال عنوان الجائزة")]
    [Display(Name = "عنوان الجائزة")]
    public string Title { get; set; }
    [Required(ErrorMessage = "يجب أدخال وصف الجائزة")]
    [Display(Name = "وصف الجائزة")]
    public string Description { get; set; }
}