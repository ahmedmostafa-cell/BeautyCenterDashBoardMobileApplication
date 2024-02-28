using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.ModelView.AuthViewModel.ChangePasswordData;
using JamalKhanah.Core.ModelView.AuthViewModel.LoginData;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace JamalKhanah.Controllers.MVC;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAccountService _accountService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
  

    public AccountController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IAccountService accountService, SignInManager<ApplicationUser> signInManager)
    {
        this._signInManager = signInManager;
        _accountService = accountService;
        _unitOfWork = unitOfWork;
        _userManager = userManager;

    }

    public IActionResult Login()
    {
       // SeedQuran.SeedUser(_unitOfWork, _userManager).Wait();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel loginUser)
    {
        if (!ModelState.IsValid)
        {
            return View(loginUser);
        }
        var result = await _accountService.LoginAsync(loginUser);
        if (!result.IsAuthenticated)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(loginUser);
        }
        var user = await _accountService.GetUserByPhoneNumber(loginUser.PhoneNumber);
        
        if (user.IsAdmin)
        {
            await _signInManager.SignInAsync(user, loginUser.IsPersist);
            return RedirectToAction("Index", "Dashboard");
        }
        ModelState.AddModelError(string.Empty, "لا تملك الصلاحية اللازمه للدخول");
        return View(loginUser);
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        if (User.Identity != null)
        {
            var userName = User.Identity.Name;
            await _accountService.Logout(userName);
        }

        await _signInManager.SignOutAsync();//expires cookie
        return RedirectToAction("Login");
    }

  /*  [Authorize]
    public async Task<IActionResult> CheckUser()
    {
        if (User.Identity != null)
        {
            var userName = User.Identity.Name;
            var user = await _unitOfWork.Users.FindAsync(s=>s.UserName.Equals(userName));
            if (user.IsAdmin)
            {
                return RedirectToAction("Details", "Admin", new { id = user.Id });
            }
            else
            {
                return RedirectToAction("Details", "Teacher", new { id = user.Id });
            }
        }
        else
        {
            return NotFound();
        }
    }*/

    //------------------------------------------------------------------------------------------------------- add Roles to user

    /*[Authorize(Roles = "Admin")]
    [HttpGet]//from anchor tag
    public IActionResult AddRole()
    {
        ViewBag.Roles = _accountService.GetRoles().Result.Where(x => x != "Admin" && x != "Teacher");
        var usersClient = _accountService.GetAllUsers().Result.Where(s => s.IsTeacher == true)
                 .Select(s => new { Id = s.Id, UserName = string.Format("{0} -----> {1}", s.FullName, s.PhoneNumber) });
        ViewBag.Users = new SelectList(usersClient, "Id", "UserName");
        AddRoleModel model = new AddRoleModel();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRole(AddRoleModel userRole)
    {
        var usersClient = _accountService.GetAllUsers().Result.Where(s => s.IsTeacher == true)
               .Select(s => new { Id = s.Id, UserName = string.Format("{0} -----> {1}", s.FullName, s.PhoneNumber) });
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = _accountService.GetRoles().Result.Where(x => x != "Admin" && x != "Teacher");

            ViewBag.Users = new SelectList(usersClient, "Id", "UserName");

            return View(userRole);
        }
        var result = await _accountService.AddRoleAsync(userRole);
        ViewBag.Roles = _accountService.GetRoles().Result.Where(x => x != "Admin" && x != "Teacher");
        ViewBag.Users = new SelectList(usersClient, "Id", "UserName");

        TempData["message"] = result;
        return View("AddRole", userRole);
    }*/
        

    //-------------------------------------------------------------------------------------------------------  open account by Admin
    [HttpGet]
    public IActionResult ChangePassword(string userId)
    {
        ChangePasswordMv model = new() { UserId = userId };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordMv model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var result = await _accountService.ChangePasswordAsync(model.UserId,model.Password);
        if (result.IsAuthenticated)
        {
            return RedirectToAction("Login");
        }
        else
        {
            ModelState.AddModelError(string.Empty, result.ArMessage);
            return View(model);
        }
    }





}
