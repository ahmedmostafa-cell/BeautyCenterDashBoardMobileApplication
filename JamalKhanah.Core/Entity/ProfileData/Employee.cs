
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using JamalKhanah.Core.Entity.ApplicationData;
using Microsoft.AspNetCore.Http;

namespace JamalKhanah.Core.Entity.ProfileData
{
    public class Employee: BaseEntity
    {

        [Required(ErrorMessage = "يجب أدخال الاسم "), StringLength(50), MinLength(5,ErrorMessage = "يجب أن يكون الاسم أكبر من 5 حروف")]
        [Display(Name = "الاسم بالكامل")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "يجب أدخال البريد"), StringLength(128)]
        [EmailAddress(ErrorMessage = "البريد الالكتروني غير صحيح")]
        [Display(Name = "البريد الالكتروني")]
        public string Email { get; set; }

        [Display(Name = "رقم الهاتف")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "يجب أدخال رقم الهاتف")]
        public string PhoneNumber { get; set; }

        
        [Display(Name = " الصورة  ")]
        public string ImgUrl { get; set; }
        
        

        [ForeignKey("User")]
        [Display(Name = "مقدم الخدمة")]
        [Required(ErrorMessage = "يجب أختيار المستخدم ")]
        public string UserId { get; set; }
        [Display(Name = "مقدم الخدمة")]
        public ApplicationUser User { get; set; }

        //----------------------------------
        [NotMapped]
        [Display(Name = " الصورة  ")]
        public IFormFile ImgFile { get; set; }
    }
}
