namespace JamalKhanah.Core.Helpers;

public enum OrderStatus
{
    Initialized, // تحت الانشاء لاتزال مع العميل
    Preparing, // تم التأكيد من العميل ولم يتم الموافقة عليه بعد من قبل مقدم الخدمة 
    Confirmed, // تم الموافقة عليه من قبل مقدم الخدمة
    WithDriver, // قيد الوصول
    Finished,  // منتهية 
    Cancelled, // ملغية
}