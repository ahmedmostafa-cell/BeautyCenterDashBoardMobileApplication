using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JamalKhanah.Core.Entity.ProfileData;
using JamalKhanah.Core.Helpers;
using JamalKhanah.RepositoryLayer.Interfaces;

namespace JamalKhanah.Controllers.MVC;

public class ExperiencesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ExperiencesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: Experiences
    public async Task<IActionResult> Index(string userId )
    {
        if (!string.IsNullOrEmpty(userId))
        {
            var all = await _unitOfWork.Experiences.FindAllAsync(s => s.UserId == userId && s.IsDeleted == false, include: s => s.Include(e => e.User));
            return View(all);
        }
   
        var applicationContext =await _unitOfWork.Experiences.FindAllAsync(s=>s.IsDeleted==false , include:s=> s.Include(e => e.User), orderBy: s => s.OrderBy(e => e.UserId));
        return View( applicationContext);
    }

 
    // GET: Experiences/Create
    public async Task<IActionResult> Create()
    {
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");

        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create( Experience experience)
    {
   
        if (ModelState.IsValid)
        {
            await _unitOfWork.Experiences.AddAsync(experience);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index),new {experience.UserId});
        }
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");
        return View(experience);
    }

    // GET: Experiences/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _unitOfWork.Experiences == null)
        {
            return NotFound();
        }

        var experience = await _unitOfWork.Experiences.FindAsync(s => s.Id == id, include: s => s.Include(e => e.User));
        if (experience == null)
        {
            return NotFound();
        }
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");
        return View(experience);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,  Experience experience)
    {
        if (id != experience.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _unitOfWork.Experiences.Update(experience);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExperienceExists(experience.Id))
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
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");
        return View(experience);
    }

    // GET: Experiences/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _unitOfWork.Experiences == null)
        {
            return NotFound();
        }

        var experience = await _unitOfWork.Experiences.FindAsync(m => m.Id == id, include: s => s.Include(address => address.User));
                
        if (experience == null)
        {
            return NotFound();
        }
        experience.IsDeleted = true;
        experience.DeletedAt = DateTime.Now;

        _unitOfWork.Experiences.Update(experience);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

  

    private bool ExperienceExists(int id)
    {
        return _unitOfWork.Experiences.IsExist(e => e.Id == id);
    }
}