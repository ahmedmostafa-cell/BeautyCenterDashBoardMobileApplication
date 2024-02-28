using CorePush.Google;
using JamalKhanah.BusinessLayer.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using JamalKhanah.Core.DTO.NotificationModel;
using JamalKhanah.Core.Helpers;

namespace JamalKhanah.BusinessLayer.Services;

public class NotificationService : INotificationService
{
    private readonly FcmNotificationSetting _fcmNotificationSetting;

    public NotificationService(IOptions<FcmNotificationSetting> settings)
    {
        _fcmNotificationSetting = settings.Value;
    }

    public async Task<ResponseModel> SendNotification(NotificationModel notificationModel)
    {
        ResponseModel response = new ResponseModel();
        try
        {
            FcmSettings settings = new FcmSettings()
            {
                SenderId = _fcmNotificationSetting.SenderId,
                ServerKey = _fcmNotificationSetting.ServerKey
            };

            HttpClient httpClient = new HttpClient();
            string authorizationKey = string.Format("keyy={0}", settings.ServerKey);
            string deviceToken = notificationModel.DeviceId;
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
            httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            GoogleNotification.DataPayload dataPayload = new GoogleNotification.DataPayload();
            dataPayload.Title = notificationModel.Title;
            dataPayload.Body = notificationModel.Body;
            GoogleNotification notification = new GoogleNotification();
            notification.Data = dataPayload;
            //notification.Notification = dataPayload;

            var fcm = new FcmSender(settings, httpClient);
            var fcmSendResponse = await fcm.SendAsync(deviceToken, notification);

            if (fcmSendResponse.IsSuccess())
            {
                response.IsSuccess = true;
                response.Message = "Notification sent successfully";
                return response;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = fcmSendResponse.Results[0].Error;
                return response;
            }
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = $"Something went wrong : {ex.Message}";
            return response;
        }
    }
}