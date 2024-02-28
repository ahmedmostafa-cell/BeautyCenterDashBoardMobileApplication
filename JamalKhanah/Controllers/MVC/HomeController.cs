using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JamalKhanah.Controllers.MVC;

public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public HomeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    //------------------------------------------------------------------------------------
    public IActionResult Index()
    {
       
        return View();
    }
  





    //----------------------------------------------------------
   

}
