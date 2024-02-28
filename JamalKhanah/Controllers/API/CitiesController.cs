using JamalKhanah.Core.DTO;
using JamalKhanah.Core.Helpers;
using JamalKhanah.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JamalKhanah.Controllers.API;

public class CitiesController : BaseApiController
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseResponse _baseResponse = new();

    public CitiesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET: api/Cities
    [HttpGet]
    public ActionResult<BaseResponse> Get([FromHeader] string lang)
    {
        var allCities =  _unitOfWork.Cities.FindAll(s=>s.IsShow==true && s.IsDeleted==false).ToList();
        if ( allCities.Any() )
        {
            _baseResponse.Data = lang == "ar" ? allCities.Select(s => new { s.Id, Name = s.NameAr, CountryName = s.CountryAr }) : allCities.Select(s => new { s.Id, Name = s.NameEn, CountryName = s.CountryEn });

            _baseResponse.ErrorCode = 0;
        }
        else
        {
            _baseResponse.ErrorCode = (int)Errors.NotFound;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد مدن لعرضها " : "There are no cities to display";
        }

        return Ok(_baseResponse);
    }

    // GET: api/Cities/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BaseResponse>> GetById([FromHeader] string lang, int id)
    {
        var city = await _unitOfWork.Cities.FindAsync(s => s.Id == id&& s.IsDeleted==false && s.IsShow == true);

        if (city == null)
        {
            _baseResponse.ErrorCode = (int)Errors.ThisCityNotExist;
            _baseResponse.ErrorMessage = (lang == "ar") ? "لا توجد مدن لعرضها " : "There are no cities to display";
        }
        else
        {
            _baseResponse.Data = lang == "ar" ? new { city.Id, Name = city.NameAr, CountryName = city.CountryAr } : new { city.Id, Name = city.NameEn, CountryName = city.CountryEn };

            _baseResponse.ErrorCode = 0;
        }


        return Ok(_baseResponse);
    }
}