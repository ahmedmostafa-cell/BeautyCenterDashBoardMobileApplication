using System.ComponentModel.DataAnnotations.Schema;
using JamalKhanah.Core.Entity.CommissionsData;
using JamalKhanah.Core.Entity.EvaluationData;
using JamalKhanah.Core.Entity.FavoriteData;
using JamalKhanah.Core.Entity.OrderData;
using JamalKhanah.Core.Entity.Other;
using JamalKhanah.Core.Entity.ProfileData;
using JamalKhanah.Core.Entity.SectionsData;
using JamalKhanah.Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace JamalKhanah.Core.Entity.ApplicationData;

public class ApplicationUser : IdentityUser
{

    public bool IsAdmin { get; set; } = false;//if true, user is admin
    public bool Status { get; set; } = true;//true=active,false=deactive
    public bool IsApproved { get; set; } = false;// this is for admin approval
    public bool ShowServices { get; set; } = true; // show services to user or not
    public bool IsFeatured { get; set; } = false; // هل متميز ام لا

    //------------------------------------------------------------------------
    public string FreelanceFormImgUrl { get; set; } // for freelance 
    public string TaxNumber { get; set; } // for Center
    //-------------------------------------------------------------------
    public UserType UserType { get; set; } // 0 = Admin,1 = NormalUser ,2 = Center, 3 = freelancer
    public string FullName { get; set; }
    public string Description { get; set; } // only for center and freelancer
    public DateTime RegistrationDate { get; set; } = DateTime.Now;
    public string DeviceToken { get; set; }
    public float? Lat { get; set; }
    public float? Lng { get; set; }
    public string RandomCode { get; set; }
    public string UserImgUrl { get; set; }
    //------------------------------------------------------------------------------------------------

    [ForeignKey("City")]
    public int? CityId { get; set; }
    public City City { get; set; }

    public IEnumerable<Complaint> Complaints { get; set; } = new List<Complaint>(); // for user
    public IEnumerable<Prize> Prizes { get; set; } = new List<Prize>(); // for center or freelancer
    public IEnumerable<Experience> Experiences { get; set; } = new List<Experience>(); // for center or freelancer
    public IEnumerable<WorkHours> WorkHours { get; set; } = new List<WorkHours>(); // for center or freelancer
    public IEnumerable<Employee> Employees { get; set; } = new List<Employee>(); // for center
    public IEnumerable<Address> Addresses { get; set; } = new List<Address>(); // for all users
    public IEnumerable<Order> Orders { get; set; } = new List<Order>(); // for all users
    public IEnumerable<Service> Services { get; set; } = new List<Service>(); // for center or freelancer

    public IEnumerable<EvaluationProvider> EvaluationProviders { get; set; } = new List<EvaluationProvider>(); // for center or freelancer
    public IEnumerable<FavoriteProvider> FavoriteProviders { get; set; } = new List<FavoriteProvider>(); // for center or freelancer
    public IEnumerable<ProviderPhoto> ProviderPhotos { get; set; } = new List<ProviderPhoto>(); // for center or freelancer
    public IEnumerable<Commission> Commissions { get; set; } = new List<Commission>(); // for center or freelancer



    //------------------------------------------------------------------------------------------------
    [NotMapped]
    public string FreelanceForm { get; set; }

    [NotMapped]
    public IFormFile FreelanceFormImgFile { get; set; }


    [NotMapped]
    public IFormFile UserImgFile { get; set; }

   
}