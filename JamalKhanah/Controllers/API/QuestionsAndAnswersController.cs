using JamalKhanah.Core.DTO;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Helpers;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace JamalKhanah.Controllers.API;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class QuestionsAndAnswersController : BaseApiController , IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseResponse _baseResponse;
    private  ApplicationUser _user;
    public QuestionsAndAnswersController(IUnitOfWork unitOfWork)
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
    [AllowAnonymous]
    [HttpGet("GetAll")]
    public async Task<ActionResult<BaseResponse>> Get([FromHeader] string lang)
    {
        var all =await  _unitOfWork.QuestionsAndAnswers.FindByQuery(s=> s.IsDeleted==false &&
                                                                        s.QuestionsAndAnswersSection.IsDeleted==false)
            .Select(s=>new
            {
                s.Id,
                s.Question,
                s.Answer,
                s.QuestionsAndAnswersSection.Section
            }).ToListAsync();
        if (!all.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.QuestionsAndAnswersSectionsNotFound;
            _baseResponse.ErrorMessage = (lang != "ar") ? "Questions and answers  not found" : "لم يتم العثور على  الأسئلة والأجوبة";
            return Ok(_baseResponse);
        }
        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = (lang != "ar") ? "Success" : "نجاح";
        _baseResponse.Data = all;

        return Ok(_baseResponse);
    }

    [AllowAnonymous]
    [HttpGet("GetBySection/{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> GetBySection([FromHeader] string lang,int id )
    {
        var all = await _unitOfWork.QuestionsAndAnswers.FindByQuery(s => s.IsDeleted == false &&
                                                                         s.QuestionsAndAnswersSection.IsDeleted == false &&
                                                                         s.QuestionsAndAnswersSectionId == id)
            .Select(s=>new
            {
                s.Id,
                s.Question,
                s.Answer,
                s.QuestionsAndAnswersSection.Section
            }).ToListAsync();
        if (!all.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.QuestionsAndAnswersSectionsNotFound;
            _baseResponse.ErrorMessage = (lang != "ar") ? "Questions and answers  not found" : "لم يتم العثور على  الأسئلة والأجوبة";
            return Ok(_baseResponse);
        }
        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = (lang != "ar") ? "Success" : "نجاح";
        _baseResponse.Data = all;

        return Ok(_baseResponse);
    }

        
}