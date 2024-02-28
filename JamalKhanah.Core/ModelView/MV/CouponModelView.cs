using JamalKhanah.Core.Entity.CouponData;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JamalKhanah.Core.ModelView.MV;

public class CouponModelView
{
	public int Id { get; set; }

	[Display(Name = "كود الكوبون")]
	
	public string CouponCode { get; set; }

	[Display(Name = " الخصم")]
	
	public float Discount { get; set; }

	[Display(Name = "نوع الخصم")]
	public DiscountType DiscountType { get; set; }

	[Display(Name = "نوع الكوبون")]
	public CouponType CouponType { get; set; }

	[Display(Name = "هل مفعل ؟ ")]
	public bool IsActive { get; set; } = true;

	[Display(Name = " عدد الطلبات المستخدم فيها")]
	 public int TotalOrderUsed { get; set; }
}