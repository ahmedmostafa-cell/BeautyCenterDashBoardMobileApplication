using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.DTO;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Helpers;
using JamalKhanah.Core.ModelView.AuthViewModel.LoginData;
using JamalKhanah.Core.ModelView.AuthViewModel.RegisterData;
using JamalKhanah.Core.ModelView.AuthViewModel.RoleData;
using JamalKhanah.RepositoryLayer.Interfaces;
using JamalKhanah.Core.ModelView.AuthViewModel.ChangePasswordData;
using JamalKhanah.Core.ModelView.AuthViewModel.UpdateData;

namespace JamalKhanah.Controllers.API;

public class UsersController : BaseApiController
{
        
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IAccountService _accountService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseResponse _baseResponse = new();

    public UsersController(IUnitOfWork unitOfWork, IAccountService accountService, RoleManager<ApplicationRole> roleManager)
    {
        _accountService = accountService;
        _unitOfWork = unitOfWork;
        _roleManager = roleManager;
    }

    //-------------------------------------------------------------------------------------------- Role Api 

    //[HttpGet("TestSms")]
    //public async Task<ActionResult<BaseResponse>> GetRoles([FromHeader] string lang, string phone)
    //{
    //    var smsResult = await SmsService.SendMessage(phone, "5555");

    //    _baseResponse.ErrorMessage = smsResult;
    //    _baseResponse.ErrorCode = (int)Errors.Success;
    //    _baseResponse.Data = new { message = smsResult };
    //    return Ok(_baseResponse);
    //}

    //----------------------------------------------------------------------------------------------
    [HttpPost("AddRole")]
    public async Task<ActionResult<BaseResponse>> AddRoles(RoleDto roleDto, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var oldRole = await _roleManager.FindByNameAsync(roleDto.RoleName);
        if (oldRole != null)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "هذا الصلاحية موجودة مسبقا" : "This role is already exist";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }

