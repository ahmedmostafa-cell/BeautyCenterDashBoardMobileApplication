using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.DTO;
using JamalKhanah.Core.DTO.NotificationModel;
using JamalKhanah.Core.Entity.ChatAndNotification;
using JamalKhanah.Core.Helpers;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JamalKhanah.Controllers.API;


public class NotificationController : BaseApiController
{
    private readonly INotificationService _notificationService;
    private readonly BaseResponse _baseResponse;
    private readonly NotificationModel _notificationModel;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationController(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _baseResponse = new BaseResponse();
        _notificationModel = new NotificationModel();
    }


    [HttpPost("SendNotification")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> SendNotification(NotificationDto notificationDto, [FromHeader] string lang)
    {
        if (!ModelState.IsValid)
        {
            _baseResponse.ErrorMessage = (lang == "ar") ? "خطأ في البيانات" : "Error in data";
            _baseResponse.ErrorCode = (int)Errors.TheModelIsInvalid;
            _baseResponse.Data = new { message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)) };
            return BadRequest(_baseResponse);
        }
        var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
        var result = await _unitOfWork.Users.FindAsync(s => s.Id == userId);
        // var result = await _accountService.getUserInfo(userId);
        if (result == null)
        {
            _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
            _baseResponse.ErrorMessage = (lang == "ar") ? "المستخدم غير موجود " : "The User Not Exist Or Deleted";
            _baseResponse.Data = null;
            return BadRequest(_baseResponse);
        }

        var notificationsConfirmed = _unitOfWork.NotificationsConfirmed.FindByQuery(s => s.UserId == userId).Select(s => s.Notification).ToList();
        
        var notifications = (_unitOfWork.Notifications.FindByQuery(s => s.CreatedOn > result.RegistrationDate)).ToList();
        result.DeviceToken = notificationDto.Token;
        _unitOfWork.Users.Update(result);
        await _unitOfWork.SaveChangesAsync();

        if (notifications.Count > 0)
        {
            foreach (var notification in notifications.Where(notification => !notificationsConfirmed.Contains(notification)))
            {
                _notificationModel.DeviceId = notificationDto.Token;
                _notificationModel.Title = notification.Title;
                _notificationModel.Body = notification.Body;
                var notificationResult = await _notificationService.SendNotification(_notificationModel);
                if (notificationResult.IsSuccess)
                {
                    await _unitOfWork.NotificationsConfirmed.AddAsync(new NotificationConfirmed() { NotificationId = notification.Id, UserId = userId });
                    await _unitOfWork.SaveChangesAsync();

                }
                else
                {
                    _baseResponse.ErrorCode = (int)Errors.SomeThingWentWrong;
                    _baseResponse.ErrorMessage = (lang == "ar") ? notificationResult.Message + "test" : notificationResult.Message;
                    _baseResponse.Data = null;
                    return BadRequest(_baseResponse);

                }
            }
        }

        _baseResponse.ErrorCode = (int)Errors.Success;
        _baseResponse.ErrorMessage = (lang == "ar") ? "تم الإرسال" : "Sent";
        _baseResponse.Data = null;
        return Ok(_baseResponse);
    }

    [HttpGet("GetAllNotifications")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetAllNotifications([FromHeader] string lang)
    {
	    var userId = this.User.Claims.First(i => i.Type == "uid").Value; // will give the user's userId
	    var result = await _unitOfWork.Users.FindAsync(s => s.Id == userId);
	    if (result == null)
	    {
		    _baseResponse.ErrorCode = (int)Errors.TheUserNotExistOrDeleted;
		    _baseResponse.ErrorMessage = (lang == "ar") ? "المستخدم غير موجود " : "The User Not Exist Or Deleted";
		    _baseResponse.Data = null;
		    return BadRequest(_baseResponse);
	    }
        string userIdd = this.User.Claims.First(i => i.Type == "uid").Value;
        var user = await _unitOfWork.Users.FindAsync(s=> s.Id == userIdd);
        var notificationsConfirmed = _unitOfWork.NotificationsConfirmed.FindByQuery(s => s.UserId == userId)
		    .Select(s => s.Notification).OrderByDescending(s => s.CreatedOn).ToList();
	    var notifications = ( _unitOfWork.Notifications.FindByQuery(s => s.CreatedOn > user.RegistrationDate)).ToList();

	    if (notifications.Count > 0)
	    {
		    foreach (var notification in notifications.Where(notification => !notificationsConfirmed.Contains(notification)))
		    {
			    await _unitOfWork.NotificationsConfirmed.AddAsync(new NotificationConfirmed() { NotificationId = notification.Id, UserId = userId });
			    await _unitOfWork.SaveChangesAsync();
		    }
	    }

	    _baseResponse.ErrorCode = (int)Errors.Success;
	    _baseResponse.ErrorMessage = (lang == "ar") ? "تم الإرسال" : "Sent";
	    _baseResponse.Data = notificationsConfirmed;
	    return Ok(_baseResponse);
    }

}