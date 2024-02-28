using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JamalKhanah.Core.Entity.ProfileData;
using JamalKhanah.RepositoryLayer.Interfaces;
using JamalKhanah.Core.Helpers;

namespace JamalKhanah.Controllers.MVC;

public class WorkHoursController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public WorkHoursController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: WorkHours
    public async Task<IActionResult> Index(string userId )
    {
        if (!string.IsNullOrEmpty(userId))
        {
            var all = await _unitOfWork.WorksHours.FindAllAsync(s => s.UserId == userId && s.IsDeleted == false, include: s => s.Include(e => e.User));
            return View(all);
        }

        var applicationContext = await _unitOfWork.WorksHours.FindAllAsync(s => s.IsDeleted == false, include: s => s.Include(e => e.User), orderBy: s => s.OrderBy(e => e.UserId));
        return View( applicationContext);
    }

      // GET: WorksHours/Create
    public async Task<IActionResult> Create()
    {
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");

        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create( WorkHours workHours)
    {

        if (ModelState.IsValid)
        {
            await _unitOfWork.WorksHours.AddAsync(workHours);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index),new {workHours.UserId});
        }
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");
        return View(workHours);
    }

    // GET: WorksHours/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _unitOfWork.WorksHours == null)
        {
            return NotFound();
        }

        var workHours = await _unitOfWork.WorksHours.FindAsync(s => s.Id == id, include: s => s.Include(e => e.User));
        if (workHours == null)
        {
            return NotFound();
        }
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");
        return View(workHours);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,  WorkHours workHours)
    {
        if (id != workHours.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _unitOfWork.WorksHours.Update(workHours);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkHoursExists(workHours.Id))
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
        return View(workHours);
    }

    // GET: WorksHours/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _unitOfWork.WorksHours == null)
        {
            return NotFound();
        }

        var workHours = await _unitOfWork.WorksHours.FindAsync(m => m.Id == id, include: s => s.Include(address => address.User));
                
        if (workHours == null)
        {
            return NotFound();
        }
        workHours.IsDeleted = true;
        workHours.DeletedAt = DateTime.Now;

        _unitOfWork.WorksHours.Update(workHours);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

  

    private bool WorkHoursExists(int id)
    {
        return _unitOfWork.WorksHours.IsExist(e => e.Id == id);
    }
}