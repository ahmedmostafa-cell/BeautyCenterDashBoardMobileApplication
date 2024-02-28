using JamalKhanah.Core.Entity.ApplicationData;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JamalKhanah.Core.Entity.FavoriteData;

public class FavoriteProvider : Favorite
{
    [ForeignKey("Provider")]
    [Display(Name = "اسم مقدم الخدمة  ")]
    public string ProviderId { get; set; }
    [Display(Name = "اسم مقدم الخدمة  ")]
    public virtual ApplicationUser Provider { get; set; }
}