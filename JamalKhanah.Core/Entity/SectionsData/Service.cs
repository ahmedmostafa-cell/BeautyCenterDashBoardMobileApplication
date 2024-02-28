using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.EvaluationData;
using JamalKhanah.Core.Entity.FavoriteData;
using JamalKhanah.Core.Entity.OrderData;
using JamalKhanah.Core.Helpers;
using Microsoft.AspNetCore.Http;

namespace JamalKhanah.Core.Entity.SectionsData;

public class Service: BaseEntity
{
    [Required(ErrorMessage = "اسم الخدمة بالعربي مطلوب")]
    [Display(Name = "اسم الخدمة بالعربي")]
    public string TitleAr { get; set; }
    
    [Required(ErrorMessage = "اسم الخدمة بالانجليزي مطلوب")]
    [Display(Name = "اسم الخدمة بالانجليزي")]
    public string TitleEn { get; set; }

    [Required(ErrorMessage = "وصف الخدمة مطلوب")]
    [Display(Name = "وصف الخدمة ")]
    public string Description { get; set; }

    [Display(Name = " الصورة  ")]
    public string ImgUrl { get; set; }

    [Display(Name = "نوع مقدم الخدمة")]
    public ServiceType ServiceType { get; set; }

    //-----------------------------------------------------------------------

    [Display(Name = " السعر  ")]
    [Required(ErrorMessage = "السعر مطلوب")]
    [Range(1, 1000000, ErrorMessage = "السعر يجب ان يكون اكبر من 0")]
    public float Price { get; set; }

    [Display(Name = " الخصم بالنسبة المئوية  ")]
    public float Discount { get; set; } = 0;

    [Display(Name = " السعر بعد الخصم  ")]
    
    public float FinalPrice { get; set; } = 0;

    [Display(Name = " وحدة السعر  ")]
    [Required(ErrorMessage = "وحدة السعر مطلوب")]
    public PriceUnite PriceUnit { get; set; }

    [Display(Name = " عدد الموظفين   ")] 
    public int EmployeesNumber { get; set; } = 0;
    
    [Display(Name = "مدة الخدمة ")] 
    public string Duration { get; set; } 



    //-----------------------------------------------------------   




    [Display(Name = "تظهر في التطبيق")]
    public bool IsShow { get; set; } = true;
    [Display(Name = "متاحة أم لا")]
    public bool IsAvailable { get; set; } = true; // هل متاح ام لا

    [Display(Name = "في المنزل ")]
    public bool InHome { get; set; } = false;

    [Display(Name = "في المركز ")]
    public bool InCenter { get; set; } = false;

    [Display(Name = "متميزة أم لا")]
    public bool IsFeatured { get; set; } = false; // هل متميز ام لا
        


    //-------------------------------------------------------------------
    [ForeignKey("MainSection")]
    [Display(Name = "القسم الرئيسي")]
    [Required(ErrorMessage = "القسم الرئيسي مطلوب")]
    public int MainSectionId { get; set; }
    [Display(Name = "القسم الرئيسي")]
    public MainSection MainSection { get; set; }

    [ForeignKey("Provider")]
    [Display(Name = "مقدم الخدمة")]
    [Required(ErrorMessage = "مقدم الخدمة مطلوب")]
    public string ProviderId { get; set; }
    [Display(Name = "مقدم الخدمة")]
    public ApplicationUser Provider { get; set; }
    //--------------------------------------------------------------------
    [NotMapped]
    [Display(Name = " الصورة  ")]
    public IFormFile ImgFile { get; set; }

    //--------------------------------------------------------------------
    public IEnumerable<FavoriteService> FavoriteServices { get; set; } = new List<FavoriteService>();
    public IEnumerable<EvaluationService> EvaluationServices { get; set; } = new List<EvaluationService>();
    public IEnumerable<Order> Orders { get; set; } = new List<Order>();



}