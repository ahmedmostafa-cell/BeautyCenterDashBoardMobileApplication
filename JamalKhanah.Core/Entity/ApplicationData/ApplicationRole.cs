using Microsoft.AspNetCore.Identity;

namespace JamalKhanah.Core.Entity.ApplicationData;

public class ApplicationRole : IdentityRole
{
    public string NameAr { get; set; }

    public string Description { get; set; }

    public int RoleNumber { get; set; }
}