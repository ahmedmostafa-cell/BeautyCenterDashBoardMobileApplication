using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.CouponData;
using JamalKhanah.Core.Entity.Other;
using JamalKhanah.Core.Entity.PaymentData;
using JamalKhanah.Core.Entity.SectionsData;
using JamalKhanah.Core.Helpers;


namespace JamalKhanah.Core.Entity.OrderData;

public class Order : BaseEntity
{
      
    [Display(Name = "رقم الطلب")]
    public string  OrderNumber { get; set; }

    [Display(Name = "مصاريف التوصيل ")]
    public float DeliveryFees { get; set; } = 0;
        
    [Display(Name = "الاجمالى")]
    public float Total { get; set; }

    [Display(Name = "الخصم")]
    public float Discount { get; set; }= 0;


[Display(Name = "طريقة الدفع")]
    public PaymentMethod PaymentMethod { get; set; }
        
    [Display(Name = "حالة الطلب")]
    public OrderStatus OrderStatus { get; set; }
        
    [Display(Name = "تم الدفع ")]
    public bool IsPaid { get; set; } = false;

    [Display(Name = "تاريخ الطلب")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime? CreatedOn { get; set; }
    [Display(Name = "تاريخ الانتهاء")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime? FinishedOn { get; set; }

    [Display(Name = "موعد التنفيذ")]
    [Required (ErrorMessage = " يجب ادخال موعد التنفيذ ")]
    public DateTime StartingOn { get; set; }

    public bool InHome { get; set; } = false;

    public bool HaveCoupon { get; set; } = false; 

    //--------------------------------------------------------
    [Required(ErrorMessage = "يجب أدخال المنطقة "), StringLength(50), MinLength(5,ErrorMessage = "يجب أن يكون العنوان أكبر من 5 حروف")]
    [Display(Name = "المنطقة")]
    public string Region { get; set; }

    [Required(ErrorMessage = "يجب أدخال الشارع "), StringLength(50), MinLength(5, ErrorMessage = "يجب أن يكون العنوان أكبر من 5 حروف")]
    [Display(Name = "الشارع")]
    public string Street { get; set; }

    [Required(ErrorMessage = "يجب أدخال رقم المبني "), StringLength(50), MinLength(5, ErrorMessage = "يجب أن يكون العنوان أكبر من 5 حروف")]
    [Display(Name = "رقم المبني")]
    public string BuildingNumber { get; set; }

    [Required(ErrorMessage = "يجب أدخال رقم الشقة "), StringLength(50), MinLength(5, ErrorMessage = "يجب أن يكون العنوان أكبر من 5 حروف")]
    [Display(Name = "رقم الشقة")]
    public string FlatNumber { get; set; }

    [Display(Name = "تفاصيل أكثر عن العنوان")]
    public string AddressDetails{ get; set; }

    [ForeignKey("City")]
    [Display(Name = "المدينة")]
    public int CityId { get; set; }
    [Display(Name = "المدينة")]
    public City City { get; set; }

     

    //--------------------------------------------------------
    [ForeignKey("Service")]
    [Display(Name = "الخدمة")]
    [Required(ErrorMessage = "الخدمة")]
    public int ServiceId { get; set; }
    [Display(Name = "اسم الخدمة")]
    public Service Service { get; set; }


    //-----------------------------------------
 
    [ForeignKey("User")]
    [Display(Name = "اسم العميل ")]
    public string UserId { get; set; }
    [Display(Name = "اسم العميل ")]
    public virtual ApplicationUser User { get; set; }

    [ForeignKey("Coupon")]
    public int? CouponId { get; set; }
    public Coupon Coupon { get; set; }


    public string PaymentUrlIdentifier { get; set; }
    public  ICollection<PaymentHistory> PaymentHistories { get; set; }

}