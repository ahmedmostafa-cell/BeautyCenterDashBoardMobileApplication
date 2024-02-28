using System.ComponentModel.DataAnnotations;
using JamalKhanah.Core.Helpers;

namespace JamalKhanah.Core.DTO.EntityDto;

public class ServiceDto
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
    public string Img { get; set; }

    //----------------------------------------
    [Display(Name = " السعر  ")]
    [Required(ErrorMessage = "السعر مطلوب")]
    [Range(1, 1000000, ErrorMessage = "السعر يجب ان يكون اكبر من 0")]
    public float Price { get; set; }

    [Display(Name = " الخصم بالنسبة المئوية  ")]
    public float Discount { get; set; } = 0;

    [Display(Name = " السعر بعد الخصم  ")]
    [Required(ErrorMessage = "السعر بعد الخصم مطلوب")]
    [Range(1, 1000000, ErrorMessage = "السعر بعد الخصم يجب ان يكون اكبر من 0")]
    public float FinalPrice { get; set; } = 0;

    [Display(Name = " وحدة السعر  ")]
    [Required(ErrorMessage = "وحدة السعر مطلوب")]
    public PriceUnite PriceUnit { get; set; }

    [Display(Name = " عدد الموظفين   ")]
    public int EmployeesNumber { get; set; } = 0;

    [Display(Name = "مدة الخدمة ")]
    [Required(ErrorMessage = "مدة الخدمة مطلوب")]
    public string Duration { get; set; }

    //-------------------------------------------
    [Display(Name = "في المنزل ")]
    [Required(ErrorMessage = "في المنزل مطلوب")]
    public bool InHome { get; set; } = false;

    [Display(Name = "في المركز ")]
    [Required(ErrorMessage = "في المركز مطلوب")]
    public bool InCenter { get; set; } = false;

    [Display(Name = "هل الخدمة متاحة الان ")]
    [Required(ErrorMessage = "هل الخدمة متاحة الان مطلوب")]
    public bool IsAvailable { get; set; } = true;

    //------------------------------------
    [Display(Name = "القسم الرئيسي")]
    [Required(ErrorMessage = "القسم الرئيسي مطلوب")]
    public int MainSectionId { get; set; }

}