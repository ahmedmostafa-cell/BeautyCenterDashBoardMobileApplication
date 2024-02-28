using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.FavoriteData;

namespace JamalKhanah.Core.Entity.EvaluationData;

public class EvaluationProvider : Evaluation
{
    [ForeignKey("Provider")]
    [Display(Name = "اسم مقدم الخدمة  ")]
    public string ProviderId { get; set; }
    [Display(Name = "اسم مقدم الخدمة  ")]
    public virtual ApplicationUser Provider { get; set; }
}