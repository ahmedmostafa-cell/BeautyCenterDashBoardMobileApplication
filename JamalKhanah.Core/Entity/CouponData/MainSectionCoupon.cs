using JamalKhanah.Core.Entity.SectionsData;
using System.ComponentModel.DataAnnotations.Schema;

namespace JamalKhanah.Core.Entity.CouponData;

public class MainSectionCoupon : BaseEntity
{
    [ForeignKey("MainSection")]
    public int MainSectionId { get; set; }
    public MainSection MainSection { get; set; }
    [ForeignKey("Coupon")]
    public int CouponId { get; set; }
    public Coupon Coupon { get; set; }

}