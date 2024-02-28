using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.ModelView.AuthViewModel.ChangePasswordData;

public class ChangeOldPassword
{
    [Required]
    public string OldPassword { get; set; }

    [Required]
    public string NewPassword { get; set; }
}