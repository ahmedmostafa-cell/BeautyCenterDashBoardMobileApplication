using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JamalKhanah.Core.DTO;
using JamalKhanah.Core.DTO.EntityDto;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.EvaluationData;
using JamalKhanah.Core.Helpers;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;

namespace JamalKhanah.Controllers.API;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class EvaluationsController : BaseApiController , IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseResponse _baseResponse;
    private  ApplicationUser _user;
    public EvaluationsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _baseResponse = new BaseResponse();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var accessToken = Request.Headers[HeaderNames.Authorization];
        if (string.IsNullOrEmpty(accessToken))
            return;

        var userId = User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        var user = _unitOfWork.Users.FindByQuery(s => s.Id == userId)
            .FirstOrDefault();
        _user = user;
        
    }

        
    [ApiExplorerSettings(IgnoreApi = true)]
    public void OnActionExecuted(ActionExecutedContext context)
    {
 
    }
    //---------------------------------------------------------------------------------------------------

    [HttpPost("AddServiceEvaluations")]
    public async Task<IActionResult> AddServiceEvaluations([FromHeader]string lang , [FromBody] ServiceEvaluationDto model)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود   "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = lang == "ar" ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new
            {
                message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
            };
            return Ok(_baseResponse);
        }
        


        var service = await _unitOfWork.Orders.FindByQuery(s => s.OrderStatus == OrderStatus.Finished && s.ServiceId == model.ServiceId).Select(s=>s.Service).FirstOrDefaultAsync();
        if (service == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheServiceNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")? "هذه الخدمة غير موجودة أو لم يتم الانتهاء من الطلب بعد " : "The Service Not Exist Or Not Finished Yet";
            return Ok(_baseResponse);

        }
        var serviceEvaluation = await _unitOfWork.EvaluationServices.FindByQuery(s => s.ServiceId == service.Id && s.UserId == _user.Id).FirstOrDefaultAsync();

        if (serviceEvaluation != null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheServiceAlreadyEvaluated;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لقد قمت بتقييم هذه الخدمة من قبل " : "The Service Already Evaluated";
            return Ok(_baseResponse);
        }
        serviceEvaluation = new EvaluationService
        {
            ServiceId = service.Id,
            UserId = _user.Id,
            NumberOfStars = model.NumberOfStars,
            Comment = model.Comment
        };
        await _unitOfWork.EvaluationServices.AddAsync(serviceEvaluation);
        await _unitOfWork.SaveChangesAsync();
        
        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم اضافة التقييم بنجاح"
            : "The evaluation Has Been Added Successfully";

        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    [HttpPost("AddProviderEvaluations")]
    public async Task<IActionResult> AddProviderEvaluations([FromHeader] string lang,
        [FromBody] ProviderEvaluationDto model)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود   "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = lang == "ar" ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new
            {
                message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
            };
            return Ok(_baseResponse);
        }

        var provider = await _unitOfWork.Orders
            .FindByQuery(s => s.OrderStatus == OrderStatus.Finished && s.Service.ProviderId == model.ProviderId)
            .Select(s => s.Service.Provider).FirstOrDefaultAsync();
        if (provider == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheProviderNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا المزود غير موجود أو لم يتم الانتهاء من الطلب بعد "
                : "The Provider Not Exist Or Not Finished Yet";
            return Ok(_baseResponse);

        }

        var providerEvaluation = await _unitOfWork.EvaluationProviders
            .FindByQuery(s => s.ProviderId == provider.Id && s.UserId == _user.Id).FirstOrDefaultAsync();

        if (providerEvaluation != null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheProviderAlreadyEvaluated;
            _baseResponse.ErrorMessage =
                (lang == "ar") ? "لقد قمت بتقييم هذا المزود من قبل " : "The Provider Already Evaluated";
            return Ok(_baseResponse);
        }

        providerEvaluation = new EvaluationProvider
        {
            ProviderId = provider.Id,
            UserId = _user.Id,
            NumberOfStars = model.NumberOfStars,
            Comment = model.Comment
        };
        await _unitOfWork.EvaluationProviders.AddAsync(providerEvaluation);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم اضافة التقييم بنجاح"
            : "The evaluation Has Been Added Successfully";

        return Ok(_baseResponse);
    }






}