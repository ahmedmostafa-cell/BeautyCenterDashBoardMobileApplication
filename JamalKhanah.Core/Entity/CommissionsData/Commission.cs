using JamalKhanah.Core.Entity.ApplicationData;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using JamalKhanah.Core.Helpers;

namespace JamalKhanah.Core.Entity.CommissionsData;

public class Commission : BaseEntity
{
    [ForeignKey("Provider")]
    [Display(Name = "اسم مقدم الخدمة  ")]
    [Required(ErrorMessage = "يجب تحديد مقدم الخدمة ")]
    public string ProviderId { get; set; }
    [Display(Name = "اسم مقدم الخدمة  ")]
    public virtual ApplicationUser Provider { get; set; }

    [Required (ErrorMessage = "يجب تحديد المبلغ ")]
    [Display(Name = "المبلغ المدفوع من الادمن ")]
    public float Amount { get; set; }

    [Display(Name = "تاريخ الدفع ")]
    [Required(ErrorMessage = "يجب تحديد تاريخ الدفع ")]
    public DateTime Date { get; set; }

    [Display(Name = "الملاحظات ")]
    public string Notes { get; set; }

    [Display(Name = "طريقة الدفع ")]
    [Required(ErrorMessage = "يجب تحديد طريقة الدفع ")]
    public ProviderPaymentMethod PaymentMethod { get; set; }


}

public enum ProviderPaymentMethod
{
    [Display (Name = "نقدا ")]
    Cash,
    [Display (Name = "حوالة بنكية ")]
    Bank
}