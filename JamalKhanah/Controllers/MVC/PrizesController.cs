using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JamalKhanah.Core.Entity.ProfileData;
using JamalKhanah.Core.Helpers;
using JamalKhanah.RepositoryLayer.Interfaces;

namespace JamalKhanah.Controllers.MVC;

public class PrizesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public PrizesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: Prizes
    public async Task<IActionResult> Index(string userId )
    {
        if (!string.IsNullOrEmpty(userId))
        {
            var all = await _unitOfWork.Prizes.FindAllAsync(s => s.UserId == userId && s.IsDeleted == false, include: s => s.Include(e => e.User));
            return View(all);
        }
   
        var applicationContext =await _unitOfWork.Prizes.FindAllAsync(s=>s.IsDeleted==false , include:s=> s.Include(e => e.User), orderBy: s => s.OrderBy(e => e.UserId));
        return View( applicationContext);
    }

 
    // GET: Prizes/Create
    public async Task<IActionResult> Create()
    {
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");

        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create( Prize prize)
    {
    
        if (ModelState.IsValid)
        {
            await _unitOfWork.Prizes.AddAsync(prize);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index),new {prize.UserId});
        }
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");
        return View(prize);
    }

    // GET: Prizes/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _unitOfWork.Prizes == null)
        {
            return NotFound();
        }

        var prize = await _unitOfWork.Prizes.FindAsync(s => s.Id == id, include: s => s.Include(e => e.User));
        if (prize == null)
        {
            return NotFound();
        }
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && (s.UserType== UserType.Center || s.UserType== UserType.FreeAgent)), "Id", "FullName");
        return View(prize);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,  Prize prize)
    {
        if (id != prize.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _unitOfWork.Prizes.Update(prize);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrizeExists(prize.Id))
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
        return View(prize);
    }

    // GET: Prizes/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _unitOfWork.Prizes == null)
        {
            return NotFound();
        }

        var prize = await _unitOfWork.Prizes.FindAsync(m => m.Id == id, include: s => s.Include(address => address.User));
                
        if (prize == null)
        {
            return NotFound();
        }
        prize.IsDeleted = true;
        prize.DeletedAt = DateTime.Now;

        _unitOfWork.Prizes.Update(prize);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

  

    private bool PrizeExists(int id)
    {
        return _unitOfWork.Prizes.IsExist(e => e.Id == id);
    }
}