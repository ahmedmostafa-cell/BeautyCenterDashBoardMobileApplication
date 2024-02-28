using JamalKhanah.BusinessLayer.Interfaces;
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
public class WorkHoursController : BaseApiController, IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileHandling _fileHandling;
    private readonly BaseResponse _baseResponse;
    private  ApplicationUser _user;
    public WorkHoursController(IUnitOfWork unitOfWork, IFileHandling fileHandling)
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
    // GET: api/WorkHours
    [HttpGet]
    public async Task<ActionResult<BaseResponse>> WorkHours([FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        var workHours = await _unitOfWork.WorksHours.FindByQuery(
            criteria: s =>  s.UserId == _user.Id  && s.IsDeleted == false)
            .Select(s=>new
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

        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = workHours;
        return Ok(_baseResponse);
    }

    // GET: api/WorkHours/5
    [HttpGet("{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> WorkHour([FromHeader] string lang, int id)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        var workHour = await _unitOfWork.WorksHours.FindByQuery(
            criteria: s => s.Id == id && s.UserId == _user.Id && s.IsDeleted == false)
            .Select(s=>new
            {
                s.Id,
                s.Day,
                s.From,
                s.To,
                s.MoreData
            }).FirstOrDefaultAsync();

        if (workHour == null)
        {
            _baseResponse.ErrorCode = (int)Errors.WorkHourNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد أوقات عمل للمستخدم " : " Work hour Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = workHour;
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    // PUT: api/WorkHours/5
    [HttpPut()]
    public async Task<ActionResult<BaseResponse>> UpdateWorkHour([FromHeader] string lang, UpdateWorkHourDto workHour)
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
        var workHourToUpdate = await _unitOfWork.WorksHours.FindByQuery(
                criteria: s => s.Id == workHour.Id && s.UserId == _user.Id && s.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (workHourToUpdate == null)
        {
            _baseResponse.ErrorCode = (int)Errors.WorkHourNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد أوقات عمل للمستخدم " : " Work hour Not Found";
            return Ok(_baseResponse);
        }

       
        workHourToUpdate.Day = workHour.Day;
        workHourToUpdate.From = new TimeSpan(TimeOnly.FromDateTime(workHour.From).Ticks);
        workHourToUpdate.To = new TimeSpan(TimeOnly.FromDateTime(workHour.To).Ticks);
        workHourToUpdate.MoreData = workHour.MoreData;
        workHourToUpdate.IsUpdated = true;
        workHourToUpdate.UpdatedAt = DateTime.Now; 
        _unitOfWork.WorksHours.Update(workHourToUpdate);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم تعديل وقت العمل  بنجاح" : "WorkHour Updated Successfully";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    // POST: api/WorkHours
    [HttpPost]
    public async Task<ActionResult<BaseResponse>> AddWorkHour([FromHeader] string lang, List<WorkHourDto>  workHour)
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

        if (_user.UserType != UserType.Center && _user.UserType != UserType.FreeAgent)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا يملك الحساب صلاحية اضافة أوقات عمل " : "The account does not have the authority to add WorkHours ";
            return Ok(_baseResponse);
        }
     
        foreach(var i in workHour) 
        {
            var newWorkHour = new WorkHours()
            {
                Day = i.Day,
                From = new TimeSpan(TimeOnly.FromDateTime(i.From).Ticks),
                To = new TimeSpan(TimeOnly.FromDateTime(i.To).Ticks),
                MoreData =i.MoreData,
                UserId = _user.Id,
                IsDeleted = false,
                IsUpdated = false
            };
            await _unitOfWork.WorksHours.AddAsync(newWorkHour);
            await _unitOfWork.SaveChangesAsync();

        }
       

  
        
       

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم اضافة وقت العمل  بنجاح" : "WorkHour Added Successfully";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    // DELETE: api/WorkHours/5
    [HttpDelete("{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> DeleteWorkHour([FromHeader] string lang, int id)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var workHour = await _unitOfWork.WorksHours.FindByQuery(
                criteria: s => s.Id == id  && s.UserId == _user.Id && s.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (workHour == null)
        {
            _baseResponse.ErrorCode = (int)Errors.WorkHourNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد أوقات عمل للمستخدم " : " Work hour Not Found";
            return Ok(_baseResponse);
        }
        workHour.IsDeleted = true;
        workHour.DeletedAt = DateTime.Now;
        _unitOfWork.WorksHours.Update(workHour);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم حذف وقت العمل  بنجاح" : "WorkHour Deleted Successfully";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------
    [HttpGet("WorkHoursByProvider")]
    public async Task<ActionResult<BaseResponse>> WorkHoursByProvider([FromHeader] string lang, string providerId)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        var workHours = await _unitOfWork.WorksHours.FindByQuery(
                criteria: s => s.UserId == providerId && s.IsDeleted == false)
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

        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = workHours;
        return Ok(_baseResponse);
    }
}