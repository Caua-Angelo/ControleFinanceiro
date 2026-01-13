using ControleFerias.Application.Interfaces;
using ControleFerias.Application.Services;
using ControleFerias.AutoMapper;
using ControleFerias.Domain.Interfaces;
using ControleFerias.Infra.Data.Repositories;
using ControleFerias.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ControleFerias.Infra.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
           IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDBContext>(options => options.UseNpgsql(configuration.GetConnectionString(
                "DefaultConnection"
            ), b => b.MigrationsAssembly(typeof(ApplicationDBContext).Assembly.FullName)));

            services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
            services.AddScoped<IEquipeRepository, EquipeRepository>();
            services.AddScoped<IFeriasRepository, FeriasRepository>();

            services.AddScoped<IColaboradorService, ColaboradorService>();
            services.AddScoped<IEquipeService, EquipeService>();
            services.AddScoped<IFeriasService, FeriasService>();
            services.AddAutoMapper(typeof(MappingProfile));

            var myhandlers = AppDomain.CurrentDomain.Load("ControleFerias.Application");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(myhandlers));
            return services;
        }



    }
}
