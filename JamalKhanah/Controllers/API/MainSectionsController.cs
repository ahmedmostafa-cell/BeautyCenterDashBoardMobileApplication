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
public class MainSectionsController : BaseApiController, IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseResponse _baseResponse;
    private ApplicationUser _user;

    public MainSectionsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _baseResponse = new BaseResponse();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var accessToken = Request.Headers[HeaderNames.Authorization];
        if(string.IsNullOrEmpty(accessToken))
            return;
            
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        var user = _unitOfWork.Users.FindByQuery(criteria: s => s.Id == userId)
            .FirstOrDefault();
        _user = user;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public void OnActionExecuted(ActionExecutedContext context)
    {

    }
    //---------------------------------------------------------------------------------------------------
    [HttpGet("MainSections")]
    public async Task<ActionResult<BaseResponse>> MainSections([FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var mainSections = await _unitOfWork.MainSections.FindByQuery(
            s => s.IsShow== true  && s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                Title =(lang == "ar")?s.TitleAr: s.TitleEn,
                s.Description,
                s.IsFeatured,
                s.ImgUrl,
                s.Rank
            }).OrderBy(s=>s.Rank).ToListAsync();

        if (!mainSections.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.MainSectionsNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? " لا يوجد أقسام رئيسية " : "Main Sections Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.Data = mainSections;
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------
    [HttpGet("FeaturedMainSections")]
    public async Task<ActionResult<BaseResponse>> FeaturedMainSections([FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var mainSections = await _unitOfWork.MainSections.FindByQuery(
                s => s.IsShow== true&& s.IsFeatured==true  && s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                Title =(lang == "ar")?s.TitleAr: s.TitleEn,
                s.Description,
                s.IsFeatured,
                s.ImgUrl,
                s.Rank
            }).OrderBy(s=>s.Rank).ToListAsync();

        if (!mainSections.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.MainSectionsNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? " لا يوجد أقسام رئيسية " : "Main Sections Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.Data = mainSections;
        return Ok(_baseResponse);
    }
}