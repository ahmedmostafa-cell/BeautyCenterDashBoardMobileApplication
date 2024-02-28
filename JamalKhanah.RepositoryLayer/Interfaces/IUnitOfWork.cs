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
using Microsoft.EntityFrameworkCore;

namespace JamalKhanah.RepositoryLayer.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBaseRepository<ApplicationUser> Users { get; }
    IBaseRepository<ApplicationRole> Roles { get; }

    //-----------------------------------------------------------------------------------
    IBaseRepository<Complaint> Complaints { get; }
    IBaseRepository<ContactUs> ContactUs { get; }
    IBaseRepository<SlidePhoto> SlidePhotos { get; }
    
    //-----------------------------------------------------------------------------------
    IBaseRepository<MessageChat> MessageChats { get; }


    IBaseRepository<Notification> Notifications { get; }
    IBaseRepository<NotificationConfirmed> NotificationsConfirmed { get; }
    IBaseRepository<UserConnection> UserConnections { get; }

    //-----------------------------------------------------------------------------------
    IBaseRepository<Prize> Prizes { get; }
    IBaseRepository<Experience> Experiences { get; }
    IBaseRepository<WorkHours> WorksHours { get; }
    IBaseRepository<Address> Addresses { get; }
    IBaseRepository<Employee> Employees { get; }

    //-----------------------------------------------------------------------------------

    IBaseRepository<MainSection> MainSections { get; }
    IBaseRepository<Service> Services { get; }
    IBaseRepository<Order> Orders { get; }
    IBaseRepository<City> Cities { get; }

    //-----------------------------------------------------------------------------------
    IBaseRepository<QuestionsAndAnswersSection> QuestionsAndAnswersSections { get; }
    IBaseRepository<QuestionsAndAnswers> QuestionsAndAnswers { get; }

    //-----------------------------------------------------------------------------------
    IBaseRepository<EvaluationProvider> EvaluationProviders { get; }
    IBaseRepository<EvaluationService> EvaluationServices { get; }
    IBaseRepository<FavoriteProvider> FavoriteProviders { get; }
    IBaseRepository<FavoriteService> FavoriteServices { get; }
    IBaseRepository<ProviderPhoto> ProviderPhotos { get; }
    //-----------------------------------------------------------------------------------
    IBaseRepository<PaymentHistory> PaymentHistories { get; }

    IBaseRepository<Commission> Commissions { get; }

    //-------------------------------------------------------------------------------

    IBaseRepository<Coupon> Coupons { get;  }
    IBaseRepository<ApplicationUserCoupon> UserCoupons { get;  }
    IBaseRepository<ServiceCoupon> ServiceCoupons { get;  }
    IBaseRepository<MainSectionCoupon> MainSectionCoupons { get;  }



    //-----------------------------------------------------------------------------------
    int SaveChanges();

    Task<int> SaveChangesAsync();
}