using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JamalKhanah.Core.Entity.ApplicationData;

namespace JamalKhanah.Core.Entity.FavoriteData;

public class Favorite : BaseEntity

{
    [ForeignKey("User")]
    [Display(Name = "اسم العميل ")]
    public string UserId { get; set; }
    [Display(Name = "اسم العميل ")]
    public virtual ApplicationUser User { get; set; }

  


}
