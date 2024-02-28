using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using JamalKhanah.Core.Entity.ApplicationData;

namespace JamalKhanah.Core.Entity.ProfileData;

public class Prize : BaseEntity
{
    [Required(ErrorMessage = "يجب أدخال عنوان الجائزة")]
    [Display(Name = "عنوان الجائزة")]
    public string Title { get; set; }
    [Required(ErrorMessage = "يجب أدخال وصف الجائزة")]
    [Display(Name = "وصف الجائزة")]
    public string Description { get; set; }

    [ForeignKey("User")]
    [Display(Name = "مقدم الخدمة")]
    [Required(ErrorMessage = "يجب أختيار مقدم الخدمة ")]
    public string UserId { get; set; }
    [Display(Name = "مقدم الخدمة")]
    public ApplicationUser User { get; set; }
}