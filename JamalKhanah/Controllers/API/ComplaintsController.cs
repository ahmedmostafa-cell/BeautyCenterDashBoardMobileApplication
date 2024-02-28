using JamalKhanah.Core.DTO;
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
public class ComplaintsController : BaseApiController , IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseResponse _baseResponse;
    private  ApplicationUser _user;
    public ComplaintsController(IUnitOfWork unitOfWork)
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
        
       

    [HttpGet("GetComplaints")]
    public async Task<IActionResult> Get([FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }

        var complaints = await _unitOfWork.Complaints.FindByQuery(
                s => s.UserId == _user.Id, include: s => s.Include(i => i.User))
            .Select(s => new { s.Data, s.CreatedAt, s.User.UserName, IsAnswered=s.IsDeleted }).ToListAsync();

        if (!complaints.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.NotFoundComplaints;
            _baseResponse.ErrorMessage = (lang != "ar") ? "No old complaints" : " لا توجد شكاوي قديمة ";
            return Ok(_baseResponse);
        }
            
        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = complaints;
        return Ok(_baseResponse);
    }

    [HttpPost("AddComplaint")]
    public async Task<IActionResult> Add([FromHeader] string lang ,ComplaintDto model)
    {
         
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود   "
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

        var complaint = new Complaint()
        {
            Data = model.Data,
            Title = model.Title,
            UserId = _user.Id
        };
        await _unitOfWork.Complaints.AddAsync(complaint);
        await _unitOfWork.SaveChangesAsync();

          
           
            
        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تمت أضافة الشكوي بنجاح " : "Complaint added successfully";
        return Ok(_baseResponse);
    }

}

