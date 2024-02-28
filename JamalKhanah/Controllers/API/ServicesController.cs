using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.DTO;
using JamalKhanah.Core.DTO.EntityDto;
using JamalKhanah.Core.DTO.Pagination;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.SectionsData;
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
public class ServicesController : BaseApiController, IActionFilter
{
    private readonly BaseResponse _baseResponse;
    private readonly IFileHandling _fileHandling;
    private readonly IUnitOfWork _unitOfWork;
    private ApplicationUser _user;

    public ServicesController(IUnitOfWork unitOfWork, IFileHandling fileHandling)
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

    [HttpGet("ServicesForUser")]
    public async Task<ActionResult<BaseResponse>> ServicesForUser([FromHeader] string lang, [FromQuery] PaginationParameters parameters, [FromQuery] GetAllServices getAll)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar" ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        if (_user.UserType is UserType.Center or UserType.FreeAgent)
        {
            _baseResponse.ErrorCode = (int)Errors.ThisUserIsProvider;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا المستخدم مقدم خدمة " : "This User Is Provider";
            return Ok(_baseResponse);
        }
        

        var services = await _unitOfWork.Services.FindByQuery(
            criteria: s => s.IsShow == true && s.IsDeleted == false && s.Provider.Status == true && 
                           (getAll.SearchName == null || s.TitleAr.Contains(getAll.SearchName) || s.TitleEn.Contains(getAll.SearchName)) &&
                           (getAll.ServiceProviderName == null || s.Provider.FullName.Contains(getAll.ServiceProviderName) ) &&
                           (getAll.MainSectionId == null || s.MainSectionId == getAll.MainSectionId) &&
                           (getAll.CityId == null || s.Provider.CityId == getAll.CityId) &&
                           (getAll.ServiceProviderId == null || s.Provider.Id == getAll.ServiceProviderId) &&
                           (getAll.InHome == null || s.InHome == getAll.InHome) &&
                           (getAll.InCenter == null || s.InCenter == getAll.InCenter) &&
                           (getAll.StartPrice == null || s.FinalPrice >= getAll.StartPrice) &&
                           (getAll.EndPrice == null || s.FinalPrice <= getAll.EndPrice) &&
                           (getAll.ServiceTypeDto == ServiceTypeDto.Both? (s.ServiceType == ServiceType.Center || s.ServiceType == ServiceType.FreeAgent) :
                               (getAll.ServiceTypeDto == ServiceTypeDto.Center)? s.ServiceType == ServiceType.Center : s.ServiceType == ServiceType.FreeAgent ),
                          
                orderBy: s => (getAll.OrderFromNew ? s.OrderByDescending(o => o.CreatedAt) : s.OrderBy(o => o.CreatedAt)),
                skip: (parameters.PageNumber - 1) * parameters.PageSize,
                take: parameters.PageSize)
            .Select(s => new
            {
                s.Id,
                title = lang == "ar" ? s.TitleAr : s.TitleEn,
                forCenter = s.ServiceType == ServiceType.Center,
                s.Description,
                s.ImgUrl,
                s.IsFeatured,
                s.PriceUnit,
                s.Price,
                s.Discount,
                s.FinalPrice,
                s.InCenter,
                s.InHome,
                s.IsAvailable,
                s.EmployeesNumber,
                NumberOfStar = (s.EvaluationServices.Select(x => x.NumberOfStars).Any()) ? s.EvaluationServices.Select(x => x.NumberOfStars).Average() : 0,
                IsFavourite = (_user != null) && s.FavoriteServices.Any(f => f.UserId == _user.Id),
                ServiceProvider = new
                {
                    s.Provider.Id,
                    s.Provider.FullName,
                    s.Provider.UserImgUrl
                },
                MainSection = new
                {
                    s.MainSection.Id,
                    title = lang == "ar" ? s.MainSection.TitleAr : s.MainSection.TitleEn,
                    s.MainSection.ImgUrl
                }
                
            }).ToListAsync();

        if (!services.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.ServicesNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? " لا يوجد خدمات " : "Services Not Found";
            return Ok(_baseResponse);
        }
        
