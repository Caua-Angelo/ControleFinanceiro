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

        public async Task<IEnumerable<Categoria>> ListAsync(int usuarioId)
        {
            return await _context.Set<Categoria>()
                .AsNoTracking()
                .Where(c => c.UsuarioId == null || c.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<Categoria?> GetByIdAsync(int id, int usuarioId)
        {
            return await _context.Set<Categoria>()
                .Include(c => c.Transacoes)
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    (c.UsuarioId == null || c.UsuarioId == usuarioId));
        }

        public async Task AddAsync(Categoria categoria)
        {
            await _context.Set<Categoria>().AddAsync(categoria);
        }

        public Task DeleteAsync(Categoria categoria)
        {
            _context.Set<Categoria>().Remove(categoria);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
