using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.ModelView.AuthViewModel;
using JamalKhanah.Core.ModelView.AuthViewModel.ChangePasswordData;
using JamalKhanah.Core.ModelView.AuthViewModel.LoginData;
using JamalKhanah.Core.ModelView.AuthViewModel.RegisterData;
using JamalKhanah.Core.ModelView.AuthViewModel.RoleData;
using JamalKhanah.Core.ModelView.AuthViewModel.UpdateData;

namespace JamalKhanah.BusinessLayer.Interfaces;

public interface IAccountService
{
    Task<List<ApplicationUser>> GetAllUsers();
    Task<ApplicationUser> GetUserById(string userId);
    Task<ApplicationUser> GetUserByPhoneNumber(string phoneNumber);
    Task<ApplicationUser> GetUserByEmail(string email);
    Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);
    Task<AuthModel> UpdateLocation(string userId, UpdateUserLocation model);
    Task<AuthModel> RegisterCenterAsync(RegisterCenterVm model);
    Task<AuthModel> RegisterFreeAgentAsync(RegisterFreeAgentVm model);
    Task<AuthModel> RegisterAdminAsync(RegisterAdminMv model);
    Task<AuthModel> RegisterUserAsync(RegisterUserMv model);
    Task<AuthModel> LoginAsync(LoginModel model);
    Task<bool> Logout(string userName);
    Task<AuthModel> ReSendSms(string phoneNumber);
    Task<AuthModel> ForgetPassword(ForgotPasswordMv forgotPasswordModelView);
    Task<AuthModel> ConfirmSmsAsync(ConfirmSms confirmSms);
    Task<AuthModel> ChangePasswordAsync(string userId, string password);
    Task<AuthModel> ChangeOldPasswordAsync(string userId, ChangeOldPassword changePassword);
    Task<AuthModel> UpdateFreeAgentProfile(string userId, UpdateFreeAgentMv updateUser);
    Task<AuthModel> UpdateCenterProfile(string userId, UpdateCenterMv updateUser);
    Task<AuthModel> UpdateFreeAgentProfileAdmin(string userId, UpdateFreeAgentModel updateUser);
    Task<AuthModel> UpdateCenterProfileAdmin(string userId, UpdateCenterModel updateUser);
    
    Task<AuthModel> UpdateAdminProfile(string userId, UpdateAdminMv updateUser);
    Task<AuthModel> UpdateUserProfile(string userId, UpdateUserMv updateUser);
    Task<AuthModel> GetUserInfo(string userId);
    Task<string> AddRoleAsync(AddRoleModel model);
    Task<List<string>> GetRoles();
 
    string ValidateJwtToken(string token);
    int GenerateRandomNo();
    //------------------------------------------------------
    Task Activate(string userId);
    Task Suspend(string userId);
    Task Approve(string userId);
    Task ShowServices(string userId);
    Task HideServices(string userId);
    Task<bool> Reject(string userId);
    Task RemoveFeatured(string userId);
    Task MakeFeatured(string userId);
    string RandomString(int length);




}