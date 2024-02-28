using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JamalKhanah.Core.Entity.Other;

public class SlidePhoto : BaseEntity
{

    [Required(ErrorMessage = "يجب ادخال العنوان بالعربي  ")]
    [Display(Name = " العنوان بالعربي ")]
    public string TitleAr { get; set; }

    [Display(Name = " الصورة  ")] 
    public string ImgUrl { get; set; }
    public bool IsShow { get; set; } = true;


    [NotMapped]
    [Display(Name = " الصورة  ")]
    public IFormFile ImgFile { get; set; }

    public string TitleEn { get; set; }
    public string DescriptionAr { get; set; }
    public string DescriptionEn { get; set; }
}
