using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JamalKhanah.Core.Entity.ProfileData;
using JamalKhanah.Core.Helpers;
using JamalKhanah.RepositoryLayer.Interfaces;
using JamalKhanah.BusinessLayer.Interfaces;

namespace JamalKhanah.Controllers.MVC;

public class EmployeesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileHandling _fileHandling;

    public EmployeesController(IUnitOfWork unitOfWork, IFileHandling fileHandling)
    {
        _unitOfWork = unitOfWork;
        _fileHandling = fileHandling;
    }

    // GET: Employees
    public async Task<IActionResult> Index(string userId )
    {
        if (!string.IsNullOrEmpty(userId))
        {
            var all = await _unitOfWork.Employees.FindAllAsync(s => s.UserId == userId && s.IsDeleted == false, include: s => s.Include(e => e.User));
            return View(all);
        }
        var applicationContext = await _unitOfWork.Employees.FindAllAsync(s=>s.IsDeleted==false, include:s=>s.Include(e => e.User));
        return View(applicationContext);
    }

    // GET: Employees/Create
    public IActionResult Create()
    {
        ViewData["UserId"] = new SelectList(_unitOfWork.Users.FindAll(s=>s.IsApproved==true && s.Status==true && s.UserType== UserType.Center), "Id", "FullName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee employee)
    {
        if (ModelState.IsValid)
        {
            if (employee.ImgFile == null)
            {
                ModelState.AddModelError("", "يجب ادخال صورة");
                ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && s.UserType== UserType.Center), "Id", "FullName");
                return View(employee);
            }
            var img = await _fileHandling.UploadFile(employee.ImgFile, "Employees");
            employee.ImgUrl = img;

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && s.UserType== UserType.Center), "Id", "FullName");
        return View(employee);
    }

    // GET: Employees/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _unitOfWork.Employees == null)
        {
            return NotFound();
        }

        var employee = await _unitOfWork.Employees.FindAsync(s=>s.Id==id && s.IsDeleted==false );
        if (employee == null)
        {
            return NotFound();
        }
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && s.UserType== UserType.Center), "Id", "FullName");
        return View(employee);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee employee)
    {
        if (id != employee.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if ((employee.ImgFile == null && string.IsNullOrEmpty(employee.ImgUrl)))
                {
                    ModelState.AddModelError("", "يجب ادخال صورة");
                    ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && s.UserType== UserType.Center), "Id", "FullName");
                    return View(employee);
                }
                if (employee.ImgFile != null)
                {
                    employee.ImgUrl = await _fileHandling.UploadFile(employee.ImgFile, "SlidePhotos", employee.ImgUrl);
                }
                

                _unitOfWork.Employees.Update(employee);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employee.Id))
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
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s=>s.IsApproved==true && s.Status==true && s.UserType== UserType.Center), "Id", "FullName");
        return View(employee);
    }

    // GET: Employees/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _unitOfWork.Employees == null)
        {
            return NotFound();
        }

        var employee = await _unitOfWork.Employees.FindAsync(m => m.Id == id, include: s => s.Include(address => address.User));
                
        if (employee == null)
        {
            return NotFound();
        }
        employee.IsDeleted = true;
        employee.DeletedAt = DateTime.Now;

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
   

    private bool EmployeeExists(int id)
    {
        return _unitOfWork.Employees.IsExist(e => e.Id == id);
    }
}