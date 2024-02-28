using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.ModelView.MV;

public class CommissionModelView
{
    public string Id { get; set; }

    [Display(Name = "اسم مقدم الخدمة  ")]
    public string Name { get; set; }
    [Display(Name = "صورة مقدم الخدمة  ")]
    public string Photo { get; set; }

    [Display(Name = "مجموع الطلبات ")]
    public float TotalOrder { get; set; }

    [Display(Name = "مجموع تكلفة الطلبات ")]
    public float TotalOrderAmount { get; set; }

    [Display(Name = "مجموع العمولات")]
    public double TotalCommission { get; set; }

    [Display(Name = "المبلغ المدفوع من الادمن ")]
    public float TotalPaid { get; set; }

    [Display(Name = "المبلغ المتبقي ")]
    public double TotalRemaining { get; set; }
}