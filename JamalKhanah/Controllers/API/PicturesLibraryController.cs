using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.DTO;
using JamalKhanah.Core.DTO.EntityDto;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.Other;
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
public class PicturesLibraryController : BaseApiController, IActionFilter
{
    private readonly BaseResponse _baseResponse;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileHandling _fileHandling;
    private ApplicationUser _user;

    public PicturesLibraryController(IUnitOfWork unitOfWork, IFileHandling fileHandling)
    {
        _unitOfWork = unitOfWork;
        _fileHandling = fileHandling;
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
    //  GET: api/ProviderPhotos
    [HttpGet("GetAllForProvider")]
    public async Task<ActionResult<BaseResponse>> PicturesLibrary([FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        if (_user.UserType is UserType.Admin)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب ليس مزود خدمة "
                : "The User Not Provider ";
            return Ok(_baseResponse);
        }
        if (_user.UserType is UserType.User)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب ليس مزود خدمة "
                : "The User Not Provider ";
            return Ok(_baseResponse);
        }
        var picturesLibrary = await _unitOfWork.ProviderPhotos.FindByQuery(
            s => s.ProviderId == _user.Id && s.IsDeleted == false)
            .Select(s=>new {s.Id, s.ImgUrl, s.CreatedAt}).ToListAsync();

        if (!picturesLibrary.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.PrizeNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? "لا توجد صور للمستخدم " : "Pictures Library Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.Data = picturesLibrary;
        return Ok(_baseResponse);
    }

    // GET: api/ProviderPhotos/5
    [HttpGet("GetAllByProviderId{id:required}")]
    public async Task<ActionResult<BaseResponse>> Prize(string id, [FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var picturesLibrary = await _unitOfWork.ProviderPhotos.FindByQuery(
                s => s.ProviderId == id && s.IsDeleted == false)
            .Select(s => new { s.Id, s.ImgUrl, s.CreatedAt }).ToListAsync();

        if (!picturesLibrary.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.PrizeNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? "لا توجد صور للمستخدم " : "Pictures Library Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.Data = picturesLibrary;
        return Ok(_baseResponse);
    }

    // POST: api/ProviderPhotos
    [HttpPost]
    public async Task<ActionResult<BaseResponse>> AddPhoto(ProviderPhotosDto model, [FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        if (_user.UserType is  UserType.Admin)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب ليس مزود خدمة "
                : "The User Not Provider ";
            return Ok(_baseResponse);
        }
        if (_user.UserType is UserType.User)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب ليس مزود خدمة "
                : "The User Not Provider ";
            return Ok(_baseResponse);
        }

        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new
            {
                message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
            };
            return Ok(_baseResponse);
        }

        var providerPhoto = new ProviderPhoto();
        try
        {
            providerPhoto.ImgUrl = await _fileHandling.UploadPhotoBase64(model.Photo, "ProviderPhotos");
            providerPhoto.ProviderId = _user.Id;
            providerPhoto.CreatedAt = DateTime.Now;
            providerPhoto.IsDeleted = false;
            
        }
        catch 
        {
            _baseResponse.ErrorCode = (int)Errors.ErrorInUploadFile;
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في رفع الصورة" : "Error in upload file";
            return Ok(_baseResponse);
        }


       


        await _unitOfWork.ProviderPhotos.AddAsync(providerPhoto);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم اضافة الصورة بنجاح" : "Photo Added Successfully";
        return Ok(_baseResponse);
    }

    // DELETE: api/ProviderPhotos/5
    [HttpDelete("{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> DeletePhoto(int id, [FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        if (_user.UserType is not UserType.Center or UserType.FreeAgent)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب ليس مزود خدمة "
                : "The User Not Provider ";
            return Ok(_baseResponse);
        }

        var providerPhoto = await _unitOfWork.ProviderPhotos
            .FindByQuery(s => s.Id == id && s.ProviderId == _user.Id && s.IsDeleted == false)
            .FirstOrDefaultAsync();
        if (providerPhoto == null)
        {
            _baseResponse.ErrorCode = (int)Errors.PrizeNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? "هذه الصورة غير موجودة " : "Photo Not Found";
            return Ok(_baseResponse);
        }


        _unitOfWork.ProviderPhotos.Delete(providerPhoto);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم حذف الصورة بنجاح" : "Photo Deleted Successfully";
        return Ok(_baseResponse);

    }

}