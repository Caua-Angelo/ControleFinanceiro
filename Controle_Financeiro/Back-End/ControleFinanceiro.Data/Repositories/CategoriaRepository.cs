using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Infra.Data.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDBContext _context;

        public CategoriaRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> ConsultarAsync()
        {
            return await _context.Set<Categoria>()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Categoria?> ConsultarPorIdAsync(int id)
        {
            return await _context.Set<Categoria>()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AdicionarAsync(Categoria categoria)
        {
            await _context.Set<Categoria>().AddAsync(categoria);
        }

        public Task RemoverAsync(Categoria categoria)
        {
            _context.Set<Categoria>().Remove(categoria);
            return Task.CompletedTask;
        }

        public async Task SalvarAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
