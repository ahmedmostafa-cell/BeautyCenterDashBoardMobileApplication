using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JamalKhanah.Core.DTO;
using JamalKhanah.Core.DTO.EntityDto;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.ProfileData;
using JamalKhanah.Core.Helpers;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;

namespace JamalKhanah.Controllers.API;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AddressesController : BaseApiController , IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseResponse _baseResponse;
    private  ApplicationUser _user;
    public AddressesController(IUnitOfWork unitOfWork)
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
        
        
    // GET: api/Addresses
    [HttpGet]
    public async Task<ActionResult<BaseResponse>> Addresses([FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        
        var addresses = await _unitOfWork.Addresses.FindByQuery(
            criteria: s => s.UserId == _user.Id&& s.IsDeleted==false)
            .Select(s=>new
            {
                s.Id,
                s.CityId,
                CityName = lang == "ar" ? s.City.NameAr : s.City.NameEn,
                s.Region,
                s.Street,
                s.BuildingNumber,
                s.FlatNumber,
                s.AddressDetails
            }).ToListAsync();

        if (!addresses.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.AddressNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد عناوين للمستخدم ": "Address Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = addresses;
        return Ok(_baseResponse);
    }

    // GET: api/Addresses/5
    [HttpGet("{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> Address([FromHeader] string lang,int id)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        
        var address = await _unitOfWork.Addresses.FindByQuery
            (criteria: s => s.Id == id && s.UserId == _user.Id&& s.IsDeleted==false).Select(s=>new
        {
            s.Id,
            CityName = lang == "ar" ? s.City.NameAr : s.City.NameEn,
            s.Region,
            s.Street,
            s.BuildingNumber,
            s.FlatNumber,
            s.AddressDetails
        }).FirstOrDefaultAsync();
        if (address == null)
        {
            _baseResponse.ErrorCode = (int)Errors.AddressNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد عناوين للمستخدم " : "Address Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = address;
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    [HttpPut]
    public async Task<ActionResult<BaseResponse>> UpdateAddress([FromHeader] string lang, UpdateAddressDto address)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }   
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var addressFromDb = await _unitOfWork.Addresses.FindByQuery
            (criteria: s => s.Id == address.Id && s.UserId == _user.Id && s.IsDeleted==false).FirstOrDefaultAsync();

        if (addressFromDb == null)
        {
            _baseResponse.ErrorCode = (int)Errors.AddressNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد عناوين للمستخدم " : "Address Not Found";
            return Ok(_baseResponse);
        }

        var city = await _unitOfWork.Cities.FindByQuery(criteria: s => s.Id == address.CityId && s.IsDeleted==false).FirstOrDefaultAsync();
        if (city == null)
        {
            _baseResponse.ErrorCode = (int)Errors.CityNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "المدينة غير موجودة " : "City Not Found";
            return Ok(_baseResponse);
        }

        addressFromDb.Region = address.Region;
        addressFromDb.Street = address.Street;
        addressFromDb.BuildingNumber = address.BuildingNumber;
        addressFromDb.FlatNumber = address.FlatNumber;
        addressFromDb.CityId = address.CityId;
        addressFromDb.AddressDetails = address.AddressDetails;
        addressFromDb.IsUpdated = true;
        addressFromDb.UpdatedAt = DateTime.Now;
        _unitOfWork.Addresses.Update(addressFromDb);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم تعديل العنوان بنجاح" : "Address Updated Successfully";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------
 
    [HttpPost]
    public async Task<ActionResult<BaseResponse>> Address([FromHeader] string lang,AddressDto address)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        
        var city = await _unitOfWork.Cities.FindByQuery(criteria: s => s.Id == address.CityId && s.IsDeleted==false).FirstOrDefaultAsync();
        if (city == null)
        {
            _baseResponse.ErrorCode = (int)Errors.CityNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "المدينة غير موجودة " : "City Not Found";
            return Ok(_baseResponse);
        }
        var newAddress = new Address
        {
            Region = address.Region,
            Street = address.Street,
            BuildingNumber = address.BuildingNumber,
            FlatNumber = address.FlatNumber,
            CityId = address.CityId,
            AddressDetails = address.AddressDetails,
            UserId = _user.Id,
            IsUpdated = false,
            CreatedAt = DateTime.Now
        };


        await _unitOfWork.Addresses.AddAsync(newAddress);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم اضافة العنوان بنجاح" : "Address Added Successfully";
        return Ok(_baseResponse);
            
    }

    // DELETE: api/Addresses/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<BaseResponse>> DeleteAddress([FromHeader] string lang,int id)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        
        var address = await _unitOfWork.Addresses.FindByQuery
            (criteria: s => s.Id == id && s.UserId == _user.Id && s.IsDeleted==false ).FirstOrDefaultAsync();
        if (address == null)
        {
            _baseResponse.ErrorCode = (int)Errors.AddressNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد عناوين للمستخدم " : "Address Not Found";
            return Ok(_baseResponse);
        }
        address.IsDeleted = true;
        address.DeletedAt = DateTime.Now;
        _unitOfWork.Addresses.Update(address);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم حذف العنوان بنجاح" : "Address Deleted Successfully";
        return Ok(_baseResponse);

    }

    
}