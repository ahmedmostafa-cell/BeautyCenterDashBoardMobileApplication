using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.DTO.NotificationModel;
using JamalKhanah.Core.Entity.ChatAndNotification;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JamalKhanah.Controllers.MVC;

//    [Authorize(Roles = "Admin")]
public class NotificationsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly NotificationModel _notificationModel;

    public NotificationsController(INotificationService notificationService, IUnitOfWork unitOfWork)
    {
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _notificationModel = new NotificationModel();
    }
   
    public async Task<IActionResult> Index()
    {
        // await SeedQuran.Seed(_unitOfWork);
        return View((await _unitOfWork.Notifications.GetAllAsync()).ToList());
    }

   

    // GET: Notifications/Create
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create( Notification notification)
    {
        if (!ModelState.IsValid) return View(notification);
          
        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();


        var notificationsConfirmed = (await _unitOfWork.NotificationsConfirmed.GetAllAsync()).ToList();
          
        var allUser = await _unitOfWork.Users.FindByQuery(s =>  s.DeviceToken != null).ToListAsync();
           

        if (!allUser.Any()) return RedirectToAction(nameof(Index));
        {
            foreach (var user in from user in allUser let notificationConfirmed = notificationsConfirmed.FirstOrDefault(s => 
                         s.UserId == user.Id && s.NotificationId == notification.Id) where notificationConfirmed == null select user)
            {
                _notificationModel.DeviceId = user.DeviceToken;
                _notificationModel.Title = notification.Title;
                _notificationModel.Body = notification.Body;
                var notificationResult = await _notificationService.SendNotification(_notificationModel);
                if (!notificationResult.IsSuccess) continue;
                await _unitOfWork.NotificationsConfirmed.AddAsync(new NotificationConfirmed() { NotificationId = notification.Id, UserId = user.Id });
                await _unitOfWork.SaveChangesAsync();
            }
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id )
    {
        var notification = await _unitOfWork.Notifications.FindAsync(s=>s.Id==id);
        if (notification == null)
        {
            return RedirectToAction(nameof(Index));
        }
        _unitOfWork.Notifications.Delete(notification);
        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }



}
