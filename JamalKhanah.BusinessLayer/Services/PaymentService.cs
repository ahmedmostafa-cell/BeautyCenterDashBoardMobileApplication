using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.DTO.NotificationModel;
using JamalKhanah.Core.Helpers;
using Microsoft.Extensions.Options;
using Moyasar;
using Moyasar.Services;
using JamalKhanah.RepositoryLayer.Interfaces;
using JamalKhanah.Core.DTO.EntityDto;
using JamalKhanah.Core.DTO;
using JamalKhanah.Core.Entity.ChatAndNotification;

namespace JamalKhanah.BusinessLayer.Services;

public class PaymentService : IPaymentService
{
    private readonly PaymentSettings _paymentSettings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly NotificationModel _notificationModel;
    private readonly BaseResponse _baseResponse;
    public PaymentService(INotificationService notificationService,IOptions<PaymentSettings> paymentSettings, IUnitOfWork unitOfWork)
    {
        _paymentSettings = paymentSettings.Value;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _notificationModel = new NotificationModel();
        _baseResponse = new BaseResponse();

    }

    public  ResponseModel  FetchPayment(string id, string status, string message, out Payment payment, out int transactionNumber, out string paymentUrlIdentifier)
    {
        transactionNumber = 0;
        paymentUrlIdentifier = null;

        ResponseModel response = new ResponseModel();
        payment = null;
        try
        {
            var userPaymentObject = _unitOfWork.PaymentHistories.Find(p => p.PaymentId == id);
            if (userPaymentObject == null)
            {
                response.IsSuccess = false;
                return response;
            }

            transactionNumber = userPaymentObject.TransactionNumber;
            paymentUrlIdentifier = userPaymentObject.PaymentUrlIdentifier;

            MoyasarService.ApiKey = _paymentSettings.IsLive ? _paymentSettings.LiveSecretKey : _paymentSettings.TestSecretKey;
            payment = Payment.Fetch(id);
            if (payment == null)
            {
                response.IsSuccess = false;
                return response;
            }

            userPaymentObject.Ip = payment.Ip;
            userPaymentObject.Status = payment.Status;
            userPaymentObject.Amount = payment.Amount;
            userPaymentObject.Amount_format = payment.FormattedAmount;
            userPaymentObject.Callback_url = payment.CallbackUrl;
            userPaymentObject.Captured = payment.CapturedAmount;
            userPaymentObject.Captured_at = payment.CapturedAt;
            userPaymentObject.Captured_format = payment.FormattedCapturedAmount;
            userPaymentObject.Created_at = payment.CreatedAt;
            userPaymentObject.Currency = payment.Currency;
            userPaymentObject.Description = payment.Description;
            userPaymentObject.Fee = payment.Fee;
            userPaymentObject.Fee_format = payment.FormattedFee;
            userPaymentObject.Invoice_id = payment.InvoiceId;
            userPaymentObject.Refunded = payment.RefundedAmount;
            userPaymentObject.Refunded_at = payment.RefundedAt;
            userPaymentObject.Refunded_format = payment.FormattedRefundedAmount;
            userPaymentObject.Updated_at = payment.UpdatedAt;
            userPaymentObject.Voided_at = payment.VoidedAt;
            userPaymentObject.Source_Type = payment.Source?.Type;

            _unitOfWork.PaymentHistories.Update(userPaymentObject);
            _unitOfWork.SaveChanges();

            if (!string.IsNullOrWhiteSpace(payment.Status) && (payment.Status.ToLower() == "paid" || payment.Status.ToLower() == "succeeded"))
            {
                var order =  _unitOfWork.Orders.FindAsync(s => s.Id == userPaymentObject.OrderId);
                var service =  _unitOfWork.Services.FindAsync(s => s.Id == order.Result.ServiceId);
                var provider =  _unitOfWork.Users.FindAsync(s => s.Id == service.Result.ProviderId);
                _notificationModel.DeviceId = provider.Result.DeviceToken;
                _notificationModel.Title = "تم طلب خدمة";
                _notificationModel.Body = "تم طلب خدمة";
                var notificationResult =   _notificationService.SendNotification(_notificationModel).GetAwaiter();
                
                   
                   //var smsResult = SmsService.SendMessage(provider.Result.PhoneNumber, "تم طلب خدمة");
                    response.IsSuccess = true;

               
              
            }
            else
            {
                response.IsSuccess = false;
            }

            return response;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = $"Something went wrong : {ex.Message}";
            return response;
        }
    }

