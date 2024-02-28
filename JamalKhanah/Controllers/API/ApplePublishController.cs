using JamalKhanah.Core.DTO;
using JamalKhanah.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JamalKhanah.Controllers.API;

public class ApplePublishController : BaseApiController
{
    private readonly ApplePublish _settings;
    private readonly BaseResponse _baseResponse;

    public ApplePublishController(IOptions<ApplePublish> settings)
    {
        _settings = settings.Value;
        _baseResponse = new BaseResponse();
    }

    [HttpGet]
    public ActionResult<BaseResponse> GetApplePublish()
    {
        _baseResponse.ErrorCode = 0;
        _baseResponse.ErrorMessage = "False";
        _baseResponse.Data = new { _settings.IsLive };
        return Ok(_baseResponse);
    }
}