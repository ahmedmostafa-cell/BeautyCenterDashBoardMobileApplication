using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JamalKhanah.Core.DTO;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.FavoriteData;
using JamalKhanah.Core.Helpers;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;

namespace JamalKhanah.Controllers.API;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class FavoritesController : BaseApiController , IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseResponse _baseResponse;
    private  ApplicationUser _user;
    public FavoritesController(IUnitOfWork unitOfWork)
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


    [HttpGet("GetFavoriteServices")]
    public async Task<ActionResult<BaseResponse>> GetFavoriteServices([FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود   "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var photos = await _unitOfWork.FavoriteServices.FindByQuery(
                criteria: s => s.IsDeleted == false && s.Service.IsShow == true && s.Service.IsDeleted == false && s.Service.Provider.Status == true && s.UserId == _user.Id,
                orderBy: s => s.OrderByDescending(photo => photo.CreatedAt))
            .Select(s => new
            {
                s.Id,
               ServiceId = s.Service.Id,
                title = lang == "ar" ? s.Service.TitleAr : s.Service.TitleEn,
                forCenter = s.Service.ServiceType == ServiceType.Center,
                s.Service.Description,
                s.Service.ImgUrl,
                s.Service.IsFeatured,
                s.Service.PriceUnit,
                s.Service.Price,
                s.Service.Discount,
                s.Service.FinalPrice,
                s.Service.InCenter,
                s.Service.InHome,
                s.Service.IsAvailable,
                s.Service.EmployeesNumber,
                ServiceProvider = new
                {
                    s.Service.Provider.Id,
                    s.Service.Provider.FullName,
                    s.Service.Provider.UserImgUrl
                },
                MainSection = new
                {
                    s.Service.MainSection.Id,
                    title = lang == "ar" ? s.Service.MainSection.TitleAr : s.Service.MainSection.TitleEn,
                    s.Service.MainSection.ImgUrl
                }

            }).ToListAsync();

        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = photos;
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------


    [HttpGet("GetFavoriteProviders")]
    public async Task<ActionResult<BaseResponse>> GetFavoriteProviders([FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود   "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var photos = await _unitOfWork.FavoriteProviders.FindByQuery(
                criteria: s => s.IsDeleted == false && s.Provider.Status == true && s.UserId == _user.Id,
                orderBy: s => s.OrderByDescending(photo => photo.CreatedAt))
            .Select(s => new
            {
                s.Id,
                s.ProviderId,
                Title = s.Provider.FullName,
                s.Provider.Description,
                s.Provider.UserImgUrl,
                s.Provider.IsFeatured,
                s.Provider.Email,
                City = lang == "ar" ? s.Provider.City.NameAr : s.Provider.City.NameEn,
                s.Provider.UserType,
                Addresses = s.Provider.Addresses.Where(address => address.IsDeleted == false).Select(address => new
                {
                    address.AddressDetails,
                    address.Region,
                    address.Street,
                    address.BuildingNumber,
                    address.FlatNumber,
                    city = lang == "ar" ? address.City.NameAr : address.City.NameEn,
                })
            }).ToListAsync();

        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = photos;
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------+
    [HttpPost("AddServiceInFavorite/{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> AddServiceInFavorite(int id, [FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود   "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var service = await _unitOfWork.Services.FindByQuery(s => s.Id == id && s.IsDeleted == false && s.IsShow == true && s.Provider.Status == true).FirstOrDefaultAsync();
        if (service == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheServiceNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذه الخدمة غير موجودة   "
                : "The Service Not Exist ";
            return Ok(_baseResponse);
        }

        var favoriteService = await _unitOfWork.FavoriteServices.FindByQuery(s => s.ServiceId == id && s.UserId == _user.Id && s.IsDeleted == false).FirstOrDefaultAsync();
        if (favoriteService != null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheServiceAlreadyInFavorite;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذه الخدمة موجودة بالفعل في المفضلة   "
                : "The Service Already In Favorite ";
            return Ok(_baseResponse);
        }

        var newFavoriteService = new FavoriteService
        {
            UserId = _user.Id,
            ServiceId = id,
            CreatedAt = DateTime.Now,
            IsDeleted = false
        };
        await _unitOfWork.FavoriteServices.AddAsync(newFavoriteService);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar")
            ? "تمت الاضافة الى المفضلة بنجاح   "
            : "The Service Added To Favorite Successfully ";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------------------------------------
    [HttpPost("AddProviderInFavorite/{id:required}")]
    public async Task<ActionResult<BaseResponse>> AddProviderInFavorite(string id, [FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود   "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var provider = await _unitOfWork.Users.FindByQuery(s => s.Id == id && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)  && s.Status == true).FirstOrDefaultAsync();
        if (provider == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheProviderNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا المزود غير موجود   "
                : "The Provider Not Exist ";
            return Ok(_baseResponse);
        }

        var favoriteProvider = await _unitOfWork.FavoriteProviders.FindByQuery(s => s.ProviderId == id && s.UserId == _user.Id && s.IsDeleted == false).FirstOrDefaultAsync();
        if (favoriteProvider != null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheProviderAlreadyInFavorite;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا المزود موجود بالفعل في المفضلة   "
                : "The Provider Already In Favorite ";
            return Ok(_baseResponse);
        }

        var newFavoriteProvider = new FavoriteProvider
        {
            UserId = _user.Id,
            ProviderId = id,
            CreatedAt = DateTime.Now,
            IsDeleted = false
        };
        await _unitOfWork.FavoriteProviders.AddAsync(newFavoriteProvider);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar")
            ? "تمت الاضافة الى المفضلة بنجاح   "
            : "The Provider Added To Favorite Successfully ";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------------------------------------
    [HttpDelete("DeleteServiceFromFavorite/{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> DeleteServiceFromFavorite(int id, [FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود   "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var service = await _unitOfWork.Services.FindByQuery(s => s.Id == id && s.IsDeleted == false && s.IsShow == true && s.Provider.Status == true).FirstOrDefaultAsync();
        if (service == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheServiceNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذه الخدمة غير موجودة   "
                : "The Service Not Exist ";
            return Ok(_baseResponse);
        }

        var favoriteService = await _unitOfWork.FavoriteServices.FindByQuery(s => s.ServiceId == id && s.UserId == _user.Id && s.IsDeleted == false).FirstOrDefaultAsync();
        if (favoriteService == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheServiceNotInFavorite;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذه الخدمة غير موجودة في المفضلة   "
                : "The Service Not In Favorite ";
            return Ok(_baseResponse);
        }

       
        _unitOfWork.FavoriteServices.Delete(favoriteService);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar")
            ? "تمت الحذف من المفضلة بنجاح   "
            : "The Service Deleted From Favorite Successfully ";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------------------------------------
    [HttpDelete("DeleteProviderFromFavorite/{id:required}")]
    public async Task<ActionResult<BaseResponse>> DeleteProviderFromFavorite(string id, [FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود   "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var provider = await _unitOfWork.Users.FindByQuery(s => s.Id == id && s.Status == true).FirstOrDefaultAsync();
        if (provider == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheProviderNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا المزود غير موجود   "
                : "The Provider Not Exist ";
            return Ok(_baseResponse);
        }

        var favoriteProvider = await _unitOfWork.FavoriteProviders.FindByQuery(s => s.ProviderId == id && s.UserId == _user.Id && s.IsDeleted == false).FirstOrDefaultAsync();
        if (favoriteProvider == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheProviderNotInFavorite;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا المزود غير موجود في المفضلة   "
                : "The Provider Not In Favorite ";
            return Ok(_baseResponse);
        }

        _unitOfWork.FavoriteProviders.Delete(favoriteProvider);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar")
            ? "تمت الحذف من المفضلة بنجاح   "
            : "The Provider Deleted From Favorite Successfully ";
        return Ok(_baseResponse);
    }




}