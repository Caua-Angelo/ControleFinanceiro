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

        public async Task<IEnumerable<Usuario>> ListAsync()
        {
            return await _context.Usuario
                .Include(u => u.Transacao)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuario
                .Include(u => u.Transacao) 
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddAsync(Usuario usuario)
        {
            await _context.Usuario.AddAsync(usuario);
        }

        public Task UpdateAsync(Usuario usuario)
        {
            _context.Usuario.Update(usuario);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Usuario usuario)
        {
            _context.Usuario.Remove(usuario);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuario
                .Include(u => u.Transacao)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
