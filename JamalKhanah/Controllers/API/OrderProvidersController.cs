using JamalKhanah.BusinessLayer.Interfaces;
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
using JamalKhanah.Core.DTO.NotificationModel;
namespace JamalKhanah.Controllers.API;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OrderProvidersController : BaseApiController, IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileHandling _fileHandling;
    private readonly BaseResponse _baseResponse;
    private ApplicationUser _user;
    private readonly NotificationModel _notificationModel;
    private readonly INotificationService _notificationService;
    public OrderProvidersController(INotificationService notificationService, IUnitOfWork unitOfWork, IFileHandling fileHandling)
    {
        _unitOfWork = unitOfWork;
        _fileHandling = fileHandling;
        _baseResponse = new BaseResponse();
        _notificationModel = new NotificationModel();
        _notificationService = notificationService;
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


    [HttpGet("OrderStatus")]
    [AllowAnonymous]
    public ActionResult<BaseResponse> GetOrderStatus()
    {
        var orderStatus = new
        {
            Initialized = new
            {
                id = (int)OrderStatus.Initialized,
                description = "تحت الانشاء لايزال مع العميل",
                ForUser = true
            },
            Preparing = new
            {
                id = (int)OrderStatus.Preparing,
                description = "تم التأكيد من العميل ولم يتم الموافقة عليه بعد من قبل مقدم الخدمة ",
                ForUser = true
            },
            Confirmed = new
            {
                id = (int)OrderStatus.Confirmed,
                description = "تم الموافقة عليه من قبل مقدم الخدمة",
                ForUser = false
            },
            WithDriver = new
            {
                id = (int)OrderStatus.WithDriver,
                description = "قيد الوصول",
                ForUser = false
            },

            Finished = new
            {
                id = (int)OrderStatus.Finished,
                description = "تم الانتهاء",
                ForUser = false
            },
            Cancelled = new
            {
                id = (int)OrderStatus.Cancelled,
                description = "تم الالغاء",
                ForUser = true
            }
        };
        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = orderStatus;
        return Ok(_baseResponse);
    }

    // put: api/ChangeOrderStatus
    [HttpPut("ChangeOrderStatus/{id:required:int}/{orderStatus:required:int}")]
    public async Task<ActionResult<BaseResponse>> ChangeOrderStatus([FromHeader] string lang, int id, int orderStatus)
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

        var order = await _unitOfWork.Orders.FindByQuery(
                s => s.Id == id &&
                     s.Service.ProviderId == _user.Id &&
                     s.IsDeleted == false)
            .FirstOrDefaultAsync();
       
        if (order == null)
        {
            _baseResponse.ErrorCode = (int)Errors.NoData;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "لا يوجد طلبات "
                : "No Orders ";
            return Ok(_baseResponse);
        }

        switch (orderStatus)
        {
            case (int)OrderStatus.Initialized:
                _baseResponse.ErrorCode = (int)Errors.InvalidData;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "لا يمكن تغيير الحالة الى مبدئية "
                    : "Can't Change Status To Initialized ";
                return Ok(_baseResponse);

            case (int)OrderStatus.Preparing:
                _baseResponse.ErrorCode = (int)Errors.InvalidData;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "لا يمكن تغيير الحالة الى جاري التجهيز "
                    : "Can't Change Status To Preparing ";
                return Ok(_baseResponse);

            case (int)OrderStatus.Cancelled:
                _baseResponse.ErrorCode = (int)Errors.InvalidData;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "لا يمكن تغيير الحالة الى ملغية "
                    : "Can't Change Status To Cancelled ";
                return Ok(_baseResponse);

            case (int)OrderStatus.Finished:
                order.OrderStatus = OrderStatus.Finished;
                break;

            case (int)OrderStatus.Confirmed:
                order.OrderStatus = OrderStatus.Confirmed;
                var orderNumber = await _unitOfWork.Orders.FindAsync(s => s.Id == order.Id);
                var service = await _unitOfWork.Services.FindAsync(s => s.Id == orderNumber.ServiceId);
                var user = await _unitOfWork.Users.FindAsync(s => s.Id == orderNumber.UserId);
                _notificationModel.DeviceId = user.DeviceToken;
                _notificationModel.Title = "اشعار جديد";
                _notificationModel.Body = "تم الموافقة علي الطلب من قبل مقدم الخدمة";
                var notificationResult = _notificationService.SendNotification(_notificationModel).GetAwaiter();

                break;

            case (int)OrderStatus.WithDriver:
                order.OrderStatus = OrderStatus.WithDriver;
                break;
        }

        ;

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم تغيير حالة الطلب بنجاح "
            : "Change Order Status Successfully ";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------
    // GET: api/OrderProviders
    [HttpGet("PreparedOrders")]
    public async Task<ActionResult<BaseResponse>> PreparedOrders([FromHeader] string lang)
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

        var orders = await _unitOfWork.Orders.FindByQuery(
                s => s.Service.ProviderId == _user.Id &&
                     s.OrderStatus == OrderStatus.Preparing &&
                     s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                s.CreatedOn,
                s.Total,
                s.OrderStatus,
                s.InHome,
                AddressData = new
                {
                    name = lang == "ar" ? s.City.NameAr : s.City.NameEn,
                    s.Region,
                    s.Street,
                    s.BuildingNumber,
                    s.FlatNumber,
                    s.AddressDetails
                },

                Service = new
                {
                    s.Service.Id,
                    title = lang == "ar" ? s.Service.TitleAr : s.Service.TitleEn,
                    s.Service.IsAvailable,
                    s.Service.FinalPrice,
                    s.Service.ImgUrl
                },
                User = new
                {
                    s.User.Id,
                    s.User.FullName,
                    s.User.PhoneNumber,
                    s.User.Email
                }
            }).ToListAsync();

        if (!orders.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.NoData;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "لا يوجد طلبات "
                : "No Orders ";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول علي الطلبات بنجاح "
            : "Get Orders Successfully ";
        _baseResponse.Data = orders;
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------
    // GET: api/OrderProviders
    [HttpGet("ConfirmedOrders")]
    public async Task<ActionResult<BaseResponse>> ConfirmedOrders([FromHeader] string lang)
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

        var orders = await _unitOfWork.Orders.FindByQuery(
                s => s.Service.ProviderId == _user.Id &&
                     s.OrderStatus == OrderStatus.Confirmed &&
                     s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                s.CreatedOn,
                s.Total,
                s.OrderStatus,
                s.InHome,
                AddressData = new
                {
                    name = lang == "ar" ? s.City.NameAr : s.City.NameEn,
                    s.Region,
                    s.Street,
                    s.BuildingNumber,
                    s.FlatNumber,
                    s.AddressDetails,
                    s.Service.ImgUrl
                },
                Service = new
                {
                    s.Service.Id,
                    title = lang == "ar" ? s.Service.TitleAr : s.Service.TitleEn,
                    s.Service.IsAvailable,
                    s.Service.FinalPrice
                },
                User = new
                {
                    s.User.Id,
                    s.User.FullName,
                    s.User.PhoneNumber,
                    s.User.Email
                }
            }).ToListAsync();

        if (!orders.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.NoData;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "لا يوجد طلبات "
                : "No Orders ";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول علي الطلبات بنجاح "
            : "Get Orders Successfully ";
        _baseResponse.Data = orders;
        return Ok(_baseResponse);
    }


    //---------------------------------------------------------------------------------------------------
    // GET: api/OrderProviders
    [HttpGet("WithDriverOrders")]
    public async Task<ActionResult<BaseResponse>> WithDriverOrders([FromHeader] string lang)
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

        var orders = await _unitOfWork.Orders.FindByQuery(
                s => s.Service.ProviderId == _user.Id &&
                     s.OrderStatus == OrderStatus.WithDriver &&
                     s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                s.CreatedOn,
                s.Total,
                s.OrderStatus,
                s.InHome,
                AddressData = new
                {
                    name = lang == "ar" ? s.City.NameAr : s.City.NameEn,
                    s.Region,
                    s.Street,
                    s.BuildingNumber,
                    s.FlatNumber,
                    s.AddressDetails
                },


                Service = new
                {
                    s.Service.Id,
                    title = lang == "ar" ? s.Service.TitleAr : s.Service.TitleEn,
                    s.Service.IsAvailable,
                    s.Service.FinalPrice,
                    s.Service.ImgUrl
                },
                User = new
                {
                    s.User.Id,
                    s.User.FullName,
                    s.User.PhoneNumber,
                    s.User.Email
                }
            }).ToListAsync();

        if (!orders.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.NoData;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "لا يوجد طلبات "
                : "No Orders ";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول علي الطلبات بنجاح "
            : "Get Orders Successfully ";
        _baseResponse.Data = orders;
        return Ok(_baseResponse);
    }

    //----------------------------------------------------------------------------------------------------
    // GET: api/OrderProviders
    [HttpGet("FinishedOrders")]
    public async Task<ActionResult<BaseResponse>> FinishedOrders([FromHeader] string lang)
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

        var orders = await _unitOfWork.Orders.FindByQuery(
                s => s.Service.ProviderId == _user.Id &&
                     s.OrderStatus == OrderStatus.Finished &&
                     s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                s.CreatedOn,
                s.Total,
                s.OrderStatus,
                s.InHome,
                AddressData = new
                {
                    name = lang == "ar" ? s.City.NameAr : s.City.NameEn,
                    s.Region,
                    s.Street,
                    s.BuildingNumber,
                    s.FlatNumber,
                    s.AddressDetails,
                    s.Service.ImgUrl
                },

                Service = new
                {
                    s.Service.Id,
                    title = lang == "ar" ? s.Service.TitleAr : s.Service.TitleEn,
                    s.Service.IsAvailable,
                    s.Service.FinalPrice
                },
                User = new
                {
                    s.User.Id,
                    s.User.FullName,
                    s.User.PhoneNumber,
                    s.User.Email
                }
            }).ToListAsync();

        if (!orders.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.NoData;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "لا يوجد طلبات "
                : "No Orders ";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول علي الطلبات بنجاح "
            : "Get Orders Successfully ";
        _baseResponse.Data = orders;
        return Ok(_baseResponse);
    }


    //----------------------------------------------------------------------------------------------------
    // GET: api/OrderProviders
    [HttpGet("CancelledOrders")]
    public async Task<ActionResult<BaseResponse>> CancelledOrders([FromHeader] string lang)
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

        var orders = await _unitOfWork.Orders.FindByQuery(
                s => s.Service.ProviderId == _user.Id &&
                     s.OrderStatus == OrderStatus.Cancelled &&
                     s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                s.CreatedOn,
                s.Total,
                s.OrderStatus,
                s.InHome,
                AddressData = new
                {
                    name = lang == "ar" ? s.City.NameAr : s.City.NameEn,
                    s.Region,
                    s.Street,
                    s.BuildingNumber,
                    s.FlatNumber,
                    s.AddressDetails
                },

                Service = new
                {
                    s.Service.Id,
                    title = lang == "ar" ? s.Service.TitleAr : s.Service.TitleEn,
                    s.Service.IsAvailable,
                    s.Service.FinalPrice,
                    s.Service.ImgUrl
                },
                User = new
                {
                    s.User.Id,
                    s.User.FullName,
                    s.User.PhoneNumber,
                    s.User.Email
                }
            }).ToListAsync();

        if (!orders.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.NoData;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "لا يوجد طلبات "
                : "No Orders ";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول علي الطلبات بنجاح "
            : "Get Orders Successfully ";
        _baseResponse.Data = orders;
        return Ok(_baseResponse);
    }


    //----------------------------------------------------------------------------------------------------
    [HttpGet("GetAllOrders")]
    public async Task<ActionResult<BaseResponse>> GetAllOrders([FromHeader] string lang, [FromQuery] GetAllOrder model)
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

        if (model.OrderStatus != null)
        {
            var exists = Enum.IsDefined(typeof(OrderStatus), model.OrderStatus);
            if (!exists || (int)model.OrderStatus is (int)OrderStatus.Initialized)
            {
                _baseResponse.ErrorCode = (int)Errors.NoDataFound;
                _baseResponse.ErrorMessage = lang == "ar"
                    ? "الحالة غير  موجودة أو غير صحيحة "
                    : "Status Not Found or not correct";
                return Ok(_baseResponse);
            }
        }


        var orders = await _unitOfWork.Orders.FindByQuery(
                s => s.Service.ProviderId == _user.Id &&
                     ((model.OrderStatus == null) || s.OrderStatus == (OrderStatus)model.OrderStatus) &&
                     ((model.Start == null || model.End == null) || (s.CreatedOn > model.Start.Value && s.CreatedOn < model.End.Value)) &&
                     s.InHome == model.InHome && s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                s.CreatedOn,
                s.Total,
                s.OrderStatus,
                s.InHome,
                AddressData = new
                {
                    name = lang == "ar" ? s.City.NameAr : s.City.NameEn,
                    s.Region,
                    s.Street,
                    s.BuildingNumber,
                    s.FlatNumber,
                    s.AddressDetails
                },

                Service = new
                {
                    s.Service.Id,
                    title = lang == "ar" ? s.Service.TitleAr : s.Service.TitleEn,
                    s.Service.IsAvailable,
                    s.Service.FinalPrice,
                    s.Service.ImgUrl
                },
                User = new
                {
                    s.User.Id,
                    s.User.FullName,
                    s.User.PhoneNumber,
                    s.User.Email
                }
            }).ToListAsync();

        if (!orders.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.NoData;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "لا يوجد طلبات "
                : "No Orders ";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الحصول علي الطلبات بنجاح "
            : "Get Orders Successfully ";
        _baseResponse.Data = orders;
        return Ok(_baseResponse);
    }

    //----------------------------------------------------------------------------------------------------
    [HttpGet("GetOrderById/{id:required:int}")]
    public async Task<ActionResult<BaseResponse>> GetOrderById([FromHeader] string lang, int id)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var order = await _unitOfWork.Orders.FindByQuery(
                criteria: s => s.Id == id &&
                               s.Service.ProviderId == _user.Id &&
                               s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                s.CreatedOn,
                s.Total,
                s.OrderStatus,
                s.InHome,
                AddressData = new
                {
                    name = (lang == "ar" ? s.City.NameAr : s.City.NameEn),
                    s.Region,
                    s.Street,
                    s.BuildingNumber,
                    s.FlatNumber,
                    s.AddressDetails,
                },
                Service = new
                {
                    s.Service.Id,
                    title = lang == "ar" ? s.Service.TitleAr : s.Service.TitleEn,
                    s.Service.IsAvailable,
                    s.Service.FinalPrice,
                    s.Service.ImgUrl,
                    s.Service.Duration
                },
                Provider = new
                {
                    s.Service.Provider.Id,
                    s.Service.Provider.FullName,
                    s.Service.Provider.PhoneNumber,
                    s.Service.Provider.Email,
                },
                User = new
                {
                    s.User.Id,
                    s.User.FullName,
                    s.User.PhoneNumber,
                    s.User.Email,
                    s.User.UserImgUrl
                }
            }).FirstOrDefaultAsync();

        if (order == null)
        {
            _baseResponse.ErrorCode = (int)Errors.NoData;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "لا يوجد طلبات "
                : "No Orders ";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.Data = order;
        return Ok(_baseResponse);
    }
}