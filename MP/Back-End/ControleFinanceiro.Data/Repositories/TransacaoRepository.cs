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

        public async Task<IEnumerable<Transacao>> ListarAsync()
        {
            return await _context.Set<Transacao>()
                .Include(t => t.Usuario)
                .Include(t => t.Categoria)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Transacao?> ObterPorIdAsync(int id)
        {
            return await _context.Set<Transacao>()
                .Include(t => t.Usuario)
                .Include(t => t.Categoria)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transacao>> ListarPorUsuarioAsync(int usuarioId)
        {
            return await _context.Set<Transacao>()
                .Include(t => t.Categoria)
                .Where(t => t.UsuarioId == usuarioId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> ListarPorCategoriaAsync(int categoriaId)
        {
            return await _context.Set<Transacao>()
                .Include(t => t.Usuario)
                .Where(t => t.CategoriaId == categoriaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AdicionarAsync(Transacao transacao)
        {
            await _context.Set<Transacao>().AddAsync(transacao);
        }

        public Task AtualizarAsync(Transacao transacao)
        {
            _context.Set<Transacao>().Update(transacao);
            return Task.CompletedTask;
        }

        public Task RemoverAsync(Transacao transacao)
        {
            _context.Set<Transacao>().Remove(transacao);
            return Task.CompletedTask;
        }

        public async Task SalvarAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
