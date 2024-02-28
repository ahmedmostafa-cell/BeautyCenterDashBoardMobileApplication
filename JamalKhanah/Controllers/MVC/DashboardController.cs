using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Helpers;
using JamalKhanah.Core.ModelView.MV;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace JamalKhanah.Controllers.MVC;

[Authorize(Roles = "Admin,Teacher")]
public class DashboardController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private ApplicationUser _user;

    public DashboardController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }
        
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = _userManager.GetUserId(User);
        _user = _unitOfWork.Users.Find(s => s.Id == userId);
    }

    public IActionResult CheckUser()
    {
        if (_user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        return _user.IsAdmin ? RedirectToAction("Index", "Dashboard") : RedirectToAction("Login", "Account");
    }

    public async Task<IActionResult> Index()
    {
        var total = await _unitOfWork.Orders.FindByQuery(s => s.IsDeleted == false && s.IsPaid == true).SumAsync(s => s.Total);
        var data = new DashboardCounts()
        {
            Cities =await _unitOfWork.Cities.CountAsync(s=>s.IsDeleted==false),
            AllProviders =await _unitOfWork.Users.CountAsync(s=>(s.UserType == UserType.Center || s.UserType == UserType.FreeAgent) && s.IsApproved == true ),
            AllProvidersWaitApproved =await _unitOfWork.Users.CountAsync(s=>(s.UserType == UserType.Center || s.UserType == UserType.FreeAgent) && s.IsApproved == false ),
            Centers =await _unitOfWork.Users.CountAsync(s=>s.UserType == UserType.Center && s.IsApproved == true ),
            FreeAgents =await _unitOfWork.Users.CountAsync(s=>s.UserType == UserType.FreeAgent && s.IsApproved == true ),
            AllSections =await _unitOfWork.MainSections.CountAsync(s=>s.IsDeleted==false),
            AllServices = await _unitOfWork.Services.CountAsync(s=>s.IsDeleted==false),
            AllOrders =await _unitOfWork.Orders.CountAsync(s=>s.IsDeleted==false),
            AllFinishOrders = await _unitOfWork.Orders.CountAsync(s=>s.IsDeleted==false && s.OrderStatus == OrderStatus.Finished),
            AllComplains =await _unitOfWork.Complaints.CountAsync(s=>s.IsDeleted==false),
            Profits =5.0 / 100.0 * total ,
            TotalAmount = total ,
            usersWantDelete = await _unitOfWork.Users.CountAsync(s => s.IsAdmin == false && s.UserType == UserType.User && s.Status == false),
            serviceProvidersWantDelete = await _unitOfWork.Users.CountAsync(s => s.IsAdmin == false && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent) && s.Status == false),
    };
        return View(data);
    }

  
}
