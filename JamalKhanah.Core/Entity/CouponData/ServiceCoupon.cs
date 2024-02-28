using JamalKhanah.Core.Entity.SectionsData;
using System.ComponentModel.DataAnnotations.Schema;

namespace JamalKhanah.Core.Entity.CouponData;

public class ServiceCoupon : BaseEntity
{

    [ForeignKey("Service")]
    public int ServiceId { get; set; }
    public Service Service { get; set; }
    [ForeignKey("Coupon")]
    public int CouponId { get; set; }
    public Coupon Coupon { get; set; }
}