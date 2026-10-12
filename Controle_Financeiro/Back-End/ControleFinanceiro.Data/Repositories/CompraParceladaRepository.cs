
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Infra.Data.Repositories
{
    public class CompraParceladaRepository : ICompraParceladaRepository
    {
        private readonly ApplicationDBContext _context;

        public CompraParceladaRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<CompraParcelada?> GetByIdAsync(int id)
        {
            return await _context.Set<CompraParcelada>()
                .Include(c => c.Parcelas)
                .Include(c => c.Cartao)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CompraParcelada>> ListByUserAsync(int usuarioId)
        {
            return await _context.Set<CompraParcelada>()
                .Include(c => c.Parcelas)
                .Include(c => c.Cartao)
                .Where(c => c.UsuarioId == usuarioId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Parcela?> GetParcelaByIdAsync(int parcelaId)
        {
            return await _context.Set<Parcela>()
                .Include(p => p.CompraParcelada)
                .FirstOrDefaultAsync(p => p.Id == parcelaId);
        }


        public async Task AddAsync(CompraParcelada compra)
        {
            if (_context.Entry(compra.Usuario).State == EntityState.Detached)
                _context.Entry(compra.Usuario).State = EntityState.Unchanged;

            if (_context.Entry(compra.Cartao).State == EntityState.Detached)
                _context.Entry(compra.Cartao).State = EntityState.Unchanged;

            await _context.Set<CompraParcelada>().AddAsync(compra);
        }

        public Task DeleteAsync(CompraParcelada compra)
        {
            _context.Set<CompraParcelada>().Remove(compra);
            return Task.CompletedTask;
        }
       public Task RemoverParcelasAsync(IEnumerable<Parcela> parcelas)
        {
            _context.Set<Parcela>().RemoveRange(parcelas);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
