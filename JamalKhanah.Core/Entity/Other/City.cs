using System.ComponentModel.DataAnnotations;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.ProfileData;

namespace JamalKhanah.Core.Entity.Other
{
    public class City : BaseEntity
    {


        [StringLength(50)]
        [Display(Name = "اسم المدينة ")]
        [Required(ErrorMessage = "اسم المدينة مطلوب")]
        public string NameAr { get; set; }

        [StringLength(50)]
        [Display(Name = " اسم المدينة بالانجليزي")]
        [Required(ErrorMessage = "اسم المدينة بالانجليزي مطلوب")]
        public string NameEn { get; set; }

        [StringLength(50)]
        [Display(Name = " اسم الدولة")]
        public string CountryEn { get; set; } 

        [StringLength(50)]
        [Display(Name = " اسم الدولة بالانجليزي")]
        public string CountryAr { get; set; }

        public bool IsShow { get; set; } = true;


        public virtual IEnumerable<Address> Addresses { get; set; } = new List<Address>();
    }
}
