

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace JamalKhanah.Core.Entity;

public class BaseEntity
{
    public int Id { get; set; }
    [Display(Name = "تاريخ الإنشاء")]
    [JsonIgnore]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [JsonIgnore]
    public bool IsUpdated { get; set; } = false;
    [Display(Name = "تاريخ أخر تحديث  ")]
    [JsonIgnore]
    public DateTime? UpdatedAt { get; set; } = null;
    [JsonIgnore]
    public bool IsDeleted { get; set; } = false;
    [JsonIgnore]
    public DateTime? DeletedAt { get; set; } = null;
}