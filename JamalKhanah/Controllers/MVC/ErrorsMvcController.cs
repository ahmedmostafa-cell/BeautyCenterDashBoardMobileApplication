using JamalKhanah.Core.DTO;
using Microsoft.AspNetCore.Mvc;


namespace JamalKhanah.Controllers.MVC;


public class ErrorsMvcController : Controller
{

        
    public IActionResult Index(int code, string details = null)
    {
        return code switch
        {
            400 => View(new BaseResponse(){ErrorMessage ="A bad request, you have made" , ErrorCode = code} )  ,
            401 => View(new BaseResponse() { ErrorMessage = "Authorized, you are not", ErrorCode = code }),
            404 => View(new BaseResponse(){ErrorMessage ="Page Not Found ", ErrorCode = code}),
            500 => View(new BaseResponse() { ErrorMessage = "Internal Server Error", ErrorCode = code, Data = details }),
            _ => null
        };
         
    }
}