    public bool RefundPayment(int id)
    {
        var paymentHistory = _unitOfWork.PaymentHistories.Find(p => p.Id == id);
        if (paymentHistory == null)
        {
            return false;
        }

        MoyasarService.ApiKey = _paymentSettings.IsLive ? _paymentSettings.LiveSecretKey : _paymentSettings.TestSecretKey;
        var payment = Payment.Fetch(paymentHistory.PaymentId);
        if (payment == null)
        {
            return false;
        }

        try
        {
            var result = payment.Refund();

            paymentHistory.Refunded = result.RefundedAmount;
            paymentHistory.Refunded_at = result.RefundedAt;
            paymentHistory.Refunded_format = result.FormattedRefundedAmount;
            paymentHistory.Updated_at = result.UpdatedAt;
            paymentHistory.Status = result.Status;

            _unitOfWork.PaymentHistories.Update(paymentHistory);
            _unitOfWork.SaveChanges();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public async Task<ResponseModel> SavePayment(SavePaymentDto savePaymentDto)
    {
        ResponseModel response = new ResponseModel();

        try
        {
            MoyasarService.ApiKey = _paymentSettings.IsLive ? _paymentSettings.LiveSecretKey : _paymentSettings.TestSecretKey;
            var fetchedPayment = Payment.Fetch(savePaymentDto.Id);
            if (fetchedPayment != null && fetchedPayment.Metadata != null && fetchedPayment.Metadata.Any(k => k.Key == "PaymentUrlIdentifier"))
            {
                var metaDataValue = fetchedPayment.Metadata.FirstOrDefault(k => k.Key == "PaymentUrlIdentifier").Value;
                var payment = _unitOfWork.PaymentHistories.Find(p => p.PaymentUrlIdentifier == metaDataValue);
                if (payment == null)
                {
                    return null;
                }

                payment.TransactionNumber = GenerateRandomNumber(100000000, 999999999);
                payment.Ip = savePaymentDto.Ip;
                payment.Status = savePaymentDto.Status;
                payment.Amount = savePaymentDto.Amount;
                payment.Amount_format = savePaymentDto.Amount_format;
                payment.Callback_url = savePaymentDto.Callback_url;
                payment.Captured = savePaymentDto.Captured;
                payment.Captured_at = savePaymentDto.Captured_at;
                payment.Captured_format = savePaymentDto.Captured_format;
                payment.Created_at = savePaymentDto.Created_at;
                payment.Currency = savePaymentDto.Currency;
                payment.Description = savePaymentDto.Description;
                payment.Fee = savePaymentDto.Fee;
                payment.Fee_format = savePaymentDto.Fee_format;
                payment.Invoice_id = savePaymentDto.Invoice_id;
                payment.PaymentId = savePaymentDto.Id;
                payment.Refunded = savePaymentDto.Refunded;
                payment.Refunded_at = savePaymentDto.Refunded_at;
                payment.Refunded_format = savePaymentDto.Refunded_format;
                payment.Updated_at = savePaymentDto.Updated_at;
                payment.Voided_at = savePaymentDto.Voided_at;
                payment.Source_Message = savePaymentDto.Source?.Message;
                payment.Source_Number = savePaymentDto.Source?.Number;
                payment.Source_Reference_number = savePaymentDto.Source?.Reference_number;
                payment.Source_Company = savePaymentDto.Source?.Company;
                payment.Source_Gateway_id = savePaymentDto.Source?.Gateway_id;
                payment.Source_Name = savePaymentDto.Source?.Name;
                payment.Source_Token = savePaymentDto.Source?.Token;
                payment.Source_Transaction_url = savePaymentDto.Source?.Transaction_url;
                payment.Source_Type = savePaymentDto.Source?.Type;

                _unitOfWork.PaymentHistories.Update(payment);
                await _unitOfWork.SaveChangesAsync();
                response.IsSuccess = true;
                return response;
            }
            return null;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = $"Something went wrong : {ex.Message}";
            return response;
        }
    }

    private int GenerateRandomNumber(int min, int max)
    {
        Random random = new Random();
        return random.Next(min, max);
    }
}