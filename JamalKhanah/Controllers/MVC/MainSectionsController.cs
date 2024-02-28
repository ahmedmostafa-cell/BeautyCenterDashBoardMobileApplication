using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JamalKhanah.Core.Entity.SectionsData;
using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;

namespace JamalKhanah.Controllers.MVC;

[Authorize(Roles = "Admin")]
public class MainSectionsController : Controller 
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFileHandling _fileHandling;
    private ApplicationUser _user;

    public MainSectionsController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IFileHandling fileHandling)
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

//-------------------------------------------------------------------------------------------------------------
    // GET: MainSections
    public async Task<IActionResult> Index()
    {
        return View(await _unitOfWork.MainSections.FindAllAsync(s=>s.IsDeleted==false));
    }

    // GET: MainSections/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _unitOfWork.MainSections == null)
        {
            return NotFound();
        }

        var mainSection = await _unitOfWork.MainSections
            .FindAsync(m => m.Id == id && m.IsDeleted == false);
        if (mainSection == null)
        {
            return NotFound();
        }

        return View(mainSection);
    }

    // GET: MainSections/Create
    public IActionResult Create()
    {
        return View();
    }

 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create( MainSection mainSection)
    {
        if (!ModelState.IsValid) return View(mainSection);
        if (mainSection.ImgFile == null)
        {
            ModelState.AddModelError("", "يجب ادخال صورة");
            return View(mainSection);
        }

        mainSection.ImgUrl = await _fileHandling.UploadFile(mainSection.ImgFile, "MainSection"); 
            
        await _unitOfWork.MainSections.AddAsync(mainSection);
        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: MainSections/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _unitOfWork.MainSections == null)
        {
            return NotFound();
        }

        var mainSection = await _unitOfWork.MainSections
            .FindAsync(m => m.Id == id && m.IsDeleted == false);
        if (mainSection == null)
        {
            return NotFound();
        }
        return View(mainSection);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MainSection mainSection)
    {
        if (id != mainSection.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid) return View(mainSection);
        try
        {
            if ((mainSection.ImgFile == null && string.IsNullOrEmpty(mainSection.ImgUrl)))
            {
                ModelState.AddModelError("", "يجب ادخال صورة");
                return View(mainSection);
            }
            if (mainSection.ImgFile != null)
            {
                mainSection.ImgUrl = await _fileHandling.UploadFile(mainSection.ImgFile, "MainSection", mainSection.ImgUrl);
            }
            _unitOfWork.MainSections.Update(mainSection);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MainSectionExists(mainSection.Id))
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

    public async Task<IActionResult> Delete(int id)
    {
        if (_unitOfWork.MainSections == null)
        {
            return Problem("Entity set 'ApplicationContext.MainSections'  is null.");
        }
        var mainSection = await _unitOfWork.MainSections
            .FindByQuery(
                criteria: s => s.Id == id && s.IsDeleted == false).FirstOrDefaultAsync();
        if (mainSection != null)
        {
            mainSection.IsDeleted = true;
            mainSection.IsShow = false;
            mainSection.DeletedAt = DateTime.Now;
            _unitOfWork.MainSections.Update(mainSection);
        }

        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Show(int? id)
    {
        if (id == null || _unitOfWork.MainSections == null)
        {
            return NotFound();
        }

        var mainSections = await _unitOfWork.MainSections.FindAsync(m => m.Id == id);
        if (mainSections == null)
        {
            return NotFound();
        }
        mainSections.IsShow = true;
        _unitOfWork.MainSections.Update(mainSections);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Hide(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var mainSections = await _unitOfWork.MainSections.FindAsync(m => m.Id == id);
        if (mainSections == null)
        {
            return NotFound();
        }
        mainSections.IsShow = false;
        _unitOfWork.MainSections.Update(mainSections);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
  
    public async Task<IActionResult> Featured(int? id)
    {
        if (id == null || _unitOfWork.MainSections == null)
        {
            return NotFound();
        }

        var mainSections = await _unitOfWork.MainSections.FindAsync(m => m.Id == id);
        if (mainSections == null)
        {
            return NotFound();
        }
        mainSections.IsFeatured = true;
        _unitOfWork.MainSections.Update(mainSections);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> NotFeatured(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var mainSections = await _unitOfWork.MainSections.FindAsync(m => m.Id == id);
        if (mainSections == null)
        {
            return NotFound();
        }
        mainSections.IsFeatured = false;
        _unitOfWork.MainSections.Update(mainSections);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool MainSectionExists(int id)
    {
        return _unitOfWork.MainSections.IsExist(e => e.Id == id);
    }
}