using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JamalKhanah.Core.Entity.Other;
using JamalKhanah.RepositoryLayer.Interfaces;

namespace JamalKhanah.Controllers.MVC;

[Authorize(Roles = "Admin")]
public class CitiesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CitiesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: Cities
    public async Task<IActionResult> Index()
    {
        return View(await _unitOfWork.Cities.FindAllAsync(s => s.IsDeleted == false));
    }

    // GET: Cities/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _unitOfWork.Cities == null)
        {
            return NotFound();
        }

        var city = await _unitOfWork.Cities.FindAsync(m => m.Id == id);
        if (city == null)
        {
            return NotFound();
        }

        return View(city);
    }

    // GET: Cities/Create
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(City city)
    {
        if (!ModelState.IsValid) return View(city);
        await _unitOfWork.Cities.AddAsync(city);
        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Cities/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _unitOfWork.Cities == null)
        {
            return NotFound();
        }

        var city = await _unitOfWork.Cities.FindAsync(m => m.Id == id);
        if (city == null)
        {
            return NotFound();
        }
        return View(city);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, City city)
    {
        if (id != city.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _unitOfWork.Cities.Update(city);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(city.Id))
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
        return View(city);
    }

    // GET: Cities/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _unitOfWork.Cities == null)
        {
            return NotFound();
        }

        var city = await _unitOfWork.Cities.FindAsync(m => m.Id == id);
        if (city == null)
        {
            return NotFound();
        }

        return View(city);
    }

    // POST: Cities/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_unitOfWork.Cities == null)
        {
            return Problem("Entity set 'ApplicationContext.Cities'  is null.");
        }
        var city = await _unitOfWork.Cities.FindAsync(m => m.Id == id);
        if (city != null)
        {
            city.IsDeleted = true;
            city.IsShow = false;
            _unitOfWork.Cities.Update(city);
            await _unitOfWork.SaveChangesAsync();
              
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Show(int? id)
    {
        if (id == null || _unitOfWork.Cities == null)
        {
            return NotFound();
        }

        var city = await _unitOfWork.Cities.FindAsync(m => m.Id == id);
        if (city == null)
        {
            return NotFound();
        }
        city.IsShow = true;
        _unitOfWork.Cities.Update(city);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Hide(int? id)
    {
        if (id == null || _unitOfWork.Cities == null)
        {
            return NotFound();
        }

        var city = await _unitOfWork.Cities.FindAsync(m => m.Id == id);
        if (city == null)
        {
            return NotFound();
        }
        city.IsShow = false;
        _unitOfWork.Cities.Update(city);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool CityExists(int id)
    {
        return _unitOfWork.Cities.IsExist(e => e.Id == id);
    }
}
