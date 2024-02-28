using System.ComponentModel.DataAnnotations;
using JamalKhanah.Core.Helpers;

namespace JamalKhanah.Core.DTO.EntityDto;

public class UpdateWorkHourDto
{
    [Required]
    public int Id { get; set; }
    [Required (ErrorMessage = "يجب أختيار اليوم")]
    [Display(Name = "اليوم")]
    public Days Day { get; set; }

    [Required(ErrorMessage = "يجب أختيار الوقت من")]
    [Display(Name = "من")]
    public DateTime From { get; set; }

    [Required(ErrorMessage = "يجب أختيار الوقت إلى")]
    [Display(Name = "إلى")]
    public DateTime To { get; set; }

    [Display(Name = "البيانات")]
    public string MoreData { get; set; }
}