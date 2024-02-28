using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.ModelView.AuthViewModel.UpdateData;

public class UpdateUserLocation
{
    [Required]
    public float Lat { get; set; }
    [Required]
    public float Lng { get; set; }
}