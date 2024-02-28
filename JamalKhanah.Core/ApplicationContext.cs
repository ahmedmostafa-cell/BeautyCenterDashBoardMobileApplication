
using JamalKhanah.Core.DTO.EntityDto;
using JamalKhanah.Core.Entity.ApplicationData;
using JamalKhanah.Core.Entity.ChatAndNotification;
using JamalKhanah.Core.Entity.CommissionsData;
using JamalKhanah.Core.Entity.CouponData;
using JamalKhanah.Core.Entity.EvaluationData;
using JamalKhanah.Core.Entity.FavoriteData;
using JamalKhanah.Core.Entity.OrderData;
using JamalKhanah.Core.Entity.Other;
using JamalKhanah.Core.Entity.PaymentData;
using JamalKhanah.Core.Entity.ProfileData;
using JamalKhanah.Core.Entity.QuestionsAndAnswersData;
using JamalKhanah.Core.Entity.SectionsData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Order = JamalKhanah.Core.Entity.OrderData.Order;

namespace JamalKhanah.Core;

public class ApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    
    

    //-----------------------------------------------------------------------------------
    public virtual DbSet<MessageChat> MessageChats { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<NotificationConfirmed> NotificationsConfirmed { get; set; }
    public virtual DbSet<UserConnection> UserConnections { get; set; }

    //-----------------------------------------------------------------------------------
    public virtual DbSet<Complaint> Complaints { get; set; }
    public virtual DbSet<ContactUs> ContactUs { get; set; }
    public virtual DbSet<SlidePhoto> SlidePhotos { get; set; }
    //-----------------------------------------------------------------------------------
    public virtual DbSet<Prize> Prizes { get; set; }
    public virtual DbSet<Experience> Experiences { get; set; }
    public virtual DbSet<WorkHours> WorksHours { get; set; }
    public virtual DbSet<Address> Addresses { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }

    //-----------------------------------------------------------------------------------
    public virtual DbSet<MainSection> MainSections { get; set; }
    public virtual DbSet<Service> Services { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<City> Cities { get; set; }
    public virtual DbSet<QuestionsAndAnswersSection> QuestionsAndAnswersSections { get; set; }
    public virtual DbSet<QuestionsAndAnswers> QuestionsAndAnswers { get; set; }

    //-----------------------------------------------------------------------------------
    public virtual DbSet<FavoriteProvider> FavoriteProviders { get; set; }
    public virtual DbSet<FavoriteService> FavoriteServices { get; set; }
    public virtual DbSet<EvaluationProvider> EvaluationProviders { get; set; }
    public virtual DbSet<EvaluationService> EvaluationServices { get; set; }

    public virtual DbSet<ProviderPhoto> ProviderPhotos { get; set; }

    //-----------------------------------------------------------------------------------
    public virtual DbSet<PaymentHistory> PaymentHistories { get; set; }

    public virtual DbSet<Commission> Commissions { get; set; }

    //-----------------------------------------------------------------------------------

    public virtual DbSet<Coupon> Coupons { get; set; }
    public virtual DbSet<ApplicationUserCoupon> UserCoupons { get; set; }
    public virtual DbSet<ServiceCoupon> ServiceCoupons { get; set; }
    public virtual DbSet<MainSectionCoupon> MainSectionCoupons { get; set; }

    //-----------------------------------------------------------------------------------






    //-----------------------------------------------------------------------------------
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }
    public ApplicationContext() 
    {
    }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Server=MAHMOUD-SABRY-P;Database=JamalKhanat;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApplicationUser>().ToTable("Users", "dbo");
        modelBuilder.Entity<ApplicationRole>().ToTable("Role", "dbo");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRole", "dbo");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim", "dbo");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin", "dbo");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "dbo");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "dbo");

        modelBuilder.Entity<EvaluationProvider>()
            .HasOne(p => p.Provider)
            .WithMany(b => b.EvaluationProviders);

        modelBuilder.Entity<FavoriteProvider>()
            .HasOne(p => p.Provider)
            .WithMany(b => b.FavoriteProviders);

       
    }
}