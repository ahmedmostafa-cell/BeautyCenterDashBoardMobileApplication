using Microsoft.AspNetCore.Mvc;
using JamalKhanah.RepositoryLayer.Interfaces;
using JamalKhanah.Core.DTO.EntityDto;
using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.Helpers;
using Microsoft.Extensions.Options;
using JamalKhanah.Core.Entity.PaymentData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace JamalKhanah.Controllers.MVC;

public class PaymentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;
    private readonly PaymentSettings _paymentSettings;

    public PaymentController(IUnitOfWork unitOfWork, IPaymentService paymentService, IOptions<PaymentSettings> paymentSettings)
    {
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _paymentSettings = paymentSettings.Value;
    }

    [HttpGet]
    public IActionResult InitializePayment(string id)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            var order = _unitOfWork.Orders.FindByQuery(o => o.PaymentUrlIdentifier == id && !o.IsPaid).FirstOrDefault();
            if (order != null)
            {
                _unitOfWork.PaymentHistories.Add(new PaymentHistory()
                {
                    OrderId = order.Id,
                    PaymentUrlIdentifier = id
                });

                _unitOfWork.SaveChanges();

                InitalPaymentObjectDto paymentObjectDto = new InitalPaymentObjectDto()
                {
                    Amount = decimal.Parse((order.Total + order.DeliveryFees).ToString("N2")),
                    APIKey = _paymentSettings.IsLive ? _paymentSettings.LivePublishableKey : _paymentSettings.TestPublishableKey,
                    Currency = _paymentSettings.Currency,
                    OrderDescription = $"عملية دفع طلب رقم {order.OrderNumber}",
                    PaymentUrlIdentifier = id
                };

                return View(paymentObjectDto);
            }
        }

        return RedirectToAction("FetchPayment");
    }

    [HttpPost]
    public async Task<IActionResult> SavePayment([FromBody] SavePaymentDto savePaymentDto)
    {
        var response = await _paymentService.SavePayment(savePaymentDto);
        if (response == null)
        {
            return BadRequest();
        }

        return Created("", null);
    }

    [HttpGet]
    public IActionResult FetchPayment(string id, string status, string message)
    {
        var fetchingResponse = _paymentService.FetchPayment(id, status, message, out var payment, out int transactionNumber, out string paymentUrlIdentifier);
        if (fetchingResponse.IsSuccess)
        {
            var order = _unitOfWork.Orders.FindByQuery(o => o.PaymentUrlIdentifier == paymentUrlIdentifier).FirstOrDefault();
            if (order != null)
            {
                order.OrderStatus = OrderStatus.Preparing;
                order.IsPaid = true;
                order.UpdatedAt = DateTime.Now;
                _unitOfWork.Orders.Update(order);
                _unitOfWork.SaveChanges();
            }
        }

        ViewBag.IsSuccess = fetchingResponse.IsSuccess;
        ViewBag.TransactionNumber = transactionNumber;
        return View(payment);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var applicationContext =
            await _unitOfWork.PaymentHistories.FindAllAsync(criteria:s=>s.Order.IsPaid==true && (s.Status== "Paid" || s.Status== "succeeded"),include:i => i.Include(w => w.Order).ThenInclude(q => q.User));
        return View(applicationContext.OrderByDescending(d => d.Created_at));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var payment =
                await _unitOfWork.PaymentHistories.FindAsync(p => p.Id == id, s => s.Include(e => e.Order).ThenInclude(e => e.User).Include(s => s.Order).ThenInclude(s => s.Service).ThenInclude(s => s.Provider));

        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult RefundPayment(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        TempData["IsRefunded"] = _paymentService.RefundPayment(id.Value);
        return RedirectToAction("Details", new { id = id.Value });
    }
}
