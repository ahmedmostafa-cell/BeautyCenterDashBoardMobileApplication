using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.Helpers;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JamalKhanah.Core.ModelView.AuthViewModel.RegisterData;
using JamalKhanah.Core.ModelView.AuthViewModel.ChangePasswordData;
using JamalKhanah.Core.ModelView.AuthViewModel.UpdateData;

namespace JamalKhanah.Controllers.MVC;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IUnitOfWork _unitOfWork;


    public AdminController(IUnitOfWork unitOfWork, IAccountService accountService)
    {
        _accountService = accountService;
        _unitOfWork = unitOfWork;
    }

    // GET: AdminController
    public async Task<ActionResult> Index()
    {
        var allAdmins = await _unitOfWork.Users.FindAllAsync(s => s.IsAdmin == true && s.UserType == UserType.Admin);

        return View(allAdmins);
    }
    //------------------------------------------------------------------------------------------------------- UsersList
    // GET: AdminController
    public async Task<ActionResult> IndexUsers()
    {
        var allUsers = await _unitOfWork.Users.FindAllAsync(s => s.IsAdmin == false && s.UserType == UserType.User && s.Status == true);

        return View(allUsers);
    }
    public async Task<ActionResult> IndexUsersDeleted()
    {
        var allUsers = await _unitOfWork.Users.FindAllAsync(s => s.IsAdmin == false && s.UserType == UserType.User && s.Status == false);

        return View(allUsers);
    }

    public async Task<ActionResult> IndexServiceProviderDeleted()
    {
        var allUsers = await _unitOfWork.Users.FindAllAsync(s => s.IsAdmin == false && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent) && s.Status == false);

        return View(allUsers);
    }


    
    //------------------------------------------------------------------------------------------------------- Create
    // GET: AdminController/Create
    public IActionResult Create()
    {

        return View(new RegisterAdminMv());
    }

    // POST: AdminController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(RegisterAdminMv adminModel)
    {
        if (!ModelState.IsValid) return View(adminModel);
        var result = await _accountService.RegisterAdminAsync(adminModel);
        if (!result.IsAuthenticated)
        {
            ModelState.AddModelError("", result.ArMessage);
            return View(adminModel);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }

    //-------------------------------------------------------------------------------------------------------  Edit
    // GET: AdminController/Edit/userId
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var admin = await _unitOfWork.Users.FindAsync(s => s.IsAdmin &&  s.Id == id);
        if (admin == null)
        {
            return NotFound();
        }
        UpdateAdminMv updateAdmin = new ()
        {
            UserId = admin.Id,
            FullName = admin.FullName,
            Email = admin.Email,
        };

        return View(updateAdmin);
    }

    // POST: AdminController/Edit/userId
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UpdateAdminMv adminModel)
    {
        var admin = await _unitOfWork.Users.FindAsync(s => s.IsAdmin && s.Id == adminModel.UserId, isNoTracking: true);
        if (admin == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid) return View(adminModel);
        var result = await _accountService.UpdateAdminProfile(adminModel.UserId,adminModel);
            
        if (!result.IsAuthenticated)
        {
            ModelState.AddModelError("", result.ArMessage);
            return View(adminModel);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }
    }



    //-------------------------------------------------------------------------------------------------------  EditUser
    // GET: AdminController/Edit/userId
    public async Task<IActionResult> EditUser(string id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var user = await _unitOfWork.Users.FindAsync(s => s.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        UpdateAdminMv updateAdmin = new()
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
           
        };

        return View(updateAdmin);
    }

    // POST: AdminController/Edit/userId
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(string id, UpdateAdminMv adminModel)
    {
        var admin = await _unitOfWork.Users.FindAsync(s =>  s.Id == adminModel.UserId, isNoTracking: true);
        if (admin == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid) return View(adminModel);
        var result = await _accountService.UpdateAdminProfile(adminModel.UserId, adminModel);

        if (!result.IsAuthenticated)
        {
            ModelState.AddModelError("", result.ArMessage);
            return View(adminModel);
        }
        else
        {
            return RedirectToAction(nameof(IndexUsers));
        }
    }

    //------------------------------------------------------------------------------------------------------- Details

    // GET: AdminController/Details/userId
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var admin = await _unitOfWork.Users.FindAsync(s => s.IsAdmin && s.Id == id);
        if (admin == null)
        {
            return NotFound();
        }
        return View(admin);
    }


    //------------------------------------------------------------------------------------------------------- DetailsUsers

    // GET: AdminController/Details/userId
    public async Task<IActionResult> DetailsUsers(string id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var user = await _unitOfWork.Users.FindAsync(s =>  s.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    //-------------------------------------------------------------------------------------------------------
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



    //-------------------------------------------------------------------------------------------------------
    public async Task<ActionResult> ActivateUser(string id)
    {
        await _accountService.Activate(id);
        TempData["Success"] = "تم تفعيل الحساب بنجاح";
        return RedirectToAction("IndexUsers");
    }

    public async Task<ActionResult> SuspendUser(string id)
    {
        await _accountService.Suspend(id);
        TempData["Success"] = "تم إيقاف الحساب بنجاح";
        return RedirectToAction("IndexUsers");
    }

    //-------------------------------------------------------------------------------------------------------
    public async Task<IActionResult> ChangePassword(string id)
    {
        ChangePasswordAdminMv changePasswordAdmin = new();
        var admin = await _unitOfWork.Users.FindAsync(s => s.IsAdmin && s.Id == id);
        if (admin == null)
        {
            return NotFound();
        }
       
        changePasswordAdmin.UserId = admin.Id;
        changePasswordAdmin.PhoneNumber = admin.PhoneNumber;
        changePasswordAdmin.FullName = admin.FullName;
  

        return View(changePasswordAdmin);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordAdminMv changePassword)
    {
        if (!ModelState.IsValid)
        {
            return View(changePassword);
        }
        var result = await _accountService.ChangePasswordAsync(changePassword.UserId, changePassword.Password);
        if (result.IsAuthenticated)
        {
            TempData["Success"] = "تم تغير كلمة السر بنجاح";
            return RedirectToAction("Index");
        }
        else
        {
            ModelState.AddModelError("", result.ArMessage);
            return View(changePassword);
        }
    }


    //-------------------------------------------------------------------------------------------------------
    public async Task<IActionResult> ChangePasswordUser(string id)
    {
        ChangePasswordAdminMv changePasswordAdmin = new();
        var user = await _unitOfWork.Users.FindAsync(s =>  s.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        changePasswordAdmin.UserId = user.Id;
        changePasswordAdmin.PhoneNumber = user.PhoneNumber;
        changePasswordAdmin.FullName = user.FullName;


        return View(changePasswordAdmin);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePasswordUser(ChangePasswordAdminMv changePassword)
    {
        if (!ModelState.IsValid)
        {
            return View(changePassword);
        }
        var result = await _accountService.ChangePasswordAsync(changePassword.UserId, changePassword.Password);
        if (result.IsAuthenticated)
        {
            TempData["Success"] = "تم تغير كلمة السر بنجاح";
            return RedirectToAction("IndexUsers");
        }
        else
        {
            ModelState.AddModelError("", result.ArMessage);
            return View(changePassword);
        }
    }

    //------------------------------------------------------------------------------------------------------------

    public async Task<ActionResult> Reject(string id)
    {
        var result = await _accountService.Reject(id);
        if (result)
        {
            TempData["Success"] = "تم مسح الحساب بنجاح";
        }
        else
        {
            TempData["Error"] = "حدث خطأ أثناء   مسح الحساب لوجود بيانات متعلقة بالحساب";
        }
        return RedirectToAction("IndexUsers");
    }
}
