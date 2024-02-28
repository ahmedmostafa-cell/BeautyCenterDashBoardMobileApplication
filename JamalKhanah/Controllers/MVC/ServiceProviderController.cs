using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.BusinessLayer.Services;
using JamalKhanah.Core.Helpers;
using JamalKhanah.Core.ModelView.AuthViewModel.UpdateData;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JamalKhanah.Controllers.MVC;

[Authorize(Roles = "Admin")]
public class ServiceProviderController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IUnitOfWork _unitOfWork;
    //private readonly SMSService SmsService;

    public ServiceProviderController(IUnitOfWork unitOfWork, IAccountService accountService)
    {
        _accountService = accountService;
        _unitOfWork = unitOfWork;
        //SmsService = smsService;
    }

    // GET: ServiceProvider
    public async Task<ActionResult> Index()
    {
        var allAdmins = await _unitOfWork.Users
            .FindAllAsync(s => s.UserType == UserType.Center || s.UserType == UserType.FreeAgent );

        return View(allAdmins);
    }
    public async Task<ActionResult> FreeAgents()
    {
        var allAdmins = await _unitOfWork.Users.FindAllAsync(s => s.UserType == UserType.FreeAgent );

        return View("Index",allAdmins);
    }
    public async Task<ActionResult> Centers()
    {
        var allAdmins = await _unitOfWork.Users.FindAllAsync(s => s.UserType == UserType.Center );

        return View("Index",allAdmins);
    }

    public async Task<ActionResult> WaitForApprove()
    {
        var allAdmins = await _unitOfWork.Users.FindAllAsync(s => (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent) && s.IsApproved == false );

        return View("Index",allAdmins);
    }

  
    //--------------------------------------------------------------------------------------
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var admin = await _unitOfWork.Users.FindAsync(
            s => (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent) && s.Id == id,
            include: s => s.Include(user => user.City));
        if (admin == null)
        {
            return NotFound();
        }
        return View(admin);
    }

    
    //-----------------------------------------------------------------------------------------
    // GET: ServiceProvider/EditCenter/5
    public async Task<ActionResult> EditCenter(string id)
    {
        var user = await _accountService.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }

        var userModel = new UpdateCenterModel()
        {
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Description = user.Description,
            TaxNumber = user.TaxNumber,
            UserId = user.Id,
            CityId = user.CityId ?? 0 ,
        };

        ViewData["Cities"] = new SelectList(await _unitOfWork.Cities.FindAllAsync(s=>s.IsDeleted==false && s.IsShow==true), "Id", "NameAr");
        return View(userModel);
    }

    // POST: ServiceProvider/EditCenter/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> EditCenter(UpdateCenterModel model)
    {
        var admin = await _unitOfWork.Users.FindAsync(s => s.UserType== UserType.Center && s.Id == model.UserId, isNoTracking: true);
        if (admin == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewData["Cities"] = new SelectList(await _unitOfWork.Cities.FindAllAsync(s=>s.IsDeleted==false && s.IsShow==true), "Id", "NameAr");
            return View(model);
        }
        

       
        var result = await _accountService.UpdateCenterProfileAdmin(model.UserId,model);
            
        if (!result.IsAuthenticated)
        {
            ModelState.AddModelError("", result.ArMessage);
            ViewData["Cities"] = new SelectList(await _unitOfWork.Cities.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "NameAr");
            return View(model);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }

    }

    //-----------------------------------------------------------------------------------------
    // GET: ServiceProvider/EditFreeAgent/5
    public async Task<ActionResult> EditFreeAgent(string id)
    {
        var user = await _accountService.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }
        ViewData["Cities"] = new SelectList(await _unitOfWork.Cities.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "NameAr");

        var userModel = new UpdateFreeAgentModel()
        {
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Description = user.Description,
            UserId = user.Id,
            CityId = user.CityId ?? 0,
        };
        
    
        return View(userModel);
    }

    // POST: ServiceProvider/EditFreeAgent/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> EditFreeAgent(UpdateFreeAgentModel model)
    {
        var freeAgent = await _unitOfWork.Users.FindAsync(s => s.UserType == UserType.FreeAgent && s.Id == model.UserId, isNoTracking: true);
        if (freeAgent == null)
        {
            return NotFound();
        }
        if (!ModelState.IsValid)
        {
            ViewData["Cities"] = new SelectList(await _unitOfWork.Cities.FindAllAsync(s=>s.IsDeleted==false && s.IsShow==true), "Id", "NameAr");
            return View(model);
        }
        var result = await _accountService.UpdateFreeAgentProfileAdmin(model.UserId,model);
            
        if (!result.IsAuthenticated)
        {
            ModelState.AddModelError("", result.ArMessage);
            ViewData["Cities"] = new SelectList(await _unitOfWork.Cities.FindAllAsync(s=>s.IsDeleted==false && s.IsShow==true), "Id", "NameAr");
            return View(model);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }

    }


        //-----------------------------------------------------------------------------------------
        public async Task<ActionResult> Activate(string id)
    {
        await _accountService.Activate(id);
        TempData["Success"] = "تم تفعيل الحساب بنجاح";
        return RedirectToAction("Index");
    }

    public async Task<ActionResult> Suspend(string id)
    {
        await _accountService.Suspend(id);
        TempData["Success"] = "تم إيقاف الحساب بنجاح";
        return RedirectToAction("Index");
    }
    //-----------------------------------------------------------------------------------------
    public async Task<ActionResult> Approve(string id)
    {
        await _accountService.Approve(id);
        TempData["Success"] = "تم الموافقة على الحساب بنجاح";
        var result = await _unitOfWork.Users.FindAsync(s => s.Id == id);
        if (result == null)
        {
            TempData["Error"] = "المستخدم غير موجود";
            
        }
        var smsResult = await SmsService.SendMessage(result.PhoneNumber, "تم الموافقة على الحساب بنجاح");
        if (smsResult != null)
        {
            TempData["Success"] = "تم الموافقة على الحساب بنجاح";
            return RedirectToAction("Index");
        }
        else 
        {
            TempData["Error"] = "حدث خطأ في ارسال الرسالة";
            return RedirectToAction("Index");
        }
            
        
    }
    public async Task<ActionResult> Reject(string id)
    {
        var result = await _accountService.Reject(id);
        if (result)
        {
            TempData["Success"] = "تم رفض الحساب بنجاح";
        }
        else
        {
            TempData["Error"] = "حدث خطأ أثناء رفض او مسح الحساب لوجود بيانات متعلقة بالحساب";
        }
        return RedirectToAction("Index");
    }
    //-----------------------------------------------------------------------------------------
    public async Task<ActionResult> ShowServices(string id)
    {
        await _accountService.ShowServices(id);
        TempData["Success"] = "تم إظهار الخدمات بنجاح";
        return RedirectToAction("Index");
    }

    public async Task<ActionResult> HideServices(string id)
    {
        await _accountService.HideServices(id);
        TempData["Success"] = "تم إخفاء الخدمات بنجاح";
        return RedirectToAction("Index");
    }
    //----------------------------------------------------------------------------------------- Featured
    public async Task<ActionResult> MakeFeatured(string id)
    {
        await _accountService.MakeFeatured(id);
        TempData["Success"] = "تم تعيين الحساب كمميز بنجاح";
        return RedirectToAction("Index");
    }

    public async Task<ActionResult> RemoveFeatured(string id)
    {
        await _accountService.RemoveFeatured(id);
        TempData["Success"] = "تم إزالة الحساب من المميزين بنجاح";
        return RedirectToAction("Index");
    }


}