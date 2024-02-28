using JamalKhanah.Core.DTO;
using JamalKhanah.Core.Helpers;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace JamalKhanah.Controllers.API;

public class ContactUsController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseResponse _baseResponse;



    public ContactUsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _baseResponse = new BaseResponse();
    }
        
    [HttpGet("ContactUs")]
    public async Task<IActionResult> Get([FromHeader] string lang)
    {
        var contactUs = (await _unitOfWork.ContactUs.GetAllAsync()).FirstOrDefault();

        if (contactUs == null)
        {
            _baseResponse.ErrorCode = (int)Errors.ContactUsNotFound;
            _baseResponse.ErrorMessage = (lang!= "ar")?"ContactUs Not Found":" لا توجد بيانات اتصال ";
            return Ok(_baseResponse);
        }
            
        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = new {
            contactUs.Id,
            contactUs.WhatsAppNumber,
            contactUs.PhoneNumber,
            contactUs.Email,
            contactUs.Link,
            contactUs.FaceBookLink,
            TermsAndConditions = (lang!="ar") ? contactUs.TermsAndConditions:contactUs.TermsAndConditionsAr
        };
        return Ok(_baseResponse);
    }


}