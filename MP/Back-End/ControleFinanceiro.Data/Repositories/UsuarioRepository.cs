using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Domain.Models;
using ControleFinanceiro.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Infra.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDBContext _context;

        public UsuarioRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> ListarAsync()
        {
            return await _context.Usuario
                .Include(u => u.Transacao)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Usuario?> ObterPorIdAsync(int id)
        {
            return await _context.Usuario
                .Include(u => u.Transacao)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AdicionarAsync(Usuario usuario)
        {
            await _context.Usuario.AddAsync(usuario);
        }

        public Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuario.Update(usuario);
            return Task.CompletedTask;
        }

        public Task RemoverAsync(Usuario usuario)
        {
            _context.Usuario.Remove(usuario);
            return Task.CompletedTask;
        }

        public async Task SalvarAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
