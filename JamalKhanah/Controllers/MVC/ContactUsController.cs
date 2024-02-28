using JamalKhanah.Core.Entity.Other;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JamalKhanah.Controllers.MVC;

[Authorize(Roles = "Admin")]
public class ContactUsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ContactUsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: ContactUs
    public async Task<IActionResult> Index()
    {
        return View(await _unitOfWork.ContactUs.GetAllAsync());
    }



    // GET: ContactUs/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _unitOfWork.ContactUs == null)
        {
            return NotFound();
        }

        var contactUs = await _unitOfWork.ContactUs
            .FindAsync(m => m.Id == id);
        if (contactUs == null)
        {
            return NotFound();
        }

        return View(contactUs);
    }

    // GET: ContactUs/Create
    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContactUs contactUs)
    {
        if (!ModelState.IsValid) return View(contactUs);
        await _unitOfWork.ContactUs.AddAsync(contactUs);
        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _unitOfWork.ContactUs == null)
        {
            return NotFound();
        }

        var contactUs = await _unitOfWork.ContactUs.GetByIdAsync(id.Value);
        if (contactUs == null)
        {
            return NotFound();
        }
        return View(contactUs);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,ContactUs contactUs)
    {
        if (id != contactUs.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid) return View(contactUs);
        try
        {
            _unitOfWork.ContactUs.Update(contactUs);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ContactUsExists(contactUs.Id))
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

        

    private bool ContactUsExists(int id)
    {
        return _unitOfWork.ContactUs.IsExist(e => e.Id == id);
    }
}
