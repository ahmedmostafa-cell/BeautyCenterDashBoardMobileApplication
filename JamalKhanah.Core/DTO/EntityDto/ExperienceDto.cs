using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.DTO.EntityDto;

public class ExperienceDto
{
    [Required(ErrorMessage = "يجب أدخال عنوان الخبرة")]
    [Display(Name = "عنوان الخبرة")]
    public string Title { get; set; }
    [Required(ErrorMessage = "يجب أدخال وصف الخبرة")]
    [Display(Name = "وصف الخبرة")]
    public string Description { get; set; }
}