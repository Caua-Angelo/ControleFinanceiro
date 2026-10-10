
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Infra.Data.Repositories
{
    public class CartaoRepository : ICartaoRepository
    {
        private readonly ApplicationDBContext _context;

        public CartaoRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cartao>> ListAsync(int usuarioId)
        {
            return await _context.Set<Cartao>()
                .AsNoTracking()
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<Cartao?> GetByIdAsync(int id, int usuarioId)
        {
            return await _context.Set<Cartao>()
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.UsuarioId == usuarioId);
        }

        public async Task AddAsync(Cartao cartao)
        {
            await _context.Set<Cartao>().AddAsync(cartao);
        }

        public Task DeleteAsync(Cartao cartao)
        {
            _context.Set<Cartao>().Remove(cartao);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
