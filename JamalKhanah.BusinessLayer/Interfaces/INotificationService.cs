
using JamalKhanah.Core.DTO.NotificationModel;

namespace JamalKhanah.BusinessLayer.Interfaces;

public interface INotificationService
{
    Task<ResponseModel> SendNotification(NotificationModel notificationModel);
}