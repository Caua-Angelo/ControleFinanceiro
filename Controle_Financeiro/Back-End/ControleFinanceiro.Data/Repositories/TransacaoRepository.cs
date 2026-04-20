using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Infra.Data.Repositories
{
    public class TransacaoRepository : ITransacaoRepository
    {
        private readonly ApplicationDBContext _context;

        public TransacaoRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transacao>> ListAsync()
        {
            return await _context.Set<Transacao>()
                .Include(t => t.Usuario)
                .Include(t => t.Categoria)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Transacao?> GetByIdAsync(int id)
        {
            return await _context.Set<Transacao>()
                .Include(t => t.Usuario)
                .Include(t => t.Categoria)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transacao>> ListByUserAsync(int usuarioId)
        {
            return await _context.Set<Transacao>()
                .Include(t => t.Categoria)
                .Where(t => t.UsuarioId == usuarioId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> ListByCategoryAsync(int categoriaId)
        {
            return await _context.Set<Transacao>()
                .Include(t => t.Usuario)
                .Where(t => t.CategoriaId == categoriaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Transacao transacao)
        {
            if (_context.Entry(transacao.Usuario).State == EntityState.Detached)
                _context.Entry(transacao.Usuario).State = EntityState.Unchanged;

            if (_context.Entry(transacao.Categoria).State == EntityState.Detached)
                _context.Entry(transacao.Categoria).State = EntityState.Unchanged;
            await _context.Set<Transacao>().AddAsync(transacao);
        }

        public Task UpdateAsync(Transacao transacao)
        {
            _context.Set<Transacao>().Update(transacao);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Transacao transacao)
        {
            _context.Set<Transacao>().Remove(transacao);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
