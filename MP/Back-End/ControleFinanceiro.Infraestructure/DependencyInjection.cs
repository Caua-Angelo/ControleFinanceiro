using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Application.Services;
using ControleFinanceiro.AutoMapper;
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Infra.Data.Repositories;
using ControleFinanceiro.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ControleFinanceiro.Infra.IoC
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
            services.AddScoped<IEquipeRepository, CategoriaRepository>();
            services.AddScoped<IFeriasRepository, TransacaoRepository>();

            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<ITransacaoService, TransacaoService>();
            services.AddAutoMapper(typeof(MappingProfile));

            var myhandlers = AppDomain.CurrentDomain.Load("ControleFinanceiro.Application");
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(myhandlers));
            return services;
        }



    }
}
