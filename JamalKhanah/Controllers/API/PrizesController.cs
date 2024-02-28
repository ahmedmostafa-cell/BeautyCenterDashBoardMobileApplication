using JamalKhanah.Core.DTO;
using JamalKhanah.Core.DTO.EntityDto;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.ProfileData;
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
public class PrizesController : BaseApiController, IActionFilter
{
    private readonly BaseResponse _baseResponse;
    private readonly IUnitOfWork _unitOfWork;
    private ApplicationUser _user;

    public PrizesController(IUnitOfWork unitOfWork)
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
    //  GET: api/Prizes
    [HttpGet]
    public async Task<ActionResult<BaseResponse>> Prizes([FromHeader] string lang)
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
        var prizes = await _unitOfWork.Prizes.FindByQuery(
            s => s.UserId == _user.Id && s.IsDeleted == false)
            .Select(s=>new {s.Id, s.Title, s.Description}).ToListAsync();

        if (!prizes.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.PrizeNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? "لا توجد جوائز للمستخدم " : "Prize Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.Data = prizes;
        return Ok(_baseResponse);
    }

    // GET: api/Prizes/5
    [HttpGet("{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> Prize(int id, [FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = lang == "ar"
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var prize = await _unitOfWork.Prizes.FindByQuery(
            s => s.UserId == _user.Id && s.IsDeleted == false && s.Id == id)
            .Select(s=>new {s.Id, s.Title, s.Description}).FirstOrDefaultAsync();

        if (prize == null)
        {
            _baseResponse.ErrorCode = (int)Errors.PrizeNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? "لا توجد جائزة للمستخدم " : "Prize Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.Data = prize;
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    // PUT: api/Prizes/5
    [HttpPut()]
    public async Task<ActionResult<BaseResponse>> UpdatePrize(UpdatePrizeDto prize, [FromHeader] string lang)
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

        var prizeDb = await _unitOfWork.Prizes.FindByQuery(
            s => s.UserId == _user.Id && s.IsDeleted == false && s.Id == prize.Id).FirstOrDefaultAsync();

        if (prizeDb == null)
        {
            _baseResponse.ErrorCode = (int)Errors.PrizeNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? "لا توجد جائزة للمستخدم " : "Prize Not Found";
            return Ok(_baseResponse);
        }

        prizeDb.Title = prize.Title;
        prizeDb.Description = prize.Description;
        prizeDb.IsUpdated = true;
        prizeDb.UpdatedAt = DateTime.Now;

        _unitOfWork.Prizes.Update(prizeDb);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم تعديل الجائزة بنجاح" : "Prize Updated Successfully";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    // POST: api/Prizes
    [HttpPost]
    public async Task<ActionResult<BaseResponse>> AddPrize(PrizeDto prize, [FromHeader] string lang)
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

        var newPrize = new Prize
        {
            Title = prize.Title,
            Description = prize.Description,
            UserId = _user.Id,
            CreatedAt = DateTime.Now,
            IsDeleted = false,
            IsUpdated = false
        };


        await _unitOfWork.Prizes.AddAsync(newPrize);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم اضافة الجائزة بنجاح" : "Prize Added Successfully";
        return Ok(_baseResponse);
    }

    // DELETE: api/Prizes/5
    [HttpDelete("{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> DeletePrize(int id, [FromHeader] string lang)
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

        var prize = await _unitOfWork.Prizes.FindByQuery(
            s => s.UserId == _user.Id && s.IsDeleted == false && s.Id == id).FirstOrDefaultAsync();

        if (prize == null)
        {
            _baseResponse.ErrorCode = (int)Errors.PrizeNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? "لا توجد جائزة للمستخدم " : "Prize Not Found";
            return Ok(_baseResponse);
        }

        prize.IsDeleted = true;
        prize.DeletedAt = DateTime.Now;

        _unitOfWork.Prizes.Update(prize);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم حذف الجائزة بنجاح" : "Prize Deleted Successfully";
        return Ok(_baseResponse);
    }


}