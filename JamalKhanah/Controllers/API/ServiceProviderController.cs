using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.DTO;
using JamalKhanah.Core.DTO.Pagination;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Helpers;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace JamalKhanah.Controllers.API;

public class ServiceProviderController : BaseApiController, IActionFilter
{
    private readonly BaseResponse _baseResponse;
    private readonly IFileHandling _fileHandling;
    private readonly IAccountService _accountService;
    private readonly IUnitOfWork _unitOfWork;
    private ApplicationUser _user;

    public ServiceProviderController(IUnitOfWork unitOfWork, IFileHandling fileHandling , IAccountService accountService)
    {
        _unitOfWork = unitOfWork;
        _fileHandling = fileHandling;
        _accountService = accountService;
        _baseResponse = new BaseResponse();
        _user = null;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var accessToken = Request.Headers[HeaderNames.Authorization];
        if (string.IsNullOrEmpty(accessToken))
            return;

        var userId = _accountService.ValidateJwtToken(accessToken);
        if (string.IsNullOrEmpty(userId))
            return;
    
        var user = _unitOfWork.Users.FindByQuery(s => s.Id == userId)
            .FirstOrDefault();
        _user = user;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
    //---------------------------------------------------------------------------------------------------
    // get all service providers
    [HttpGet("GetAll")]
    public async Task<ActionResult<BaseResponse>> GetAll([FromHeader] string lang, [FromQuery] PaginationParameters parameters)
    {
        var serviceProviders = await _unitOfWork.Users.FindByQuery(
                s => (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent) && s.IsApproved == true && s.ShowServices==true && s.Status==true ,
                orderBy: s => s.OrderByDescending(information => information.RegistrationDate),
                skip: (parameters.PageNumber - 1) * parameters.PageSize,
                take: parameters.PageSize)
            .Select(s => new
            {
                s.Id,
                title = s.FullName,
                s.Description,
                s.UserImgUrl,
                s.IsFeatured,
                s.Email,
                City = lang == "ar" ? s.City.NameAr : s.City.NameEn,
                s.UserType,
                NumberOfStar = (s.EvaluationProviders.Select(x => x.NumberOfStars).Any()) ? s.EvaluationProviders.Select(x => x.NumberOfStars).Average() : 0,
                IsFavourite = (_user != null) && s.FavoriteProviders.Any(f => f.UserId == _user.Id),
                Addresses = s.Addresses.Where(address=>address.IsDeleted==false).Select(address=>new
                {
                    address.AddressDetails,
                    address.Region,
                    address.Street,
                    address.BuildingNumber,
                    address.FlatNumber,
                    city = lang == "ar" ? address.City.NameAr : address.City.NameEn,
                })
            }).ToListAsync();

        if (!serviceProviders.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.ServicesNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? " لا يوجد مقدمين خدمات  " : "There are no service providers";
            return Ok(_baseResponse);
        }
        
        var servicesCount = await _unitOfWork.Users.CountAsync(criteria: s =>  (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent) && s.IsApproved == true && s.ShowServices==true && s.Status==true );

        var pageCount = servicesCount / parameters.PageSize;
        if (servicesCount % parameters.PageSize > 0)
        {
            pageCount++;
        }

        if (pageCount == 0)
        {
            pageCount = 1;
        }

        PaginationResponse paginationResponse = new()
        {
            CurrentPage = parameters.PageNumber,
            TotalItems = servicesCount,
            TotalPages = pageCount,
            Items = serviceProviders
        };

        _baseResponse.Data = paginationResponse;
        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول على البيانات بنجاح"
            : "The Data Has Been Retrieved Successfully";

        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------
    // get service provider by id
    [HttpGet("GetById/{id:required}")]
    public async Task<ActionResult<BaseResponse>> GetById([FromHeader] string lang, string id)
    {
        var serviceProvider = await _unitOfWork.Users.FindByQuery(
                s => s.Id == id && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent) && s.IsApproved == true && s.ShowServices == true && s.Status == true,
                orderBy: s => s.OrderByDescending(information => information.RegistrationDate))
            .Select(s => new
            {
                s.Id,
                title = s.FullName,
                s.Description,
                s.UserImgUrl,
                s.IsFeatured,
                s.Email,
                City = lang == "ar" ? s.City.NameAr : s.City.NameEn,
                s.UserType,
                Empeloyees = s.Employees.Select(e => new
                {
                    e.Id,
                    e.FullName,
                    e.Email,
                    e.PhoneNumber,
                    e.ImgUrl,

                }),
                Prizes = s.Prizes.Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Description,
                   
                }),
                Experiences = s.Experiences.Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Description,
                }),
                WorkDays = s.WorkHours.Where(s=>s.IsDeleted==false).Select(w => new
                {
                    w.Id,
                    w.Day,
                    w.From,
                    w.To,
                    w.MoreData
                }),
                MainSection = s.Services.Select(z=>new
                {
                    z.MainSectionId,
                    Titele = lang == "ar" ? z.MainSection.TitleAr : z.MainSection.TitleEn,
                    z.MainSection.ImgUrl,
                }).Distinct(),
                Addresses = s.Addresses.Where(address => address.IsDeleted == false).Select(address => new
                {
                    address.AddressDetails,
                    address.Region,
                    address.Street,
                    address.BuildingNumber,
                    address.FlatNumber,
                    city = lang == "ar" ? address.City.NameAr : address.City.NameEn,
                }),
                PicturesLibrary = s.ProviderPhotos.Where(providerPhoto=>providerPhoto.IsDeleted==false ).Select(c=> new
                {
                    c.ImgUrl
                }),
                ServicesCount = s.Services.Count(),
                IsFavourite = (_user != null) && s.FavoriteProviders.Any(f => f.UserId == _user.Id),
                NumberOfStar = (s.EvaluationProviders.Select(x => x.NumberOfStars).Any())? s.EvaluationProviders.Select(x => x.NumberOfStars).Average(): 0 ,
                Evaluation = (s.EvaluationProviders.Select(x => x.NumberOfStars).Any()) ? s.EvaluationProviders.Select(x => new { x.NumberOfStars , x.Comment}) : null

            }).FirstOrDefaultAsync();

        if (serviceProvider == null)
        {
            _baseResponse.ErrorCode = (int)Errors.ServicesNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? " لا يوجد مقدمين خدمات  " : "There are no service providers";
            return Ok(_baseResponse);
        }

        _baseResponse.Data = serviceProvider;
        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول على البيانات بنجاح"
            : "The Data Has Been Retrieved Successfully";

        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------
    // get service provider services
    [HttpGet("GetAllFeatured")]
    public async Task<ActionResult<BaseResponse>> GetAllFeatured([FromHeader] string lang, [FromQuery] PaginationParameters parameters)
    {
        var serviceProviders = await _unitOfWork.Users.FindByQuery(
                s => (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent) && s.IsApproved == true && s.ShowServices==true && s.Status==true && s.IsFeatured == true  ,
                orderBy: s => s.OrderByDescending(information => information.RegistrationDate),
                skip: (parameters.PageNumber - 1) * parameters.PageSize,
                take: parameters.PageSize)
            .Select(s => new
            {
                s.Id,
                title = s.FullName,
                s.Description,
                s.UserImgUrl,
                s.IsFeatured,
                s.Email,
                City = lang == "ar" ? s.City.NameAr : s.City.NameEn,
                s.UserType,
                Addresses = s.Addresses.Where(address=>address.IsDeleted==false).Select(address=>new
                {
                    address.AddressDetails,
                    address.Region,
                    address.Street,
                    address.BuildingNumber,
                    address.FlatNumber,
                    city = lang == "ar" ? address.City.NameAr : address.City.NameEn,
                })
                
            }).ToListAsync();

        if (!serviceProviders.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.ServicesNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? " لا يوجد مقدمين خدمات  " : "There are no service providers";
            return Ok(_baseResponse);
        }
        
        var servicesCount = await _unitOfWork.Users.CountAsync(criteria: s =>  (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent) && s.IsApproved == true && s.ShowServices==true && s.Status==true );

        var pageCount = servicesCount / parameters.PageSize;
        if (servicesCount % parameters.PageSize > 0)
        {
            pageCount++;
        }

        if (pageCount == 0)
        {
            pageCount = 1;
        }

        PaginationResponse paginationResponse = new()
        {
            CurrentPage = parameters.PageNumber,
            TotalItems = servicesCount,
            TotalPages = pageCount,
            Items = serviceProviders
        };

        _baseResponse.Data = paginationResponse;
        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول على البيانات بنجاح"
            : "The Data Has Been Retrieved Successfully";

        return Ok(_baseResponse);
    }
        
        

}