using JamalKhanah.Core.Entity.EvaluationData;
using JamalKhanah.Core.Entity.FavoriteData;
using JamalKhanah.Core.Entity.Other;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.OrderData;
using JamalKhanah.Core.Entity.SectionsData;

namespace JamalKhanah.Core.Entity.CouponData;

public class Coupon : BaseEntity
{
    [Display(Name = "كود الكوبون")]
    [Required (ErrorMessage = " يجب ادخال كود الكوبون ")]
    public string CouponCode { get; set; }

    [Display(Name = " الخصم")]
    [Required (ErrorMessage = " يجب ادخال الخصم ")]
    public float Discount { get; set; }

    
    [Display(Name = "تاريخ الانتهاء")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime? ExpireDate { get; set; }

    [Display(Name = "نوع الخصم")]
    public DiscountType DiscountType { get; set; }

    [Display(Name = "نوع الكوبون")]
    public CouponType CouponType { get; set; }

    [Display(Name = "هل مفعل ؟ ")]
	public bool IsActive { get; set; } = true;

    //------------------------------------------------------------------------

    public ICollection<MainSectionCoupon> MainSections { get; set; } = new List<MainSectionCoupon>(); 
    public ICollection<ServiceCoupon> Services { get; set; } = new List<ServiceCoupon>(); 
    public ICollection<ApplicationUserCoupon> Providers { get; set; } = new List<ApplicationUserCoupon>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    //---------------------------------------------------------------------------

    [NotMapped]
    [Display(Name = "أختر الاقسام ")]
    public List<int> MainSectionsId { get; set; } = new List<int>();

    [NotMapped]
    [Display(Name = "أختر الخدمات ")]
	public List<int> ServicesId { get; set; } = new List<int>();

	[NotMapped]
	[Display(Name = "أختر مقدمي الخدمات  ")]
	public List<string> ProvidersId { get; set; } = new List<string>();






	//---------------------------------------------------------------------------




}
public  enum DiscountType
{
    [Display(Name = "نسبة مئوية")]
    Percentage,
    [Display(Name = "كاش")]
    Cash,
   
   
}

public enum CouponType
{
    [Display(Name = "أختر نوع الكارت ")]
    None,
    [Display(Name = "فئات")]
    MainSections ,
    [Display(Name = "مقدم خدمة ")]
    ServiceProvider,

	[Display(Name = "خدمات")]
    Service,
 
}