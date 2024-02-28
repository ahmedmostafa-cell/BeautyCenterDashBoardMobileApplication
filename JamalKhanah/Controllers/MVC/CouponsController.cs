using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JamalKhanah.Core.Entity.CouponData;
using JamalKhanah.BusinessLayer.Interfaces;
using JamalKhanah.RepositoryLayer.Interfaces;
using JamalKhanah.Core.Helpers;
using JamalKhanah.Core.ModelView.MV;

namespace JamalKhanah.Controllers.MVC
{
    public class CouponsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;


        public CouponsController(IUnitOfWork unitOfWork, IAccountService accountService)
        {
            _accountService = accountService;
            _unitOfWork = unitOfWork;
        }

        // GET: Coupons
        public async Task<IActionResult> Index()
        {
	        var data = await _unitOfWork.Coupons.FindByQuery(s => s.IsDeleted == false)
		        .Select(s => new CouponModelView
				{
			        Id = s.Id,
			        CouponCode = s.CouponCode,
			        CouponType = s.CouponType,
			        DiscountType = s.DiscountType,
			        Discount = s.Discount,
			        IsActive = s.IsActive,
			        TotalOrderUsed = s.Orders.Count,
		        }).ToListAsync();
              return View(data);
        }

        // GET: Coupons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _unitOfWork.Coupons == null)
            {
                return NotFound();
            }

