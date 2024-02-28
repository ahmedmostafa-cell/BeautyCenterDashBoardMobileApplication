
using JamalKhanah.Core.Helpers;

namespace JamalKhanah.Core.ModelView.AuthViewModel;

public class AuthModel
{

    public bool IsAuthenticated { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string ImgUrl { get; set; }
    public List<string> Roles { get; set; }
    public string Token { get; set; }
    public string FullName { get; set; }
    public bool PhoneVerify { get; set; }
    public bool Status { get; set; }
    public bool IsUser { get; set; } = false;
    public bool IsAdmin { get; set; } = false;

    public string Message { get; set; }
    public string ArMessage { get; set; }
    public int ErrorCode { get; set; }
    public string PhoneNumber { get; set; }
    public UserType UserType { get; set; }
    public bool IsApproved { get; set; }
    public float? Lat { get; set; }
    public float? Lng { get; set; }
    public string TaxNumber { get; set; }
    public string FreelanceFormUrl { get; set; }
    public string UserImgUrl { get; set; }
    public string Description { get; set; }

    public int  CityId { get; set; }
    public string City { get; set; }
}