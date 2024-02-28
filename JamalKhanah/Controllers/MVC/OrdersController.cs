using JamalKhanah.Core.Helpers;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JamalKhanah.Controllers.MVC
{
    public class OrdersController : Controller
    { 
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var applicationContext =
                await _unitOfWork.Orders.FindAllAsync(s=>s.IsDeleted==false && s.OrderStatus != OrderStatus.Initialized 
                    , include:s=> s.Include(e => e.User).Include(s=>s.Service).ThenInclude(s=>s.Provider), orderBy: s => s.OrderBy(e => e.UserId));
            return View( applicationContext);
        }
    }
}