            var coupon = await _unitOfWork.Coupons
                .FindAsync(m => m.Id == id,include:s=>s.Include(coupon=>coupon.Services)
	                .Include(coupon=>coupon.MainSections).Include(coupon1=>coupon1.Providers));
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // GET: Coupons/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_unitOfWork.Users.FindAll(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
            ViewData["MainSectionId"] = new SelectList(_unitOfWork.MainSections.FindAll(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
            ViewData["Services"] = new SelectList(_unitOfWork.Services.FindAll(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");

            return View(new Coupon(){CouponCode = _accountService.RandomString(8)});
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon model)
        {

            if (ModelState.IsValid)
            {
                switch (model.CouponType)
                {
                    case CouponType.MainSections when model.MainSectionsId.Any():
                    {
                        var data = await _unitOfWork.MainSections.FindByQuery(s => s.IsDeleted == false && s.IsShow == true).Select(s=>s.Id).ToListAsync();
                        if (model.MainSectionsId.Any(item => !data.Contains(item)))
                        {
                            ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
                            ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
                            ViewData["Services"] = new SelectList(await _unitOfWork.Services.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");

                            ModelState.AddModelError("CouponType", "يجب اختيار قسم موجود");
                            return View(model);
                        }

                        foreach (var item in model.MainSectionsId)
                        {
                            model.MainSections.Add(new MainSectionCoupon() {MainSectionId = item});
                        }
                      

                        break;
                    }
                    case CouponType.Service when model.ServicesId.Any():
                    {
                        var data = await _unitOfWork.Services.FindByQuery(s => s.IsDeleted == false && s.IsShow == true).Select(s => s.Id).ToListAsync();
                        if (model.ServicesId.Any(item => !data.Contains(item)))
                        {
                            ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
                            ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
                            ViewData["Services"] = new SelectList(await _unitOfWork.Services.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");

                            ModelState.AddModelError("CouponType", "يجب اختيار خدمة موجود");
                            return View(model);
                        }
                        foreach (var item in model.ServicesId)
                        {
                            model.Services.Add(new ServiceCoupon() { ServiceId = item });
                        }


                        break;
                    }
                    case CouponType.ServiceProvider when model.ProvidersId.Any():
                    {
                        var data = await _unitOfWork.Users.FindByQuery(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)).Select(s => s.Id).ToListAsync();
                        if (model.ProvidersId.Any(item => !data.Contains(item)))
                        {
                            ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
                            ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
                            ViewData["Services"] = new SelectList(await _unitOfWork.Services.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");

                            ModelState.AddModelError("CouponType", "يجب اختيار مقدم خدمة موجود");
                            return View(model);
                        }
                        foreach (var item in model.ProvidersId)
                        {
                            model.Providers.Add(new ApplicationUserCoupon() { ProviderId = item });
                        }


                        break;
                    }
                    default:
                        ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
                        ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
                        ViewData["Services"] = new SelectList(await _unitOfWork.Services.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");

                        ModelState.AddModelError("CouponType", "يجب اختيار نوع الكوبون وتحديد المستفيدين");
                        return View(model);
                }

                await _unitOfWork.Coupons.AddAsync(model);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
            ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
            ViewData["Services"] = new SelectList(await _unitOfWork.Services.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
            return View(model);
        }

        // GET: Coupons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _unitOfWork.Coupons == null)
            {
                return NotFound();
            }

            var coupon = await _unitOfWork.Coupons
                .FindAsync(m => m.Id == id, include: s => s.Include(coupon => coupon.Services).Include(coupon => coupon.MainSections).Include(coupon1 => coupon1.Providers));
            if (coupon == null)
            {
                return NotFound();
            }

            switch (coupon.CouponType)
            {
                case CouponType.MainSections :
                {
                    coupon.MainSectionsId = coupon.MainSections.Select(s => s.MainSectionId).ToList();
                    break;
                }
                case CouponType.Service:
                {
                    coupon.ServicesId = coupon.Services.Select(s => s.ServiceId).ToList();
                    break;
                }
                case CouponType.ServiceProvider:
                {
                    coupon.ProvidersId = coupon.Providers.Select(s => s.ProviderId).ToList();
                    break;
                }

                default:
                    TempData["Error"] = "لا يمكن تعديل الكوبون تواصل مع الدعم ";
                    return RedirectToAction("Index");
            }
            ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
            ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
            ViewData["Services"] = new SelectList(await _unitOfWork.Services.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
            return View(coupon);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Coupon model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            var oldCoupon = await _unitOfWork.Coupons
                .FindAsync(m => m.Id == id, include: s => s.Include(coupon => coupon.Services)
	                .Include(coupon => coupon.MainSections).Include(coupon1 => coupon1.Providers));
            if (oldCoupon == null)
            {
                return NotFound();
            }
            switch (oldCoupon.CouponType)
            {
                case CouponType.MainSections:
                {
                   _unitOfWork.MainSectionCoupons.DeleteRange(oldCoupon.MainSections);
                    break;
                }
                case CouponType.Service:
                {
                   _unitOfWork.ServiceCoupons.DeleteRange(oldCoupon.Services);
                    break;
                }
                case CouponType.ServiceProvider:
                {
                    _unitOfWork.UserCoupons.DeleteRange(oldCoupon.Providers);
                    break;
                }

                default:
                    TempData["Error"] = "لا يمكن تعديل الكوبون تواصل مع الدعم ";
                    return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    switch (model.CouponType)
                    {
                        case CouponType.MainSections when model.MainSectionsId.Any():
                            {
                                var data = await _unitOfWork.MainSections.FindByQuery(s => s.IsDeleted == false && s.IsShow == true).Select(s => s.Id).ToListAsync();
                                if (model.MainSectionsId.Any(item => !data.Contains(item)))
                                {
                                    ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
                                    ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
                                    ViewData["Services"] = new SelectList(await _unitOfWork.Services.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");

                                    ModelState.AddModelError("CouponType", "يجب اختيار قسم موجود");
                                    return View(model);
                                }

                                foreach (var item in model.MainSectionsId)
                                {
                                    model.MainSections.Add(new MainSectionCoupon() { MainSectionId = item });
                                }


                                break;
                            }
                        case CouponType.Service when model.ServicesId.Any():
                            {
                                var data = await _unitOfWork.Services.FindByQuery(s => s.IsDeleted == false && s.IsShow == true).Select(s => s.Id).ToListAsync();
                                if (model.ServicesId.Any(item => !data.Contains(item)))
                                {
                                    ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
                                    ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
                                    ViewData["Services"] = new SelectList(await _unitOfWork.Services.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");

                                    ModelState.AddModelError("CouponType", "يجب اختيار خدمة موجود");
                                    return View(model);
                                }
                                foreach (var item in model.ServicesId)
                                {
                                    model.Services.Add(new ServiceCoupon() { ServiceId = item });
                                }


                                break;
                            }
                        case CouponType.ServiceProvider when model.ProvidersId.Any():
                            {
                                var data = await _unitOfWork.Users.FindByQuery(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)).Select(s => s.Id).ToListAsync();
                                if (model.ProvidersId.Any(item => !data.Contains(item)))
                                {
                                    ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
                                    ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
                                    ViewData["Services"] = new SelectList(await _unitOfWork.Services.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");

                                    ModelState.AddModelError("CouponType", "يجب اختيار مقدم خدمة موجود");
                                    return View(model);
                                }
                                foreach (var item in model.ProvidersId)
                                {
                                    model.Providers.Add(new ApplicationUserCoupon() { ProviderId = item });
                                }


                                break;
                            }
                        default:
                            ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
                            ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
                            ViewData["Services"] = new SelectList(await _unitOfWork.Services.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");

                            ModelState.AddModelError("CouponType", "يجب اختيار نوع الكوبون");
                            return View(model);
                    }
                    _unitOfWork.Coupons.DeAttach(oldCoupon);
                    _unitOfWork.Coupons.Update(model);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponExists(model.Id))
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
            ViewData["UserId"] = new SelectList(await _unitOfWork.Users.FindAllAsync(s => s.IsApproved == true && s.Status == true && (s.UserType == UserType.Center || s.UserType == UserType.FreeAgent)), "Id", "FullName");
            ViewData["MainSectionId"] = new SelectList(await _unitOfWork.MainSections.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
            ViewData["Services"] = new SelectList(await _unitOfWork.Services.FindAllAsync(s => s.IsDeleted == false && s.IsShow == true), "Id", "TitleAr");
            return View(model);
        }

        // GET: Coupons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _unitOfWork.Coupons == null)
            {
                return NotFound();
            }

            var coupon = await _unitOfWork.Coupons
                .FindAsync(m => m.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }

            coupon.IsActive = false;
            coupon.IsDeleted = false;
            _unitOfWork.Coupons.Update(coupon);
            await _unitOfWork.SaveChangesAsync();
            TempData["Success"] = "تم الحذف بنجاح ";
            return RedirectToAction("Index");
        }

      
      

        private bool CouponExists(int id)
        {
          return _unitOfWork.Coupons.IsExist(e => e.Id == id);
        }
    }
}
