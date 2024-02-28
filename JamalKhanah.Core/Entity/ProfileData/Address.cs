using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.Other;

namespace JamalKhanah.Core.Entity.ProfileData
{
    public class Address: BaseEntity
    {
        [Required(ErrorMessage = "يجب أدخال المنطقة "), StringLength(50)]
        [Display(Name = "المنطقة")]
        public string Region { get; set; }

        [Required(ErrorMessage = "يجب أدخال الشارع "), StringLength(50)]
        [Display(Name = "الشارع")]
        public string Street { get; set; }

        [Required(ErrorMessage = "يجب أدخال رقم المبني "), StringLength(50)]
        [Display(Name = "رقم المبني")]
        public string BuildingNumber { get; set; }

        [Required(ErrorMessage = "يجب أدخال رقم الشقة "), StringLength(50)]
        [Display(Name = "رقم الشقة")]
        public string FlatNumber { get; set; }

        [Display(Name = "تفاصيل أكثر عن العنوان")]
        public string AddressDetails{ get; set; }
        


        //----------------------------------------------------------------------------
        [ForeignKey("User")]
        [Display(Name = "المستخدم ")]
        [Required(ErrorMessage = "يجب أختيار المستخدم ")]
        public string UserId { get; set; }
        [Display(Name = "المستخدم")]
        public ApplicationUser User { get; set; }
        
        [ForeignKey("City")]
        [Display(Name = "المدينة")]
        [Required(ErrorMessage = "يجب أختيار المدينة ")]
        public int CityId { get; set; }
        [Display(Name = "المدينة")]
        public City City { get; set; }
        
    }
}
