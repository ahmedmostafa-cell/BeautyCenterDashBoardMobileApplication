using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JamalKhanah.Core.Entity.ProfileData;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace JamalKhanah.Controllers.MVC;

[Authorize(Roles = "Admin")]
public class AddressesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AddressesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: Addresses
    public async Task<IActionResult> Index(string userId )
    {
        if (!string.IsNullOrEmpty(userId))
        {
            var all = await _unitOfWork.Addresses.FindAllAsync(s => s.UserId == userId && s.IsDeleted == false, include: s => s.Include(e => e.User));
            return View(all);
        }
        var applicationContext = await _unitOfWork.Addresses.FindAllAsync(s => s.IsDeleted == false, include: s => s.Include(a => a.User), orderBy: s => s.OrderBy(a => a.UserId));
        return View( applicationContext);
    }


    // GET: Addresses/Create
    public IActionResult Create()
    {
        ViewData["UserId"] = new SelectList(_unitOfWork.Users.FindAll(s=>s.IsApproved==true && s.Status==true), "Id", "FullName");
        ViewData["CityId"] = new SelectList(_unitOfWork.Cities.FindAll(s=>s.IsShow==true && s.IsDeleted==false), "Id", "NameAr");

        return View();
    }

  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create( Address address)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.Addresses.AddAsync(address);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index),new {address.UserId});
        }
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true), "Id", "FullName");
        ViewData["CityId"] = new SelectList(await _unitOfWork.Cities.FindAllAsync(s=>s.IsShow==true && s.IsDeleted==false), "Id", "NameAr");
        return View(address);
    }

    // GET: Addresses/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _unitOfWork.Addresses == null)
        {
            return NotFound();
        }

        var address = await _unitOfWork.Addresses.FindAsync(s=>s.Id==id,include:s=>s.Include(address=>address.UserId));
        if (address == null)
        {
            return NotFound();
        }
        ViewData["CityId"] = new SelectList(await _unitOfWork.Cities.FindAllAsync(s=>s.IsShow==true && s.IsDeleted==false), "Id", "NameAr");
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true), "Id", "FullName");
        return View(address);
    }

   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Address address)
    {
        if (id != address.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _unitOfWork.Addresses.Update(address);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(address.Id))
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
        ViewData["CityId"] = new SelectList(await _unitOfWork.Cities.FindAllAsync(s=>s.IsShow==true && s.IsDeleted==false), "Id", "NameAr");
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true), "Id", "FullName");
        return View(address);
    }

    // GET: Addresses/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _unitOfWork.Addresses == null)
        {
            return NotFound();
        }

        var address = await _unitOfWork.Addresses.FindAsync(m => m.Id == id, include: s => s.Include(address => address.User));
                
        if (address == null)
        {
            return NotFound();
        }
        address.IsDeleted = true;
        address.DeletedAt = DateTime.Now;

        _unitOfWork.Addresses.Update(address);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

  
    private bool AddressExists(int id)
    {
        return _unitOfWork.Addresses.IsExist(e => e.Id == id);
    }
}