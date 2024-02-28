using System.ComponentModel.DataAnnotations.Schema;
using JamalKhanah.Core.Entity.SectionsData;

namespace JamalKhanah.Core.Entity.FavoriteData;

public class FavoriteService : Favorite
{

    [ForeignKey("Service")]
    public int ServiceId { get; set; }
    public Service Service { get; set; }
}