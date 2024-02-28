using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.Core.Entity.CommissionsData;
using JamalKhanah.Core.Helpers;
using JamalKhanah.Core.ModelView.MV;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JamalKhanah.Controllers.MVC;

[Authorize(Roles = "Admin")]
public class CommissionsController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IUnitOfWork _unitOfWork;


    public CommissionsController(IUnitOfWork unitOfWork, IAccountService accountService)
    {
        _accountService = accountService;
        _unitOfWork = unitOfWork;
    }

    // GET: ServiceProvider
    public async Task<ActionResult> ServiceProviderCommission()
    {
      
        var allAdmins = await _unitOfWork.Users.FindByQuery(s => (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent) && s.IsApproved == true,
                include: s=>s.Include(s=>s.Orders).Include(s=>s.Commissions))
            .Select(s=>new CommissionModelView
            {
                Id = s.Id,
                Name = s.FullName,
                Photo = s.UserImgUrl,
                TotalOrder = s.Services.SelectMany(service => service.Orders.Where(o => o.IsDeleted == false && o.IsPaid == true)).Count(),
                TotalOrderAmount = s.Services.SelectMany(service=>service.Orders.Where(o => o.IsDeleted == false && o.IsPaid == true)).Sum(order=>order.Total),
                TotalCommission = 5.0 / 100 * (s.Services.SelectMany(service => service.Orders.Where(o => o.IsDeleted == false && o.IsPaid == true)).Sum(order => order.Total)),
                TotalPaid = s.Commissions.Where(o=>o.IsDeleted==false).Sum(o=>o.Amount),
                TotalRemaining = s.Services.SelectMany(service => service.Orders.Where(o => o.IsDeleted == false && o.IsPaid == true)).Sum(order => order.Total) - (s.Commissions.Where(o => o.IsDeleted == false).Sum(o => o.Amount))
            }).ToListAsync();

     

        return View(allAdmins);
    }
    //--------------------------------------------------------------------------------------

    public async Task<ActionResult> Index(string id )
    {
        if (id == null)
        {
            var data =await _unitOfWork.Commissions.FindByQuery(s => s.IsDeleted == false, include: s => s.Include(commission=>commission.Provider))
                .ToListAsync();
            return View(data);
        }
        var dataForProvider = await _unitOfWork.Commissions.FindByQuery(s => s.IsDeleted == false && s.ProviderId==id, include: s => s.Include(commission => commission.Provider))
            .ToListAsync();

        return View(dataForProvider);
    }

    //-------------------------
    public IActionResult Create()
    {
        ViewData["UserId"] = new SelectList(_unitOfWork.Users.FindAll(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
        return View(new Commission());
    }

 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Commission model)
    {
        if (ModelState.IsValid)
        {
            var user = await _accountService.GetUserById(model.ProviderId);
            if (user == null)
            {
                ModelState.AddModelError("", "يجب اختيار مقدم الخدمة");
                ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
                return View(model);
            }
            await _unitOfWork.Commissions.AddAsync(model);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
        return View(model);
    }

    // GET: Commissions/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var commission = await _unitOfWork.Commissions.FindByQuery(s=>s.Id==id.Value && s.IsDeleted==false).FirstOrDefaultAsync();
        if (commission == null)
        {
            return NotFound();
        }
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName", commission.ProviderId);
        return View(commission);
    }

    // POST: Commissions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Commission model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var user = await _accountService.GetUserById(model.ProviderId);
                if (user == null)
                {
                    ModelState.AddModelError("", "يجب اختيار مقدم الخدمة");
                    ViewData["UserId"] =
                        new SelectList(
                            await _unitOfWork.Users.FindAllAsync(s =>
                                s.IsApproved == true && s.Status == true &&
                                (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
                    return View(model);
                }

                _unitOfWork.Commissions.Update(model);
                await _unitOfWork.SaveChangesAsync();
            }
            catch 
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName", model.ProviderId);
        return View(model);
    }

    //--------------------------------------------------------
    public async Task<IActionResult> Delete(int id)
    {
        if (_unitOfWork.Commissions == null)
        {
            return Problem("Entity set 'ApplicationContext.MainSections'  is null.");
        }
        var commission = await _unitOfWork.Commissions
            .FindByQuery(
                criteria: s => s.Id == id && s.IsDeleted == false).FirstOrDefaultAsync();
        if (commission != null)
        {
            commission.IsDeleted = true;
            commission.DeletedAt = DateTime.Now;
            _unitOfWork.Commissions.Update(commission);
        }

        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }





}