using JamalKhanah.Core.DTO.EntityDto;
using JamalKhanah.Core.DTO.NotificationModel;
using Moyasar.Services;

namespace JamalKhanah.BusinessLayer.Interfaces;

public interface IPaymentService
{
    Task<ResponseModel> SavePayment(SavePaymentDto savePaymentDto);
    ResponseModel FetchPayment(string id, string status, string message, out Payment payment, out int transactionNumber, out string paymentUrlIdentifier);
    bool RefundPayment(int id);
}