using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.Other;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace JamalKhanah.Controllers.MVC;

[Authorize(Roles = "Admin")]
public class SlidePhotosController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IFileHandling _fileHandling;
    private ApplicationUser _user;

    public SlidePhotosController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IFileHandling fileHandling)
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

    // GET: SlidePhotoes
    public async Task<IActionResult> Index()
    {
        var applicationContext = await _unitOfWork.SlidePhotos.FindAllAsync(
            criteria: s => s.IsDeleted == false);
        return View(applicationContext);
    }

    // GET: SlidePhotoes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _unitOfWork.SlidePhotos == null)
        {
            return NotFound();
        }

        var slidePhoto = await _unitOfWork.SlidePhotos
            .FindByQuery(
                criteria: s => s.Id == id && s.IsDeleted == false).FirstOrDefaultAsync();

        if (slidePhoto == null)
        {
            return NotFound();
        }

        return View(slidePhoto);
    }

    // GET: SlidePhotoes/Create
    public IActionResult CreateAsync()
    {
        return View(new SlidePhoto());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SlidePhoto slidePhoto)
    {
        if (!ModelState.IsValid) return View(slidePhoto);
        if (slidePhoto.ImgFile == null)
        {
            ModelState.AddModelError("", "يجب ادخال صورة");
            return View(slidePhoto);
        }

        slidePhoto.ImgUrl = await _fileHandling.UploadFile(slidePhoto.ImgFile, "SlidePhotos");

        await _unitOfWork.SlidePhotos.AddAsync(slidePhoto);
        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));

    }


    // GET: SlidePhotoes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _unitOfWork.SlidePhotos == null)
        {
            return NotFound();
        }

        var slidePhoto = await _unitOfWork.SlidePhotos
            .FindByQuery(
                criteria: s => s.Id == id && s.IsDeleted == false).FirstOrDefaultAsync();

        if (slidePhoto == null)
        {
            return NotFound();
        }
        return View(slidePhoto);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SlidePhoto slidePhoto)
    {
        if (id != slidePhoto.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid) return View(slidePhoto);
        try
        {

            if ((slidePhoto.ImgFile == null && string.IsNullOrEmpty(slidePhoto.ImgUrl)))
            {
                ModelState.AddModelError("", "يجب ادخال صورة");
                return View(slidePhoto);
            }
            if (slidePhoto.ImgFile != null)
            {
                slidePhoto.ImgUrl = await _fileHandling.UploadFile(slidePhoto.ImgFile, "SlidePhotos", slidePhoto.ImgUrl);
            }

            _unitOfWork.SlidePhotos.Update(slidePhoto);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SlidePhotoExists(slidePhoto.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }

    // GET: SlidePhotoes/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        if (_unitOfWork.SlidePhotos == null)
        {
            return Problem("Entity set 'ApplicationContext.SlidePhotos'  is null.");
        }
        var slidePhoto = await _unitOfWork.SlidePhotos
            .FindByQuery(
                criteria: s => s.Id == id && s.IsDeleted == false).FirstOrDefaultAsync();
        if (slidePhoto != null)
        {
            slidePhoto.IsDeleted = true;
            slidePhoto.IsShow = false;
            slidePhoto.DeletedAt = DateTime.Now;
            _unitOfWork.SlidePhotos.Update(slidePhoto);
        }

        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SlidePhotoExists(int id)
    {
        return _unitOfWork.SlidePhotos.IsExist(e => e.Id == id);
    }
    public async Task<IActionResult> Show(int? id)
    {
        if (id == null || _unitOfWork.SlidePhotos == null)
        {
            return NotFound();
        }

        var sidePhoto = await _unitOfWork.SlidePhotos.FindAsync(m => m.Id == id);
        if (sidePhoto == null)
        {
            return NotFound();
        }
        sidePhoto.IsShow = true;
        _unitOfWork.SlidePhotos.Update(sidePhoto);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Hide(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sidePhoto = await _unitOfWork.SlidePhotos.FindAsync(m => m.Id == id);
        if (sidePhoto == null)
        {
            return NotFound();
        }
        sidePhoto.IsShow = false;
        _unitOfWork.SlidePhotos.Update(sidePhoto);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
