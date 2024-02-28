using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.Entity.Other;

public class ContactUs
{
    public int Id { get; set; }

    [Display(Name = "رقم الواتس اب ")]
    public string WhatsAppNumber { get; set; }

    [Display(Name = "الايميل ")]
    public string Email { get; set; }

    [Display(Name = "لينك الموقع ")]
    public string Link { get; set; }

    [Display(Name = "لينك الفيسبوك ")]
    public string FaceBookLink { get; set; }

    [Display(Name = "رقم الهاتف ")]
    public string PhoneNumber { get; set; }


    [Display(Name = "سياسة الخصوصية")]
    public string TermsAndConditions { get; set; }

    [Display(Name = "سياسة الخصوصية عربي")]
    public string TermsAndConditionsAr { get; set; }
}