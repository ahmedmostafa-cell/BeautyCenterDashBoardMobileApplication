using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.DTO;
using JamalKhanah.Core.DTO.EntityDto;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.CouponData;
using JamalKhanah.Core.Entity.OrderData;
using JamalKhanah.Core.Helpers;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualBasic;

namespace JamalKhanah.Controllers.API;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OrderUsersController : BaseApiController, IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileHandling _fileHandling;
    private readonly BaseResponse _baseResponse;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private ApplicationUser _user;

    public OrderUsersController(IUnitOfWork unitOfWork, IFileHandling fileHandling, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _fileHandling = fileHandling;
        _baseResponse = new BaseResponse();
        _httpContextAccessor = httpContextAccessor;
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


    /*   [HttpGet("OrderStatus")]
       [AllowAnonymous]
       public ActionResult<BaseResponse> GetOrderStatus()
       {
           var orderStatus = new
           {
               Initialized = new
               {
                   id = (int)OrderStatus.Initialized,
                   description = "تحت الانشاء لايزال مع العميل",
                   ForUser = true,
               },
               Preparing = new
               {
                   id = (int)OrderStatus.Preparing,
                   description = "تم التأكيد من العميل ولم يتم الموافقة عليه بعد من قبل مقدم الخدمة ",
                   ForUser = true,
               },
               Confirmed = new
               {
                   id = (int)OrderStatus.Confirmed,
                   description = "تم الموافقة عليه من قبل مقدم الخدمة",
                   ForUser = false
               },
               Rejected = new
               {
                   id = (int)OrderStatus.Rejected,
                   description = "تم الرفض من قبل مقدم الخدمة ",
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
       public async Task<ActionResult<BaseResponse>> ChangeOrderStatus([FromHeader] string lang, int id,
           int orderStatus)
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
                                  s.UserId == _user.Id &&
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
                   order.OrderStatus = OrderStatus.Preparing;
                   break;

               case (int)OrderStatus.Cancelled:
                   _baseResponse.ErrorCode = (int)Errors.InvalidData;
                   _baseResponse.ErrorMessage = lang == "ar"
                       ? "لا يمكن تغيير الحالة الى ملغية "
                       : "Can't Change Status To Cancelled ";
                   return Ok(_baseResponse);

               case (int)OrderStatus.Finished:
                   _baseResponse.ErrorCode = (int)Errors.InvalidData;
                   _baseResponse.ErrorMessage = lang == "ar"
                       ? "لا يمكن تغيير الحالة الى منهية "
                       : "Can't Change Status To Finished ";
                   return Ok(_baseResponse);

               case (int)OrderStatus.Confirmed:
                   _baseResponse.ErrorCode = (int)Errors.InvalidData;
                   _baseResponse.ErrorMessage = lang == "ar"
                       ? "لا يمكن تغيير الحالة الى مؤكدة "
                       : "Can't Change Status To Confirmed ";
                   return Ok(_baseResponse);

               case (int)OrderStatus.Rejected:
                   _baseResponse.ErrorCode = (int)Errors.InvalidData;
                   _baseResponse.ErrorMessage = lang == "ar"
                       ? "لا يمكن تغيير الحالة الى مرفوضة "
                       : "Can't Change Status To Rejected ";
                   return Ok(_baseResponse);

               case (int)OrderStatus.WithDriver:
                   _baseResponse.ErrorCode = (int)Errors.InvalidData;
                   _baseResponse.ErrorMessage = lang == "ar"
                       ? "لا يمكن تغيير الحالة الى مع سائق "
                       : "Can't Change Status To With Driver ";
                   return Ok(_baseResponse);
           }

           _unitOfWork.Orders.Update(order);
           await _unitOfWork.SaveChangesAsync();

           _baseResponse.ErrorCode = (int)Errors.Success;
           _baseResponse.ErrorMessage = lang == "ar"
               ? "تم تغيير حالة الطلب بنجاح "
               : "Change Order Status Successfully ";
           return Ok(_baseResponse);

       }*/

    //-----------------------------------------------------------------------------------------------------
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
                               s.UserId == _user.Id &&
                               s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                s.CreatedOn,
                s.Total,
                s.OrderStatus,
                s.InHome,
                Coupon=s.Coupon==null? null: s.Coupon.CouponCode,
                s.Discount,
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
                    s.Service.ImgUrl
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
                    s.User.Email
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

	[HttpPut("AddCouponToOrder")]
	public async Task<ActionResult<BaseResponse>> AddCouponToOrder([FromHeader] string lang, OrderCouponDto model)
	{
		if (_user == null)
		{
			_baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
			_baseResponse.ErrorMessage = lang == "ar"
				? "هذا الحساب غير موجود "
				: "The User Not Exist ";
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
        var orders = await _unitOfWork.Orders.FindByQuery(criteria: s =>s.UserId == _user.Id  && s.IsDeleted == false,include: s=>s.Include(order=>order.Coupon),isNoTracking: true)
			.ToListAsync();
        if (!orders.Any())
        {
	        _baseResponse.ErrorCode = (int)Errors.NoData;
	        _baseResponse.ErrorMessage = lang == "ar"
		        ? "لا يوجد طلبات "
		        : "No Orders ";
	        return Ok(_baseResponse);
        }
        var coupon = await _unitOfWork.Coupons.FindByQuery(criteria: s => s.CouponCode == model.CouponCode && s.IsDeleted == false && s.IsActive == true, 
	        include: s=>s.Include(coupon=>coupon.MainSections).Include(coupon=>coupon.Services).Include(coupon=>coupon.Providers)
	        ).FirstOrDefaultAsync();
        if (coupon == null)
        {
	        _baseResponse.ErrorCode = (int)Errors.NoData;
	        _baseResponse.ErrorMessage = lang == "ar"
		        ? "لا يوجد كوبون "
		        : "No Coupon ";
	        return Ok(_baseResponse);
		}

        if (orders.Any(s => s.CouponId == coupon.Id))
        {
			_baseResponse.ErrorCode = (int)Errors.InvalidData;
			_baseResponse.ErrorMessage = lang == "ar"
				? "هذا الكوبون مستخدم من قبل "
				: "This Coupon Used Before ";
			return Ok(_baseResponse);
		}

        var order =await _unitOfWork.Orders.FindByQuery(s => s.OrderStatus == OrderStatus.Initialized && s.Id == model.OrderId && s.CouponId== null && s.UserId == _user.Id && s.IsDeleted == false,
	        include:s=>s.Include(order=>order.Service).ThenInclude(service=>service.MainSection).Include(order => order.Service).ThenInclude(service => service.Provider)).FirstOrDefaultAsync();
        if (order == null)
        {
	        _baseResponse.ErrorCode = (int)Errors.NoData;
	        _baseResponse.ErrorMessage = lang == "ar"
		        ? "لا يوجد طلب بهذا الرقم "
		        : "No Order have this Id ";
	        return Ok(_baseResponse);
        }

        if (coupon.ExpireDate!= null && coupon.ExpireDate> DateTime.Now)
        {
            _baseResponse.ErrorCode = (int)Errors.InvalidData;
	        _baseResponse.ErrorMessage = lang == "ar"
		        ? "هذا الكوبون منتهي الصلاحية "
		        : "This Coupon Expired ";
	        return Ok(_baseResponse);
        }

		switch (coupon.CouponType)
		{
			case CouponType.MainSections:
			{
				if (coupon.MainSections.All(s => s.MainSectionId != order.Service.MainSectionId))
				{
					_baseResponse.ErrorCode = (int)Errors.InvalidData;
					_baseResponse.ErrorMessage = lang == "ar"
						? "هذا الكوبون غير متاح لهذا القسم "
						: "This Coupon Not Available For This Section ";
					return Ok(_baseResponse);
				}

				break;
			}
			case CouponType.Service:
			{
				if (coupon.Services.All(s => s.ServiceId != order.ServiceId))
				{
					_baseResponse.ErrorCode = (int)Errors.InvalidData;
					_baseResponse.ErrorMessage = lang == "ar"
						? "هذا الكوبون غير متاح لهذه الخدمة "
						: "This Coupon Not Available For This Service ";
					return Ok(_baseResponse);
				}

				break;
			}
			case CouponType.ServiceProvider:
			{
				if (coupon.Providers.All(s => s.ProviderId != order.Service.ProviderId))
				{
					_baseResponse.ErrorCode = (int)Errors.InvalidData;
					_baseResponse.ErrorMessage = lang == "ar"
						? "هذا الكوبون غير متاح لهذا المقدم "
						: "This Coupon Not Available For This Provider ";
					return Ok(_baseResponse);
				}

				break;
			}
			case CouponType.None:
			default:
				_baseResponse.ErrorCode = (int)Errors.InvalidData;
				_baseResponse.ErrorMessage = lang == "ar"
					? "هذا الكوبون غير متاح "
					: "This Coupon Not Available ";
				return Ok(_baseResponse);
		}

        if (coupon.DiscountType == DiscountType.Percentage)
        {
	        order.Discount = (order.Total * coupon.Discount / 100);
			order.Total -= (order.Total * coupon.Discount / 100);
           
		}
        else
        {
	        order.Discount = coupon.Discount;
			order.Total -= coupon.Discount;
        }
        order.CouponId = coupon.Id;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();
        _unitOfWork.Orders.DeAttach(order);


        

	var newOrder = await _unitOfWork.Orders.FindByQuery(
				criteria: s => s.Id == model.OrderId &&
							   s.UserId == _user.Id &&
							   s.IsDeleted == false)
			.Select(s => new
			{
				s.Id,
				s.CreatedOn,
				s.Total,
				s.OrderStatus,
				s.InHome,
                s.Discount,
                Coupon = s.Coupon == null ? null : s.Coupon.CouponCode,
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
					s.Service.ImgUrl
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
					s.User.Email
				}
			}).FirstOrDefaultAsync();

		if (newOrder == null)
		{
			_baseResponse.ErrorCode = (int)Errors.NoData;
			_baseResponse.ErrorMessage = lang == "ar"
				? "لا يوجد طلبات "
				: "No Orders ";
			return Ok(_baseResponse);
		}

		_baseResponse.ErrorCode = (int)Errors.Success;
		_baseResponse.Data = newOrder;
		return Ok(_baseResponse);
	}

	//-----------------------------------------------------------------------------------------------------
	[HttpGet("GetOrders")]
    public async Task<ActionResult<BaseResponse>> GetOrders([FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var orders = await _unitOfWork.Orders.FindByQuery(
                criteria: s => s.UserId == _user.Id &&
                               s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                s.CreatedOn,
                s.Total,
                s.OrderStatus,
                s.InHome,
                Coupon = s.Coupon == null ? null : s.Coupon.CouponCode,
                s.Discount,
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
                    s.Service.ImgUrl
                },
                Provider = new
                {
                    s.Service.Provider.Id,
                    s.Service.Provider.FullName,
                    s.Service.Provider.PhoneNumber,
                    s.Service.Provider.Email,
                }
            }).OrderByDescending(a=> a.CreatedOn).ToListAsync();

        if (!orders.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.NoData;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "لا يوجد طلبات "
                : "No Orders ";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.Data = orders;
        return Ok(_baseResponse);
    }

    //--------------------------------------------------------------------------------------------------------
    [HttpGet("GetOrdersByStatus/{status:required:int}")]
    public async Task<ActionResult<BaseResponse>> GetOrdersByStatus([FromHeader] string lang, int status)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var orders = await _unitOfWork.Orders.FindByQuery(
                criteria: s => s.UserId == _user.Id &&
                               s.OrderStatus == (OrderStatus)status &&
                               s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                s.CreatedOn,
                s.Total,
                s.OrderStatus,
                s.InHome,
                Coupon = s.Coupon == null ? null : s.Coupon.CouponCode,
                s.Discount,
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
                    s.Service.ImgUrl
                },
                Provider = new
                {
                    s.Service.Provider.Id,
                    s.Service.Provider.FullName,
                    s.Service.Provider.PhoneNumber,
                    s.Service.Provider.Email,
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
        _baseResponse.Data = orders;
        return Ok(_baseResponse);
    }

    //--------------------------------------------------------------------------------------------------------
    // Create order 
    [HttpPost("AddOrder")]
    public async Task<ActionResult<BaseResponse>> AddOrder([FromHeader] string lang, [FromBody] OrderDto orderDto)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
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

        var service = await _unitOfWork.Services.FindByQuery(
                criteria: s => s.Id == orderDto.ServiceId &&
                               s.IsAvailable == true &&
                               s.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (service == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheServiceNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذه الخدمة غير موجودة "
                : "The Service Not Exist ";
            return Ok(_baseResponse);
        }

        var address = await _unitOfWork.Addresses.FindByQuery(
                criteria: s => s.Id == orderDto.AddressId && s.UserId == _user.Id&&
                               s.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (address == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheAddressNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا العنوان غير موجود "
                : "The Address Not Exist ";
            return Ok(_baseResponse);
        }
        var workHours = await _unitOfWork.WorksHours.FindByQuery(
          criteria: s => s.UserId == service.ProviderId && s.IsDeleted == false)
          .Select(s => new
          {
              s.Id,
              s.Day,
              s.From,
              s.To,
              s.MoreData
          }).ToListAsync();

        if (!workHours.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.WorkHourNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد أوقات عمل للمستخدم " : " Work hour Not Found";
            return Ok(_baseResponse);
        }

        //foreach (var i in workHours)
        //{
        //    var a = orderDto.StartingOn.Date.ToString("HH:mm"); 
        //    if (orderDto.StartingOn.Date.ToString("dd/MM/yyyy") == i.MoreData) 
        //    {


        //    }
        //    //int hours = (int)duration.Value.TotalMinutes;
        //    //if (hours > 0 && hours < leastHoursBetweenConsultation)
        //    //{

        //    //    myString = "لم يتم الحجز لوجود استشارة بعد الميعاد المطلوب باقل من الوقت المحدد من الادمن";
        //    //    MyModelStringObject.MyString = myString;
        //    //    MyModelStringObject.MyModel = new TbConsultingEstablish();
        //    //    return MyModelStringObject;
        //    //}
        //    //else if (hours < 0 && hours > (leastHoursBetweenConsultation * -1))
        //    //{
        //    //    MyModelStringObject = new MyModelStringObject();
        //    //    myString = "لم يتم الحجز لوجود استشارة قبل الميعاد المطلوب باقل من الوقت المحدد من الادمن";
        //    //    MyModelStringObject.MyString = myString;
        //    //    MyModelStringObject.MyModel = new TbConsultingEstablish();
        //    //    return MyModelStringObject;
        //    //}
        //    //else if (hours == 0)
        //    //{
        //    //    MyModelStringObject = new MyModelStringObject();
        //    //    myString = "توجد استشارة بنفس الميعاد";
        //    //    MyModelStringObject.MyString = myString;
        //    //    MyModelStringObject.MyModel = new TbConsultingEstablish();
        //    //    return MyModelStringObject;
        //    //}

        //}

        var order = new Order
        {
            UserId = _user.Id,
            ServiceId = service.Id,
            Total = service.FinalPrice,
            OrderStatus = OrderStatus.Initialized,
            CreatedOn = DateTime.Now,
            IsDeleted = false,
            CityId = address.CityId,
            Region = address.Region,
            Street = address.Street,
            BuildingNumber = address.BuildingNumber,
            FlatNumber = address.FlatNumber,
            AddressDetails = address.AddressDetails,
            PaymentMethod = PaymentMethod.Online,
            StartingOn = orderDto.StartingOn,
            IsPaid =  false,

        };



        order.InHome = service.InCenter switch
        {
            true when service.InHome => orderDto.InHome,
            true when service.InHome == false => false,
            false when service.InHome => true,
            _ => order.InHome
        };

        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم اضافة الطلب بنجاح "
            : "Add Order Successfully ";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------------
    // confirm order
    [HttpPost("ConfirmOrder/{orderId:required:int}")]
    public async Task<ActionResult<BaseResponse>> ConfirmOrder([FromHeader] string lang, int orderId)
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
                criteria: s => s.Id == orderId &&
                               s.UserId == _user.Id &&
                               s.OrderStatus == OrderStatus.Initialized &&
                               s.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (order == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheOrderNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الطلب غير موجود "
                : "The Order Not Exist ";
            return Ok(_baseResponse);
        }

        if (order.PaymentMethod == PaymentMethod.Online && !order.IsPaid)
        {
            order.PaymentUrlIdentifier = Guid.NewGuid().ToString();
            await _unitOfWork.SaveChangesAsync();
            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.Data = $"{$"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}"}/payment/InitializePayment?id={order.PaymentUrlIdentifier}";
            return Ok(_baseResponse);
        }

        order.OrderStatus = OrderStatus.Preparing;
        order.UpdatedAt = DateTime.Now;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();
        
        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم تأكيد الطلب بنجاح "
            : "Confirm Order Successfully ";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------------
    // Remove order
    [HttpPost("RemoveOrder/{orderId:required:int}")]
    public async Task<ActionResult<BaseResponse>> RemoveOrder([FromHeader] string lang, int orderId)
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
                criteria: s => s.Id == orderId &&
                               s.UserId == _user.Id &&
                               s.OrderStatus == OrderStatus.Initialized &&
                               s.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (order == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheOrderNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الطلب غير موجود "
                : "The Order Not Exist ";
            return Ok(_baseResponse);
        }

        _unitOfWork.Orders.Delete(order);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم حذف الطلب بنجاح "
            : "delete Order Successfully ";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------------
    // cancel order 
    [HttpPost("CancelOrder/{orderId:required:int}")]
    public async Task<ActionResult<BaseResponse>> CancelOrder([FromHeader] string lang, int orderId)
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
                criteria: s => s.Id == orderId &&
                               s.UserId == _user.Id &&
                               s.OrderStatus == OrderStatus.Preparing &&
                               s.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (order == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheOrderNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الطلب غير موجود "
                : "The Order Not Exist ";
            return Ok(_baseResponse);
        }

        order.OrderStatus = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.Now;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar"
            ? "تم الغاء الطلب بنجاح "
            : "Cancel Order Successfully ";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------------
    // Remove Coupon
    [HttpPost("RemoveCoupon")]
    public async Task<ActionResult<BaseResponse>> RemoveCoupon([FromHeader] string lang, OrderCouponDto model)
    {
	    if (_user == null)
	    {
		    _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
		    _baseResponse.ErrorMessage = lang == "ar"
			    ? "هذا الحساب غير موجود "
			    : "The User Not Exist ";
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
	    var order = await _unitOfWork.Orders.FindByQuery(s => s.OrderStatus == OrderStatus.Initialized && s.Id == model.OrderId && s.Coupon.CouponCode==model.CouponCode && s.UserId == _user.Id && s.IsDeleted == false,
			    include: s=>s.Include(order=>order.Coupon))
		    .FirstOrDefaultAsync();
	    if (order == null)
	    {
		    _baseResponse.ErrorCode = (int)Errors.NoData;
		    _baseResponse.ErrorMessage = lang == "ar"
			    ? "لا يوجد طلب بهذا الرقم مع هذا الكوبون "
			    : "No Order have this Id and this coupon code  ";
		    return Ok(_baseResponse);
	    }

	    order.Total += order.Discount;
	    order.Discount = 0;
	    order.CouponId = null;
	    order.Coupon = null;
	    order.UpdatedAt = DateTime.Now;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();
        _unitOfWork.Orders.DeAttach(order);




        var newOrder = await _unitOfWork.Orders.FindByQuery(
		        criteria: s => s.Id == model.OrderId &&
		                       s.UserId == _user.Id &&
		                       s.IsDeleted == false)
	        .Select(s => new
	        {
		        s.Id,
		        s.CreatedOn,
		        s.Total,
		        s.OrderStatus,
		        s.InHome,
		        s.Discount,
		        Coupon = s.Coupon == null ? null : s.Coupon.CouponCode,
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
			        s.Service.ImgUrl
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
			        s.User.Email
		        }
	        }).FirstOrDefaultAsync();

        if (newOrder == null)
        {
	        _baseResponse.ErrorCode = (int)Errors.NoData;
	        _baseResponse.ErrorMessage = lang == "ar"
		        ? "لا يوجد طلبات "
		        : "No Orders ";
	        return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.Data = newOrder;
        return Ok(_baseResponse);

	}
}