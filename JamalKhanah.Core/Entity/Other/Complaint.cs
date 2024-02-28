using JamalKhanah.Core.Entity.ApplicationData;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JamalKhanah.Core.Entity.Other;

public class Complaint : BaseEntity
{
    [Display(Name = "عنوان الشكوي ")]
    [Required(ErrorMessage = "من فضلك ادخل عنوان الشكوي")]
    public string Title { get; set; }

    [Display(Name = "محتوي الشكوي ")]
    [Required(ErrorMessage = "من فضلك ادخل محتوي الشكوي")]
    public string Data { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; }

    [Display(Name = "اسم صاحب الشكوي ")]
    public virtual ApplicationUser User { get; set; }


}