        var servicesCount = await _unitOfWork.Services.CountAsync(  criteria: s => s.IsShow == true && s.IsDeleted == false && s.Provider.Status == true && 
            (getAll.SearchName == null || s.TitleAr.Contains(getAll.SearchName) || s.TitleEn.Contains(getAll.SearchName)) &&
            (getAll.ServiceProviderName == null || s.Provider.FullName.Contains(getAll.ServiceProviderName) ) &&
            (getAll.MainSectionId == null || s.MainSectionId == getAll.MainSectionId) &&
            (getAll.CityId == null || s.Provider.CityId == getAll.CityId) &&
            (getAll.ServiceProviderId == null || s.Provider.Id == getAll.ServiceProviderId) &&
            (getAll.InHome == null || s.InHome == getAll.InHome) &&
            (getAll.InCenter == null || s.InCenter == getAll.InCenter) &&
            (getAll.StartPrice == null || s.FinalPrice >= getAll.StartPrice) &&
            (getAll.EndPrice == null || s.FinalPrice <= getAll.EndPrice) &&
            (getAll.ServiceTypeDto == ServiceTypeDto.Both? (s.ServiceType == ServiceType.Center || s.ServiceType == ServiceType.FreeAgent) :
                (getAll.ServiceTypeDto == ServiceTypeDto.Center)? s.ServiceType == ServiceType.Center : s.ServiceType == ServiceType.FreeAgent ));

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
            Items = services
        };

        _baseResponse.Data = paginationResponse;
        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول على البيانات بنجاح"
            : "The Data Has Been Retrieved Successfully";

        return Ok(_baseResponse);
    }

    [HttpGet("ServicesForProvider")]
    public async Task<ActionResult<BaseResponse>> Services([FromHeader] string lang, [FromQuery] PaginationParameters parameters)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        if (_user.UserType is UserType.Center or UserType.FreeAgent)
        {
            var providerService = await _unitOfWork.Services.FindByQuery(
                    s =>  s.IsDeleted == false && s.ProviderId == _user.Id,
                    orderBy: s => s.OrderByDescending(information => information.CreatedAt),
                    skip: (parameters.PageNumber - 1) * parameters.PageSize,
                    take: parameters.PageSize)
                .Select(s => new
                {
                    s.Id,
                    title = lang == "ar" ? s.TitleAr : s.TitleEn,
                    forCenter = s.ServiceType == ServiceType.Center,
                    s.Description,
                    s.ImgUrl,
                    s.IsFeatured,
                    s.PriceUnit,
                    s.Price,
                    s.Discount,
                    s.FinalPrice,
                    s.InCenter,
                    s.InHome,
                    s.IsAvailable,
                    s.EmployeesNumber,
                    s.IsShow,
                    NumberOfStar = (s.EvaluationServices.Select(x => x.NumberOfStars).Any()) ? s.EvaluationServices.Select(x => x.NumberOfStars).Average() : 0,
                    Evaluation = (s.EvaluationServices.Any()) ? s.EvaluationServices.Select(x => new { x.NumberOfStars, x.Comment }) : null,
                    ServiceProvider = new
                    {
                        s.Provider.Id,
                        s.Provider.FullName,
                        s.Provider.UserImgUrl,
                      
                    },
                    MainSection = new
                    {
                        s.MainSection.Id,
                        title = lang == "ar" ? s.MainSection.TitleAr : s.MainSection.TitleEn,
                        s.MainSection.ImgUrl
                    }
                }).ToListAsync();

            if (!providerService.Any())
            {
                _baseResponse.ErrorCode = (int)Errors.ServicesNotFound;
                _baseResponse.ErrorMessage = lang == "ar" ? " لا يوجد خدمات " : "Services Not Found";
                return Ok(_baseResponse);
            }

            var servicesCount = await _unitOfWork.Services.CountAsync(criteria: s => s.IsDeleted == false && s.ProviderId == _user.Id);

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
                Items = providerService
            };

            _baseResponse.Data = paginationResponse;

            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "تم الحصول على البيانات بنجاح"
                : "The Data Has Been Retrieved Successfully";
            return Ok(_baseResponse);
            
           
        }
        else
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserIsNotProvider;
            _baseResponse.ErrorMessage = lang == "ar" ? "هذا الحساب ليس مقدم خدمة " : "The User Is Not Provider";
            return Ok(_baseResponse);
        }


    }
    

    [HttpGet("FeaturedServices")]
    public async Task<ActionResult<BaseResponse>> FeaturedServices([FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var services = await _unitOfWork.Services.FindByQuery(
                s => s.IsShow == true && s.IsFeatured == true && s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                title = lang == "ar" ? s.TitleAr : s.TitleEn,
                forCenter = s.ServiceType == ServiceType.Center,
                s.Description,
                s.ImgUrl,
                s.IsFeatured,
                s.PriceUnit,
                s.Price,
                s.Discount,
                s.FinalPrice,
                s.InCenter,
                s.InHome,
                s.EmployeesNumber,
                s.IsAvailable,
                NumberOfStar = (s.EvaluationServices.Select(x => x.NumberOfStars).Any()) ? s.EvaluationServices.Select(x => x.NumberOfStars).Average() : 0,
                IsFavourite = (_user != null) && s.FavoriteServices.Any(f => f.UserId == _user.Id),
                ServiceProvider = new
                {
                    s.Provider.Id,
                    s.Provider.FullName,
                    s.Provider.UserImgUrl,
                 
                },
                MainSection = new
                {
                    s.MainSection.Id,
                    title = lang == "ar" ? s.MainSection.TitleAr : s.MainSection.TitleEn,
                    s.MainSection.ImgUrl
                }
            }).ToListAsync();

        if (!services.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.ServicesNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? " لا يوجد خدمات " : "Services Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول على البيانات بنجاح"
            : "The Data Has Been Retrieved Successfully";
        _baseResponse.Data = services;
        return Ok(_baseResponse);
    }

   /* [HttpGet("ServicesByMainSection/{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> ServicesByMainSection([FromHeader] string lang, int id)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var services = await _unitOfWork.Services.FindByQuery(
                s => s.IsShow == true && s.IsDeleted == false && s.MainSectionId == id)
            .Select(s => new
            {
                s.Id,
                title = lang == "ar" ? s.TitleAr : s.TitleEn,
                forCenter = s.ServiceType == ServiceType.Center,
                s.Description,
                s.ImgUrl,
                s.IsFeatured,
                s.PriceUnit,
                s.Price,
                s.Discount,
                s.FinalPrice,
                s.InCenter,
                s.InHome,
                s.EmployeesNumber,
                s.IsAvailable,
                ServiceProvider = new
                {
                    s.User.Id,
                    s.User.FullName,
                    s.User.UserImgUrl
                },
                MainSection = new
                {
                    s.MainSection.Id,
                    title = lang == "ar" ? s.MainSection.TitleAr : s.MainSection.TitleEn,
                    s.MainSection.ImgUrl
                }
            }).ToListAsync();

        if (!services.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.ServicesNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? " لا يوجد خدمات " : "Services Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول على البيانات بنجاح"
            : "The Data Has Been Retrieved Successfully";
        _baseResponse.Data = services;
        return Ok(_baseResponse);
    }*/

    //---------------------------------------------------------------------------------------------------

    [HttpGet("ServiceDetails/{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> ServiceDetails([FromHeader] string lang, int id)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var service = await _unitOfWork.Services.FindByQuery(
                s => s.IsShow == true && s.IsAvailable && s.IsDeleted == false && s.Id == id)
            .Select(s => new
            {
                s.Id, s.TitleAr , s.TitleEn,
                forCenter = s.ServiceType == ServiceType.Center,
                s.Description,
                s.ImgUrl,
                s.IsFeatured,
                s.PriceUnit,
                s.Price,
                s.Discount,
                s.FinalPrice,
                s.InCenter,
                s.IsAvailable,
                s.Duration,
                s.InHome,
                s.EmployeesNumber,
                NumberOfStar = (s.EvaluationServices.Select(x => x.NumberOfStars).Any()) ? s.EvaluationServices.Select(x => x.NumberOfStars).Average() : 0,
                IsFavourite = (_user != null) && s.FavoriteServices.Any(f => f.UserId == _user.Id),
                Evaluation = (s.EvaluationServices.Any()) ? s.EvaluationServices.Select(x => new { x.NumberOfStars, x.Comment }) : null,
                ServiceProvider = new
                {
                    s.Provider.Id,
                    s.Provider.FullName,
                    s.Provider.UserImgUrl, 
                    Addresses = s.Provider.Addresses.Where(address=>address.IsDeleted==false).Select(address=>new
                    {
                        address.AddressDetails,
                        address.Region,
                        address.Street,
                        address.BuildingNumber,
                        address.FlatNumber,
                        city = lang == "ar" ? address.City.NameAr : address.City.NameEn,
                    }).FirstOrDefault(),
                    NumberOfStar = (s.Provider.EvaluationProviders.Select(x => x.NumberOfStars).Any())? s.Provider.EvaluationProviders.Select(x => x.NumberOfStars).Average(): 0 ,
                },
                MainSection = new
                {
                    s.MainSection.Id,
                    title = lang == "ar" ? s.MainSection.TitleAr : s.MainSection.TitleEn,
                    s.MainSection.ImgUrl
                }
            }).FirstOrDefaultAsync();

        if (service == null)
        {
            _baseResponse.ErrorCode = (int)Errors.ServicesNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? " لا يوجد خدمات " : "Services Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول على البيانات بنجاح"
            : "The Data Has Been Retrieved Successfully";
        _baseResponse.Data = service;
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    [HttpPost("AddService")]
    public async Task<ActionResult<BaseResponse>> AddService([FromHeader] string lang, [FromForm] ServiceDto serviceDto)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        if (_user.UserType != UserType.Center && _user.UserType != UserType.FreeAgent)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب ليس مزود خدمة "
                : "The User Not Provider ";
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

        var mainSection = await _unitOfWork.MainSections.FindByQuery(s => s.Id == serviceDto.MainSectionId)
            .FirstOrDefaultAsync();
        if (mainSection == null)
        {
            _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "القسم الرئيسي غير موجود "
                : "The Main Section Not Exist ";
            return Ok(_baseResponse);
        }

        if (serviceDto.InCenter == false && serviceDto.InHome == false)
        {
            _baseResponse.ErrorCode = (int)Errors.InCenterAndInHomeAreFalse;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "يجب اختيار خدمة في المنزل او في المركز "
                : "In Center And In Home Are False ";
            return Ok(_baseResponse);
        }

        if (string.IsNullOrEmpty(serviceDto.Img))
        {
            _baseResponse.ErrorCode = (int)Errors.TheImageIsRequired;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "الصورة مطلوبة "
                : "The Image Is Required ";
            return Ok(_baseResponse);
        }

        string imgUrl;
        try
        {
            imgUrl = await _fileHandling.UploadPhotoBase64(serviceDto.Img, "Services");
        }
        catch
        {
            _baseResponse.ErrorCode = (int)Errors.ErrorInUploadPhoto;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "خطأ في رفع الصورة "
                : "Error In Upload Photo ";
            return Ok(_baseResponse);
        }

        var service = new Service
        {
            TitleAr = serviceDto.TitleAr,
            TitleEn = serviceDto.TitleEn,
            Description = serviceDto.Description,
            ImgUrl = imgUrl,
            IsFeatured = false,
            PriceUnit = serviceDto.PriceUnit,
            Price = serviceDto.Price,
            Discount = serviceDto.Discount,
            FinalPrice = serviceDto.FinalPrice,
            InCenter = _user.UserType != UserType.FreeAgent && serviceDto.InCenter,
            InHome = serviceDto.InHome,
            EmployeesNumber = _user.UserType == UserType.FreeAgent ? 0 : serviceDto.EmployeesNumber,
            ServiceType = _user.UserType == UserType.FreeAgent ? ServiceType.FreeAgent : ServiceType.Center,
            MainSectionId = serviceDto.MainSectionId,
            ProviderId = _user.Id,
            IsShow = true,
            IsAvailable = serviceDto.IsAvailable,
            IsDeleted = false,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Duration = serviceDto.Duration
        };
        try
        {
            await _unitOfWork.Services.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();
        }
        catch 
        {
            _baseResponse.ErrorCode = (int)Errors.ErrorInAddService;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "خطأ في اضافة الخدمة "
                : "Error In Add Service ";
            return Ok(_baseResponse);
        }

      

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم اضافة الخدمة بنجاح"
            : "The Service Has Been Added Successfully";

        return Ok(_baseResponse);
    }

    //-----------------------------------------------------------------------------------------------------

    [HttpPut("UpdateService/{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> UpdateService([FromHeader] string lang, int id,
        [FromForm] ServiceDto serviceDto)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        if (_user.UserType != UserType.Center && _user.UserType != UserType.FreeAgent)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotProvider;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب ليس مزود خدمة "
                : "The User Not Provider ";
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

        var service = await _unitOfWork.Services.FindByQuery(s =>
                s.Id == id && s.ProviderId == _user.Id && s.IsDeleted == false)
            .FirstOrDefaultAsync();
        if (service == null)
        {
            _baseResponse.ErrorCode = (int)Errors.ServiceNotFound;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "الخدمة غير موجودة "
                : "The Service Not Exist ";
            return Ok(_baseResponse);
        }

        var mainSection = await _unitOfWork.MainSections.FindByQuery(s => s.Id == serviceDto.MainSectionId)
            .FirstOrDefaultAsync();
        if (mainSection == null)
        {
            _baseResponse.ErrorCode = (int)Errors.MainSectionNotFound;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "القسم الرئيسي غير موجود "
                : "The Main Section Not Exist ";
            return Ok(_baseResponse);
        }
        if (serviceDto.InCenter == false && serviceDto.InHome == false)
        {
            _baseResponse.ErrorCode = (int)Errors.InCenterAndInHomeAreFalse;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "يجب اختيار خدمة في المنزل او في المركز "
                : "In Center And In Home Are False ";
            return Ok(_baseResponse);
        }

        var imgUrl = "";
        if (!string.IsNullOrEmpty(serviceDto.Img))
            try
            {
                imgUrl = await _fileHandling.UploadPhotoBase64(serviceDto.Img, "Services");
            }
            catch
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInUploadPhoto;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "خطأ في رفع الصورة "
                    : "Error In Upload Photo ";
                return Ok(_baseResponse);
            }

        service.TitleAr = serviceDto.TitleAr;
        service.TitleEn = serviceDto.TitleEn;
        service.Description = serviceDto.Description;
        service.ImgUrl = string.IsNullOrEmpty(imgUrl) ? service.ImgUrl : imgUrl;
        service.PriceUnit = serviceDto.PriceUnit;
        service.Price = serviceDto.Price;
        service.Discount = serviceDto.Discount;
        service.FinalPrice = serviceDto.FinalPrice;
        service.InCenter = _user.UserType != UserType.FreeAgent && serviceDto.InCenter;
        service.InHome = serviceDto.InHome;
        service.EmployeesNumber = _user.UserType == UserType.FreeAgent ? 0 : serviceDto.EmployeesNumber;
        service.ServiceType = _user.UserType == UserType.FreeAgent ? ServiceType.FreeAgent : ServiceType.Center;
        service.MainSectionId = serviceDto.MainSectionId;
        service.UpdatedAt = DateTime.Now;
        service.IsShow = true;
        service.IsAvailable = service.IsAvailable;
        service.IsDeleted = false;

        _unitOfWork.Services.Update(service);
        await _unitOfWork.SaveChangesAsync();

        #region  return data Error 

        /*   var newData = _unitOfWork.Services
             .FindByQuery(s => s.Id == service.Id && s.IsDeleted == false && s.IsAvailable == true)
             .Select(s => new
             {
                 s.Id,
                 title = lang == "ar" ? s.TitleAr : s.TitleEn,
                 forCenter = s.ServiceType == ServiceType.Center,
                 s.Description,
                 s.ImgUrl,
                 s.IsFeatured,
                 s.PriceUnit,
                 s.Price,
                 s.Discount,
                 s.FinalPrice,
                 s.InCenter,
                 s.InHome,
                 s.EmployeesNumber,
                 ServiceProvider = new
                 {
                     s.User.Id,
                     s.User.FullName,
                     s.User.UserImgUrl
                 },
                 MainSection = new
                 {
                     s.MainSection.Id,
                     title = lang == "ar" ? s.MainSection.TitleAr : s.MainSection.TitleEn,
                     s.MainSection.ImgUrl
                 }
 
             }).FirstOrDefaultAsync();*/

        #endregion
      

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم اضافة الخدمة بنجاح"
            : "The Service Has Been Added Successfully";
        /* _baseResponse.Data = newData;*/

        return Ok(_baseResponse);
    }

    //-----------------------------------------------------------------------------------------------------

    [HttpDelete("DeleteService/{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> DeleteService([FromHeader] string lang, int id)
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

        var service = await _unitOfWork.Services.FindByQuery(s =>
                s.Id == id && s.ProviderId == _user.Id && s.IsDeleted == false)
            .FirstOrDefaultAsync();
        if (service == null)
        {
            _baseResponse.ErrorCode = (int)Errors.ServiceNotFound;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "الخدمة غير موجودة "
                : "The Service Not Exist ";
            return Ok(_baseResponse);
        }

        service.IsDeleted = true;
        service.IsAvailable = false;
        service.IsShow = false;
        service.UpdatedAt = DateTime.Now;
        service.DeletedAt = DateTime.Now;

        _unitOfWork.Services.Update(service);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم حذف الخدمة بنجاح"
            : "The Service Has Been Deleted Successfully";
        _baseResponse.Data = new { };

        return Ok(_baseResponse);
    }

    //-----------------------------------------------------------------------------------------------------
      [HttpGet("ServicesByProvider/{id:required}")]
    public async Task<ActionResult<BaseResponse>> ServicesForUser([FromHeader] string lang, [FromQuery] PaginationParameters parameters,
       string id)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var services = await _unitOfWork.Services.FindByQuery(
                s => s.IsShow == true && s.IsDeleted == false && s.ProviderId == id,
                orderBy: s => s.OrderByDescending(information => information.CreatedAt),
                skip: (parameters.PageNumber - 1) * parameters.PageSize,
                take: parameters.PageSize)
            .Select(s => new
            {
                s.Id,
                title = lang == "ar" ? s.TitleAr : s.TitleEn,
                forCenter = s.ServiceType == ServiceType.Center,
                s.Description,
                s.ImgUrl,
                s.IsFeatured,
                s.PriceUnit,
                s.Price,
                s.Discount,
                s.FinalPrice,
                s.InCenter,
                s.InHome,
                s.IsAvailable,
                s.Duration,
                s.EmployeesNumber,
                ServiceProvider = new
                {
                    s.Provider.Id,
                    s.Provider.FullName,
                    s.Provider.UserImgUrl
                },
                MainSection = new
                {
                    s.MainSection.Id,
                    title = lang == "ar" ? s.MainSection.TitleAr : s.MainSection.TitleEn,
                    s.MainSection.ImgUrl
                }
            }).ToListAsync();

        if (!services.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.ServicesNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? " لا يوجد خدمات " : "Services Not Found";
            return Ok(_baseResponse);
        }
        
        var servicesCount = await _unitOfWork.Services.CountAsync(criteria: s => s.IsShow == true && s.IsDeleted == false  &&  s.ProviderId == id);

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
            Items = services
        };

        _baseResponse.Data = paginationResponse;
        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول على البيانات بنجاح"
            : "The Data Has Been Retrieved Successfully";

        return Ok(_baseResponse);
    }
}