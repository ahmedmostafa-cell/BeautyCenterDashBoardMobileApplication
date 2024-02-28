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
public class ExperiencesController : BaseApiController, IActionFilter
{
    private readonly BaseResponse _baseResponse;
    private readonly IUnitOfWork _unitOfWork;
    private ApplicationUser _user;

    public ExperiencesController(IUnitOfWork unitOfWork)
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

    // GET: api/Experiences
    [HttpGet]
    public async Task<ActionResult<BaseResponse>> Experiences([FromHeader] string lang)
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

        var experiences = await _unitOfWork.Experiences.FindByQuery(
            s => s.UserId == _user.Id && s.IsDeleted == false)
            .Select( s=> new
                {
                s.Id,
                s.Title,
                s.Description,
            }).ToListAsync();

        if (!experiences.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.ExperienceNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? "لا توجد خبرات للمستخدم " : "Experience Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar" ? "تم الحصول على الخبرات بنجاح " : "Get Experiences Successfully";
        _baseResponse.Data = experiences;
        return Ok(_baseResponse);
    }

    // GET: api/Experiences/5
    [HttpGet("{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> Experience(int id, [FromHeader] string lang)
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

        var experience = await _unitOfWork.Experiences.FindByQuery(
            s => s.Id == id && s.UserId == _user.Id && s.IsDeleted == false)
            .Select( s=> new
            {
                s.Id,
                s.Title,
                s.Description,
            }).FirstOrDefaultAsync();

        if (experience == null)
        {
            _baseResponse.ErrorCode = (int)Errors.ExperienceNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? "لا توجد خبرة للمستخدم " : "Experience Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar" ? "تم الحصول على الخبرة بنجاح " : "Get Experience Successfully";
        _baseResponse.Data = experience;
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    // PUT: api/Experiences/5
    [HttpPut]
    public async Task<ActionResult<BaseResponse>> PutExperience(UpdateExperienceDto experience, [FromHeader] string lang)
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
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        
        var experienceDb = await _unitOfWork.Experiences.FindByQuery(
            s => s.Id == experience.Id && s.UserId == _user.Id && s.IsDeleted == false).FirstOrDefaultAsync();

        
        if (experienceDb == null)
        {
            _baseResponse.ErrorCode = (int)Errors.ExperienceNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? "لا توجد خبرة للمستخدم " : "Experience Not Found";
            return Ok(_baseResponse);
        }

        experienceDb.Title = experience.Title;
        experienceDb.Description = experience.Description;
        experienceDb.UpdatedAt = DateTime.Now;
        _unitOfWork.Experiences.Update(experienceDb);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar" ? "تم تعديل الخبرة بنجاح " : "Update Experience Successfully";
        _baseResponse.Data = new
        {
            experienceDb.Id,
            experienceDb.Title,
            experienceDb.Description,
        };
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    // POST: api/Experiences
    [HttpPost]
    public async Task<ActionResult<BaseResponse>> PostExperience(ExperienceDto experience, [FromHeader] string lang)
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
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var  newExperience = new Experience
        {
            Title = experience.Title,
            Description = experience.Description,
            UserId = _user.Id,
            CreatedAt = DateTime.Now
        };


        await _unitOfWork.Experiences.AddAsync(newExperience);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar" ? "تم اضافة الخبرة بنجاح " : "Add Experience Successfully";
        _baseResponse.Data = new
        {
            newExperience.Id,
            newExperience.Title,
            newExperience.Description,
        };
        return Ok(_baseResponse);
    }

    // DELETE: api/Experiences/5
    [HttpDelete("{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> DeleteExperience(int id, [FromHeader] string lang)
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
        var experience = await _unitOfWork.Experiences.FindByQuery(
            s => s.Id == id && s.UserId == _user.Id && s.IsDeleted == false).FirstOrDefaultAsync();

        if (experience == null)
        {
            _baseResponse.ErrorCode = (int)Errors.ExperienceNotFound;
            _baseResponse.ErrorMessage = lang == "ar" ? "لا توجد خبرة للمستخدم " : "Experience Not Found";
            return Ok(_baseResponse);
        }

        experience.IsDeleted = true;
        experience.DeletedAt = DateTime.Now;
        _unitOfWork.Experiences.Update(experience);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = lang == "ar" ? "تم حذف الخبرة بنجاح " : "Delete Experience Successfully";
        return Ok(_baseResponse);
    }
}