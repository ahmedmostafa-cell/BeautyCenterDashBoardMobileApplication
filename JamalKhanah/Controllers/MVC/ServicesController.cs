using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JamalKhanah.Core.Entity.SectionsData;
using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using JamalKhanah.Core.Helpers;

namespace JamalKhanah.Controllers.MVC;

[Authorize(Roles = "Admin")]
public class ServicesController  : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFileHandling _fileHandling;
    private ApplicationUser _user;

    public ServicesController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IFileHandling fileHandling)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _fileHandling = fileHandling;

    }
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = _userManager.GetUserId(User);
        _user = _unitOfWork.Users.Find(s => s.Id == userId);
    }

//---------------------------------------------------------------------------------------------

    // GET: Services
    public async Task<IActionResult> Index()
    {
        var applicationContext = await _unitOfWork.Services.FindAllAsync(s=>s.IsDeleted==false, include:s=>s.Include(service => service.MainSection).Include(service=>service.Provider));
        return View(applicationContext);
    }

    // GET: Services/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _unitOfWork.Services == null)
        {
            return NotFound();
        }

        var service = await _unitOfWork.Services.FindAsync(m => m.Id == id && m.IsDeleted == false, include: s => s.Include(service => service.MainSection).Include(service=>service.Provider));
        if (service == null)
        {
            return NotFound();
        }

        return View(service);
    }

    // GET: Services/Create
    public IActionResult Create()
    {
        ViewData["MainSectionId"] = new SelectList(_unitOfWork.MainSections.FindAll(s=>s.IsDeleted==false && s.IsShow==true), "Id", "TitleAr");
        ViewData["UserId"] = new SelectList( _unitOfWork.Users.FindAll(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");
        return View(new Service());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create( Service service)
    {
        if (ModelState.IsValid)
        {
            if (service.ImgFile == null)
            {
                ModelState.AddModelError("", "يجب ادخال صورة");
                ViewData["MainSectionId"] = new SelectList(_unitOfWork.MainSections.FindAll(s=>s.IsDeleted==false && s.IsShow==true), "Id", "TitleAr");
                ViewData["UserId"] = new SelectList( _unitOfWork.Users.FindAll(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");
                return View(service);
            }
            service.ImgUrl = await _fileHandling.UploadFile(service.ImgFile, "Services");
            var user = await _userManager.FindByIdAsync(service.ProviderId);
            if (user == null)
            {
                ModelState.AddModelError("", "يجب اختيار مقدم الخدمة");
                ViewData["MainSectionId"] = new SelectList(_unitOfWork.MainSections.FindAll(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
                ViewData["UserId"] = new SelectList(_unitOfWork.Users.FindAll(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
                return View(service);
            }
            if (user.UserType == UserType.Center)
            {
                service.ServiceType = ServiceType.Center;
            }
            else
            {
                service.ServiceType = ServiceType.FreeAgent;
                service.InCenter = false;
                service.EmployeesNumber = 0;
            }
            if (service.InCenter == false && service.InHome == false)
            {
                ModelState.AddModelError("", "يجب اختيار نوع الخدمة في المنزل او في المركز");
                ViewData["MainSectionId"] = new SelectList(_unitOfWork.MainSections.FindAll(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
                ViewData["UserId"] = new SelectList(_unitOfWork.Users.FindAll(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
                return View(service);
            }
         

            await _unitOfWork.Services.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s=>s.IsDeleted==false && s.IsShow==true), "Id", "TitleAr", service.MainSectionId);
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");
        return View(service);
    }

    // GET: Services/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _unitOfWork.Services == null)
        {
            return NotFound();
        }

        var service = await _unitOfWork.Services.FindAsync(m => m.Id == id && m.IsDeleted == false, include: s => s.Include(service => service.MainSection).Include(service=>service.Provider));
        if (service == null)
        {
            return NotFound();
        }
        ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s=>s.IsDeleted==false && s.IsShow==true), "Id", "TitleAr", service.MainSectionId);
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");
        return View(service);
    }

  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Service service)
    {
        if (id != service.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if ((service.ImgFile == null && string.IsNullOrEmpty(service.ImgUrl)))
                {
                    ModelState.AddModelError("", "يجب ادخال صورة");
                    return View(service);
                }
                if (service.ImgFile != null)
                {
                    service.ImgUrl = await _fileHandling.UploadFile(service.ImgFile, "Services", service.ImgUrl);
                }
                var user = await _userManager.FindByIdAsync(service.ProviderId);
                if (user == null)
                {
                    ModelState.AddModelError("", "يجب اختيار مقدم الخدمة");
                    ViewData["MainSectionId"] = new SelectList(_unitOfWork.MainSections.FindAll(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
                    ViewData["UserId"] = new SelectList(_unitOfWork.Users.FindAll(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
                    return View(service);
                }
                if (user.UserType == UserType.Center)
                {
                    service.ServiceType = ServiceType.Center;
                }
                else
                {
                    service.ServiceType = ServiceType.FreeAgent;
                    service.InCenter = false;
                    service.EmployeesNumber = 0;
                }
                if (service.InCenter == false && service.InHome == false)
                {
                    ModelState.AddModelError("", "يجب اختيار نوع الخدمة في المنزل او في المركز");
                    ViewData["MainSectionId"] = new SelectList(_unitOfWork.MainSections.FindAll(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
                    ViewData["UserId"] = new SelectList(_unitOfWork.Users.FindAll(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
                    return View(service);
                }
                _unitOfWork.Services.Update(service);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(service.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s=>s.IsDeleted==false && s.IsShow==true), "Id", "TitleAr", service.MainSectionId);
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");
        return View(service);
    }

     public async Task<IActionResult> Delete(int id)
    {
        if (_unitOfWork.Services == null)
        {
            return Problem("Entity set 'ApplicationContext.MainSections'  is null.");
        }
        var services = await _unitOfWork.Services
            .FindByQuery(
                criteria: s => s.Id == id && s.IsDeleted == false).FirstOrDefaultAsync();
        if (services != null)
        {
            services.IsDeleted = true;
            services.IsShow = false;
            services.DeletedAt = DateTime.Now;
            _unitOfWork.Services.Update(services);
        }

        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Show(int? id)
    {
        if (id == null || _unitOfWork.Services == null)
        {
            return NotFound();
        }

        var services = await _unitOfWork.Services.FindAsync(m => m.Id == id);
        if (services == null)
        {
            return NotFound();
        }
        services.IsShow = true;
        _unitOfWork.Services.Update(services);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Hide(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var services = await _unitOfWork.Services.FindAsync(m => m.Id == id);
        if (services == null)
        {
            return NotFound();
        }
        services.IsShow = false;
        _unitOfWork.Services.Update(services);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
  
    public async Task<IActionResult> Featured(int? id)
    {
        if (id == null || _unitOfWork.Services == null)
        {
            return NotFound();
        }

        var services = await _unitOfWork.Services.FindAsync(m => m.Id == id);
        if (services == null)
        {
            return NotFound();
        }
        services.IsFeatured = true;
        _unitOfWork.Services.Update(services);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> NotFeatured(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var services = await _unitOfWork.Services.FindAsync(m => m.Id == id);
        if (services == null)
        {
            return NotFound();
        }
        services.IsFeatured = false;
        _unitOfWork.Services.Update(services);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    private bool ServiceExists(int id)
    {
        return _unitOfWork.Services.IsExist(e => e.Id == id);
    }

    #region  AJAX

    // check user type 
    public async Task<IActionResult> CheckUserType(string userId)
    {
        if (userId == null)
        {
             return Json(new { Error = true, Message = "يجب تحديد مقدم الخدمة أولا  " });
        }

        var user = await _unitOfWork.Users.FindAsync(m => m.Id == userId);
        if (user == null)
        {
            return Json(new { Error = true, Message = "يجب تحديد مقدم الخدمة أولا  " });
        }

        return user.UserType switch
        {
            UserType.Center => Json(new { Error = false, type = 1, Message = "مقدم الخدمة مركز" }),
            UserType.FreeAgent => Json(new { Error = false, type = 2, Message = "مقدم الخدمة مستقل" }),
            _ => Json(new { Error = true, Message = "يجب تحديد مقدم الخدمة أولا  " })
        };
    }

    #endregion
}