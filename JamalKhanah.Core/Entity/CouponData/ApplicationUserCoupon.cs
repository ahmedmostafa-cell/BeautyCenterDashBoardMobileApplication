using System.ComponentModel.DataAnnotations.Schema;
using JamalKhanah.Core.Entity.ApplicationData;

namespace JamalKhanah.Core.Entity.CouponData;

public class ApplicationUserCoupon : BaseEntity
{
    [ForeignKey("Provider")]
    public string ProviderId { get; set; }
    public ApplicationUser Provider { get; set; }

    [ForeignKey("Coupon")]
    public int CouponId { get; set; }
    public Coupon Coupon { get; set; }
    
}