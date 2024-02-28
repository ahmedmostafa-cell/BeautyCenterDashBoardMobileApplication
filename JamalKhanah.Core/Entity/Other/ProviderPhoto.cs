using JamalKhanah.Core.Entity.ApplicationData;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JamalKhanah.Core.Entity.Other;

public class ProviderPhoto : BaseEntity
{

    [ForeignKey("Provider")]
    [Display(Name = "اسم مقدم الخدمة  ")]
    public string ProviderId { get; set; }

    [Display(Name = "اسم مقدم الخدمة  ")]
    public virtual ApplicationUser Provider { get; set; }



    [Display(Name = " الصورة  ")] 
    public string ImgUrl { get; set; }


    [NotMapped]
    [Display(Name = " الصورة  ")]
    public IFormFile ImgFile { get; set; }

}

  
