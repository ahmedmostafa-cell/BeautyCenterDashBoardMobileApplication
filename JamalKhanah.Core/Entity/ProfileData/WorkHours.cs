using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Helpers;

namespace JamalKhanah.Core.Entity.ProfileData;

public class WorkHours : BaseEntity
{
    [Required (ErrorMessage = "يجب أختيار اليوم")]
    [Display(Name = "اليوم")]
    public Days Day { get; set; }

    [Required(ErrorMessage = "يجب أختيار الوقت من")]
    [Display(Name = "من")]
    [DataType(DataType.Time)]
    public TimeSpan From { get; set; }

    [Required(ErrorMessage = "يجب أختيار الوقت إلى")]
    [Display(Name = "إلى")]
    [DataType(DataType.Time)]
    public TimeSpan To { get; set; }

    [Display(Name = "البيانات")]
    public string MoreData { get; set; }


    [ForeignKey("User")]
    [Display(Name = "مقدم الخدمة")]
    [Required(ErrorMessage = "يجب أختيار مقدم الخدمة ")]
    public string UserId { get; set; }
    [Display(Name = "مقدم الخدمة")]
    public ApplicationUser User { get; set; }
}