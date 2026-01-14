using ControleFerias.API; // 🔥 importantíssimo
using ControleFerias.Infraestructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ControleFerias.testes.Comum
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove o contexto real, se existir
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDBContext>)
                );
                if (descriptor != null)
                    services.Remove(descriptor);

                // Usa banco em memória
                services.AddDbContext<ApplicationDBContext>(options =>
                    options.UseInMemoryDatabase("DbTest_" + Guid.NewGuid()));

                // Cria escopo e popula dados iniciais
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                context.Database.EnsureCreated();

                context.Equipe.Add(new Domain.Models.Equipe { Id = 2, sNome = "Equipe Teste" });
                context.SaveChanges();
            });
        }
    }
}