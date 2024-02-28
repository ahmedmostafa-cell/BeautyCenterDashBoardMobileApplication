using System.Text.Json.Serialization;
using JamalKhanah.Core;
using JamalKhanah.RepositoryLayer.Interfaces;
using JamalKhanah.RepositoryLayer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JamalKhanah.Extensions;

public static class ContextServicesExtensions
{
    public static IServiceCollection AddContextServices(this IServiceCollection services, IConfiguration config)
    {

        //- context && json services
        services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(config.GetConnectionString("url")));//,b => b.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName)).UseLazyLoadingProxies());
        services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

        // IBaseRepository && IUnitOfWork Service
        //services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>)); // only Repository
        services.AddTransient<IUnitOfWork, UnitOfWork>(); // Repository and UnitOfWork
            
        return services;
    }
       
}
