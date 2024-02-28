using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.DTO.EntityDto;

public class OrderCouponDto
{
	[Required]
	public int OrderId { get; set; }
	[Required]
	public string CouponCode { get; set; }

}