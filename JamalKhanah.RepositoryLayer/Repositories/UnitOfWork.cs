using JamalKhanah.Core;
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
using JamalKhanah.RepositoryLayer.Interfaces;

namespace JamalKhanah.RepositoryLayer.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _context;

    public IBaseRepository<ApplicationUser> Users { get; private set; }
    public IBaseRepository<ApplicationRole> Roles { get; private set; }
    public IBaseRepository<Complaint> Complaints { get; private set; }

    public IBaseRepository<ContactUs> ContactUs { get; private set; }
    public IBaseRepository<SlidePhoto> SlidePhotos { get; private set; }

    public IBaseRepository<MessageChat> MessageChats { get; private set; }

    public IBaseRepository<Notification> Notifications { get; private set; }
    public IBaseRepository<NotificationConfirmed> NotificationsConfirmed { get; }

    public IBaseRepository<UserConnection> UserConnections { get; private set; }

    public IBaseRepository<Prize> Prizes { get; private set; }
    public IBaseRepository<Experience> Experiences { get; private set; }
    public IBaseRepository<WorkHours> WorksHours { get; private set; }
    public IBaseRepository<Address> Addresses { get; private set; }
    public IBaseRepository<Employee> Employees { get; private set; }

    public IBaseRepository<MainSection> MainSections { get; private set; }
    public IBaseRepository<Service> Services { get; private set; }
    public IBaseRepository<Order> Orders { get; private set; }
    public IBaseRepository<City> Cities { get; private set; }
    public IBaseRepository<EvaluationProvider> EvaluationProviders { get; private set; }
    public IBaseRepository<EvaluationService> EvaluationServices { get; private set; }
    public IBaseRepository<FavoriteProvider> FavoriteProviders { get; private set; }
    public IBaseRepository<FavoriteService> FavoriteServices { get; private set; }
    public IBaseRepository<ProviderPhoto> ProviderPhotos { get; private set; }
    public IBaseRepository<PaymentHistory> PaymentHistories { get; private set; }
    public IBaseRepository<Commission> Commissions { get; private set; }
    public IBaseRepository<Coupon> Coupons { get; private set; }
    public IBaseRepository<ApplicationUserCoupon> UserCoupons { get; private set; }
    public IBaseRepository<ServiceCoupon> ServiceCoupons { get; private set; }
    public IBaseRepository<MainSectionCoupon> MainSectionCoupons { get; private set; }


    public IBaseRepository<QuestionsAndAnswersSection> QuestionsAndAnswersSections { get; private set; }
    public IBaseRepository<QuestionsAndAnswers> QuestionsAndAnswers { get; private set; }





    public UnitOfWork(ApplicationContext context)
    {
        _context = context;
        NotificationsConfirmed = new BaseRepository<NotificationConfirmed>(_context);
        MessageChats = new BaseRepository<MessageChat>(_context);
        Notifications = new BaseRepository<Notification>(_context);
        UserConnections = new BaseRepository<UserConnection>(_context);
        Users = new BaseRepository<ApplicationUser>(_context);
        Roles = new BaseRepository<ApplicationRole>(_context);
        Complaints = new BaseRepository<Complaint>(_context);
        ContactUs = new BaseRepository<ContactUs>(_context);
        SlidePhotos = new BaseRepository<SlidePhoto>(_context);
        Prizes = new BaseRepository<Prize>(_context);
        Experiences = new BaseRepository<Experience>(_context);
        WorksHours = new BaseRepository<WorkHours>(_context);
        Addresses = new BaseRepository<Address>(_context);
        Employees = new BaseRepository<Employee>(_context);
        MainSections = new BaseRepository<MainSection>(_context);
        Services = new BaseRepository<Service>(_context);
        Orders = new BaseRepository<Order>(_context);
        Cities = new BaseRepository<City>(_context);
        QuestionsAndAnswersSections = new BaseRepository<QuestionsAndAnswersSection>(_context);
        QuestionsAndAnswers = new BaseRepository<QuestionsAndAnswers>(_context);
        EvaluationProviders = new BaseRepository<EvaluationProvider>(_context);
        EvaluationServices = new BaseRepository<EvaluationService>(_context);
        FavoriteProviders = new BaseRepository<FavoriteProvider>(_context);
        FavoriteServices = new BaseRepository<FavoriteService>(_context);
        ProviderPhotos = new BaseRepository<ProviderPhoto>(_context);
        PaymentHistories = new BaseRepository<PaymentHistory>(_context);
        Commissions = new BaseRepository<Commission>(_context);
        Coupons = new BaseRepository<Coupon>(_context);
        UserCoupons = new BaseRepository<ApplicationUserCoupon>(_context);
        ServiceCoupons = new BaseRepository<ServiceCoupon>(_context);
        MainSectionCoupons = new BaseRepository<MainSectionCoupon>(_context);

    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}