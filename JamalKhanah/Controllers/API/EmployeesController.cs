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
public class EmployeesController : BaseApiController, IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileHandling _fileHandling;
    private readonly BaseResponse _baseResponse;
    private  ApplicationUser _user;
    public EmployeesController(IUnitOfWork unitOfWork, IFileHandling fileHandling)
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
    // GET: api/Employees
    [HttpGet]
    public async Task<ActionResult<BaseResponse>> Employees([FromHeader] string lang)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        var employees = await _unitOfWork.Employees.FindByQuery(
            criteria: s =>  s.User.UserType == UserType.Center && s.UserId == _user.Id  && s.IsDeleted == false)
            .Select(s=>new
            {
                s.Id,
                s.FullName,
                s.Email,
                s.PhoneNumber,
                s.ImgUrl
            }).ToListAsync();

        if (!employees.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.EmployeeNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد موظفين للمستخدم " : "Employee Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = employees;
        return Ok(_baseResponse);
    }

    // GET: api/Employees/5
    [HttpGet("{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> Employee([FromHeader] string lang, int id)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        var employee = await _unitOfWork.Employees.FindByQuery(
            criteria: s => s.Id == id && s.User.UserType == UserType.Center && s.UserId == _user.Id && s.IsDeleted == false)
            .Select(s=>new
            {
                s.Id,
                s.FullName,
                s.Email,
                s.PhoneNumber,
                s.ImgUrl
            }).FirstOrDefaultAsync();

        if (employee == null)
        {
            _baseResponse.ErrorCode = (int)Errors.EmployeeNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد موظفين للمستخدم " : "Employee Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = employee;
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    // PUT: api/Employees/5
    [HttpPut()]
    public async Task<ActionResult<BaseResponse>> UpdateEmployee([FromHeader] string lang, UpdateEmployeeDto employee)
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
        var employeeToUpdate = await _unitOfWork.Employees.FindByQuery(
                criteria: s => s.Id == employee.Id && s.User.UserType == UserType.Center && s.UserId == _user.Id && s.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (employeeToUpdate == null)
        {
            _baseResponse.ErrorCode = (int)Errors.EmployeeNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد موظفين للمستخدم " : "Employee Not Found";
            return Ok(_baseResponse);
        }

        if (!string.IsNullOrEmpty(employee.ImgUrl))
        {
            try
            {
                var img =await _fileHandling.UploadPhotoBase64(employee.ImgUrl, "Employees",employeeToUpdate.ImgUrl);
                employeeToUpdate.ImgUrl = img;
            }
            catch
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInUploadImage;
                _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في رفع الصورة" : "Error in upload image";
                return Ok(_baseResponse);
            }
        }
      
        employeeToUpdate.FullName = employee.FullName;
        employeeToUpdate.Email = employee.Email;
        employeeToUpdate.PhoneNumber = employee.PhoneNumber;
        employeeToUpdate.IsUpdated = true;
        employeeToUpdate.UpdatedAt = DateTime.Now; 
        _unitOfWork.Employees.Update(employeeToUpdate);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم تعديل الموظف بنجاح" : "Employee Updated Successfully";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    // POST: api/Employees
    [HttpPost]
    public async Task<ActionResult<BaseResponse>> AddEmployee([FromHeader] string lang, EmployeeDto employee)
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

        if (_user.UserType != UserType.Center)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا يملك الحساب صلاحية اضافة موظفين " : "The account does not have the authority to add employees ";
            return Ok(_baseResponse);
        }
        if (!string.IsNullOrEmpty(employee.ImgUrl))
        {
            try
            {
                var img =await _fileHandling.UploadPhotoBase64(employee.ImgUrl, "Employees");
                employee.ImgUrl = img;
            }
            catch
            {
                _baseResponse.ErrorCode = (int)Errors.ErrorInUploadImage;
                _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في رفع الصورة" : "Error in upload image";
                return Ok(_baseResponse);
            }
        }else
        {
            _baseResponse.ErrorCode = (int)Errors.ErrorInUploadImage;
            _baseResponse.ErrorMessage = (lang == "ar") ? "يجب ارفاق صورة" : "Image is required";
            return Ok(_baseResponse);

        }

        var newEmployee = new Employee
        {
            FullName = employee.FullName,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber,
            ImgUrl = employee.ImgUrl,
            UserId = _user.Id,
            IsDeleted = false,
            IsUpdated = false,
            CreatedAt = DateTime.Now
        };

  
        
        await _unitOfWork.Employees.AddAsync(newEmployee);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم اضافة الموظف بنجاح" : "Employee Added Successfully";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------

    // DELETE: api/Employees/5
    [HttpDelete("{id:int:required}")]
    public async Task<ActionResult<BaseResponse>> DeleteEmployee([FromHeader] string lang, int id)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        var employee = await _unitOfWork.Employees.FindByQuery(
                criteria: s => s.Id == id && s.User.UserType == UserType.Center && s.UserId == _user.Id && s.IsDeleted == false)
            .FirstOrDefaultAsync();

        if (employee == null)
        {
            _baseResponse.ErrorCode = (int)Errors.EmployeeNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد موظفين للمستخدم " : "Employee Not Found";
            return Ok(_baseResponse);
        }
        employee.IsDeleted = true;
        employee.DeletedAt = DateTime.Now;
        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();

        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم حذف الموظف بنجاح" : "Employee Deleted Successfully";
        return Ok(_baseResponse);
    }

    //---------------------------------------------------------------------------------------------------
    [HttpGet("EmployeesByProvider")]
    public async Task<ActionResult<BaseResponse>> EmployeesByProvider([FromHeader] string lang, string providerId)
    {
        if (_user == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar")
                ? "هذا الحساب غير موجود "
                : "The User Not Exist ";
            return Ok(_baseResponse);
        }
        var employees = await _unitOfWork.Employees.FindByQuery(
                criteria: s => s.User.UserType == UserType.Center && s.UserId == providerId && s.IsDeleted == false)
            .Select(s => new
            {
                s.Id,
                s.FullName,
                s.Email,
                s.PhoneNumber,
                s.ImgUrl
            }).ToListAsync();

        if (!employees.Any())
        {
            _baseResponse.ErrorCode = (int)Errors.EmployeeNotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد موظفين للمستخدم " : "Employee Not Found";
            return Ok(_baseResponse);
        }

        _baseResponse.ErrorCode = 0;
        _baseResponse.Data = employees;
        return Ok(_baseResponse);
    }
}