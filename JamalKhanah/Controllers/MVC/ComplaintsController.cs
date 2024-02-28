using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JamalKhanah.Controllers.MVC;

[Authorize(Roles = "Admin")]
public class ComplaintsController : Controller
{

    private readonly IUnitOfWork _unitOfWork;

    public ComplaintsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    // GET: Complaints
    public async Task<IActionResult> Index()
    {
        var applicationContext =
            await _unitOfWork.Complaints.FindAllAsync(s => s.IsDeleted == false, include: s => s.Include(complaint => complaint.User));
        return View(applicationContext);
    }

    // GET: Complaints/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _unitOfWork.Complaints == null)
        {
            return NotFound();
        }

        var complaint = await _unitOfWork.Complaints.FindAsync(m => m.Id == id, include: s => s.Include(complaint => complaint.User));
              
        if (complaint == null)
        {
            return NotFound();
        }

        return View(complaint);
    }

       

       

       

    // GET: Complaints/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _unitOfWork.Complaints == null)
        {
            return NotFound();
        }

        var complaint = await _unitOfWork.Complaints.FindAsync(m => m.Id == id, include: s => s.Include(complaint => complaint.User));
                
        if (complaint == null)
        {
            return NotFound();
        }
        complaint.IsDeleted = true;
        complaint.DeletedAt = DateTime.Now;

        _unitOfWork.Complaints.Update(complaint);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

}