        var role = await _roleManager.CreateAsync(new ApplicationRole()
        {
            Name = roleDto.RoleName,
            NameAr = roleDto.RoleNameAr,
            Description = roleDto.Description,
            RoleNumber = roleDto.GroupNumber,
        });
        if (role.Succeeded)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "تم اضافة الصلاحية بنجاح" : "Role added successfully";
            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.Data = null;
            return Ok(_baseResponse);
        }
        else
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = role.Errors.ToString() };
            return Ok(_baseResponse);
        }
    }

    //-------------------------------------------------------------------------------------------- student Register 
    [HttpPost("FreeAgentRegister")]
    public async Task<ActionResult<BaseResponse>> FreeAgentRegister(RegisterFreeAgentVm model, [FromHeader] string lang) 
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var result =await _accountService.RegisterFreeAgentAsync(model);
        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;

        }
        else
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.Data = new {result.FullName , result.PhoneNumber, PhoneNumberConfirmed=result.PhoneVerify, result.UserImgUrl };
        }
        return Ok(_baseResponse);

    }

    [HttpPost("CenterRegister")]
    public async Task<ActionResult<BaseResponse>> CenterRegister(RegisterCenterVm model, [FromHeader] string lang) 
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var result =await _accountService.RegisterCenterAsync(model);
        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;

        }
        else
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.Data = new {result.FullName , result.PhoneNumber, PhoneNumberConfirmed=result.PhoneVerify, result.UserImgUrl };
        }
        return Ok(_baseResponse);

    }
    [HttpPost("UserRegister")]
    public async Task<ActionResult<BaseResponse>> UserRegister(RegisterUserMv model, [FromHeader] string lang) 
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var result =await _accountService.RegisterUserAsync(model);
        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;

        }
        else
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.Data = new {result.FullName , result.PhoneNumber, PhoneNumberConfirmed=result.PhoneVerify, result.UserImgUrl };
        }
        return Ok(_baseResponse);

    }
        

    //-------------------------------------------------------------------------------------------- parent Register 
    [HttpPut("UpdateFreeAgentProfile")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
    public async  Task<ActionResult<BaseResponse>>  UpdateProfile(UpdateFreeAgentMv updateUser, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return BadRequest(_baseResponse);
        }
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        var result = await _accountService.UpdateFreeAgentProfile(userId, updateUser);
        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.Data = updateUser;
            return BadRequest(_baseResponse);
        }
        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.Data = new {
            result.Email,
            result.FullName,
            result.PhoneNumber,
            Role= result.Roles,
            result.UserType,
            result.IsApproved,
            result.PhoneVerify,
            result.Lat,
            result.Lng,
            result.FreelanceFormUrl,
            result.Status,
        };
        return Ok(_baseResponse);

    }
    [HttpPut("UpdateCenterProfile")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
    public async  Task<ActionResult<BaseResponse>>  UpdateCenterProfile(UpdateCenterMv updateUser, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return BadRequest(_baseResponse);
        }
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        var result = await _accountService.UpdateCenterProfile(userId, updateUser);
        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.Data = updateUser;
            return BadRequest(_baseResponse);
        }
        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.Data = new {
            result.Email,
            result.FullName,
            result.PhoneNumber,
            Role= result.Roles,
            result.UserType,
            result.IsApproved,
            result.PhoneVerify,
            result.Lat,
            result.Lng,
            result.TaxNumber,
            result.Status,
        };
        return Ok(_baseResponse);

    }
    [HttpPut("UpdateUserProfile")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
    public async  Task<ActionResult<BaseResponse>>  UpdateUserProfile(UpdateUserMv updateUser, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return BadRequest(_baseResponse);
        }
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        var result = await _accountService.UpdateUserProfile(userId, updateUser);
        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.Data = updateUser;
            return BadRequest(_baseResponse);
        }
        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.Data = new {
            result.Email,
            result.FullName,
            result.PhoneNumber,
            Role= result.Roles,
            result.UserType,
            result.IsApproved,
            result.PhoneVerify,
            result.Lat,
            result.Lng,
            result.Status,
        };
        return Ok(_baseResponse);

    }

    [HttpPut("UpdateLocation")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] 
    public async  Task<ActionResult<BaseResponse>>  UpdateLocation(UpdateUserLocation updateUser, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return BadRequest(_baseResponse);
        }
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        var result = await _accountService.UpdateLocation(userId, updateUser);
        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.Data = updateUser;
            return BadRequest(_baseResponse);
        }
        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.Data = new {
            result.Email,
            result.FullName,
            result.PhoneNumber,
        };
        return Ok(_baseResponse);

    }

    //-------------------------------------------------------------------------------------------- Comfirm Account 
    [HttpPost("confirmRegister")]
    public async Task<ActionResult<BaseResponse>> ConfirmSmsAsync([FromBody] ConfirmSms confirmSms, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var result = await _accountService.ConfirmSmsAsync(confirmSms);

        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.Data = confirmSms;
            return Ok(_baseResponse);
        }
        var user = await _unitOfWork.Users.FindAsync(s => s.Id == result.UserId);
        user.PhoneNumberConfirmed = true;
        user.EmailConfirmed = true;
        await _unitOfWork.SaveChangesAsync();
                

        _baseResponse.Data = new {
            result.Email,
            result.FullName,  Role = result.Roles,
            result.UserImgUrl,
            result.PhoneNumber };
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.ErrorCode = 0;


        return Ok(_baseResponse);
    }

    //-------------------------------------------------------------------------------------------- Resend SMS Api 
    [HttpPost("ReSendSMS")]
    public async Task<ActionResult<BaseResponse>> ReSendSms([FromHeader] string lang, ForgotPasswordMv resendSms)
    {

        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var result = await _accountService.ReSendSms(resendSms.PhoneNumber);
        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;

        }
        else
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = (int)Errors.Success;
            _baseResponse.Data = new {result.FullName, result.PhoneNumber, PhoneNumberConfirmed = result.PhoneVerify };
        }
        return Ok(_baseResponse);

    }

    //-------------------------------------------------------------------------------------------- login Api 
    [HttpPost("login")]
    public async Task<ActionResult<BaseResponse>> LoginAsync([FromBody] LoginModel model, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var result = await _accountService.LoginAsync(model);

        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.Data = model;
            return Ok(_baseResponse);
        }
        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم تسجيل الدخول" : "Login Successfully";
        _baseResponse.Data = new {
            result.UserId,
            result.Email,
            result.FullName,
            result.Token,
            Role = result.Roles,
            result.UserImgUrl,
            result.PhoneNumber ,
            result.Description,
            result.UserType
        };
        return Ok(_baseResponse);
    }

    //-------------------------------------------------------------------------------------------- logout Api 
    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<BaseResponse>> LogoutAsync([FromHeader] string lang)
    {
        //var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        var userName = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userName
        if (!string.IsNullOrEmpty(userName))
        {
            var result = await _accountService.Logout(userName);
            if (result)
            {
                _baseResponse.ErrorCode = 0;
                _baseResponse.ErrorMessage = (lang == "ar") ? "تم تسجيل الخروج بنجاح " : "Signed out successfully";
                return Ok(_baseResponse);
            }
        }
        _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
        _baseResponse.ErrorMessage = (lang == "ar") ? "هذا الحساب غير موجود " : "The User Not Exist";
        return Ok(_baseResponse);
    }

    //------------------------------------------------------------------------------------------------ Change Password Api
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("changeoldPassword")]
    public async Task<ActionResult<BaseResponse>> ChangeOldPasswordAsync([FromBody] ChangeOldPassword changePassword, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId

        var result = await _accountService.ChangeOldPasswordAsync(userId, changePassword);

        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.Data = null;
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.Data = new {
            result.Email,
            result.FullName,
            result.Token, Role = result.Roles,
            result.UserImgUrl,
            result.PhoneNumber };
        return Ok(_baseResponse);
    }

    //------------------------------------------------------------------------------------------------ forgot Password Api
    [HttpPost("ForgetPassword")]
    public async Task<ActionResult<BaseResponse>> ForgotPasswordAsync([FromBody] ForgotPasswordMv model, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var result = await _accountService.ForgetPassword(model);
        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.Data =model;
            return Ok(_baseResponse);
        }
        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.Data = null;
        return Ok(_baseResponse);

    }

    [HttpPost("ChangePasswordConfirm")]
    public async Task<ActionResult<BaseResponse>> ConfirmToChangePassAsync([FromBody] ConfirmSms confirmSms, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var result = await _accountService.ConfirmSmsAsync(confirmSms);

        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.Data = confirmSms;
            return Ok(_baseResponse);
        }

        _baseResponse.Data = new {
            result.Email,
            result.FullName,
            result.Token, Role = result.Roles,
            result.UserImgUrl,
            result.PhoneNumber };
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.ErrorCode = 0;

        return Ok(_baseResponse);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("changeForgotPassword")]
    public async Task<ActionResult<BaseResponse>> ChangePasswordAsync([FromBody] ChangePasswordMv password, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return Ok(_baseResponse);
        }
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        //var userName = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userName
        var result = await _accountService.ChangePasswordAsync(userId, password.Password);

        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.Data = new {
            result.Email,
            result.FullName,  Role = result.Roles,
            result.UserImgUrl,
            result.PhoneNumber };//Token = result.Token,
        return Ok(_baseResponse);
    }

    //----------------------------------------------------------------------------------------------------- get profile
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("GetUserInfo")]
    public async Task<ActionResult<BaseResponse>> GetUserInfo([FromHeader] string lang)
    {
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        if (string.IsNullOrEmpty(userId))
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar") ? "المستخدم غير موجود" : "User not exist";
            _baseResponse.Data = null;
            return Ok(_baseResponse);
        }
        var result = await _accountService.GetUserInfo(userId);

        if (!result.IsAuthenticated)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
            _baseResponse.ErrorCode = result.ErrorCode;
            _baseResponse.Data = result;
            return Ok(_baseResponse);
        }

           


        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? result.ArMessage : result.Message;
        _baseResponse.Data = new {
            result.Email,
            result.FullName,
            result.PhoneNumber,
            Role= result.Roles,
            result.UserType,
            result.IsApproved,
            result.PhoneVerify,
            result.Lat,
            result.Lng,
            result.TaxNumber,
            result.FreelanceFormUrl,
            result.Status,
            result.UserImgUrl,
            result.Description,
            result.CityId
        };
        return Ok(_baseResponse);
    }



    //----------------------------------------------------------------------------------------------------- update user profile

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("DeleteAccount")]
    public async Task<ActionResult<BaseResponse>> DeleteAccount([FromHeader] string lang)
    {
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        var userName = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userName
        if (!string.IsNullOrEmpty(userId))
        {
            await _accountService.Suspend(userId);
           
                _baseResponse.ErrorCode = 0;
                _baseResponse.ErrorMessage = (lang == "ar") ? "تم ايقاف حسابك و جاري مراجعة طلب الحذف  " : "Account Is Handed and Deleting Account is Under Processing";
                return Ok(_baseResponse);
           
        }
        _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
        _baseResponse.ErrorMessage = (lang == "ar") ? "هذا الحساب غير موجود " : "The User Not Exist";
        return Ok(_baseResponse);
    }



}