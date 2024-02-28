using System.ComponentModel.DataAnnotations.Schema;
using JamalKhanah.Core.Entity.FavoriteData;
using JamalKhanah.Core.Entity.SectionsData;

namespace JamalKhanah.Core.Entity.EvaluationData;

public class EvaluationService : Evaluation
{

    [ForeignKey("Service")]
    public int ServiceId { get; set; }
    public Service Service { get; set; }
}