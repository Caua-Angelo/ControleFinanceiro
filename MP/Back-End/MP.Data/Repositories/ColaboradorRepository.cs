using ControleFerias.Domain.Interfaces;
using ControleFerias.Domain.Models;
using ControleFerias.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ControleFerias.Infra.Data.Repositories
{
    public class ColaboradorRepository : IColaboradorRepository
    {
        private readonly ApplicationDBContext _context;

        public ColaboradorRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Colaborador>> ConsultarColaboradores()
        {
            return await _context.Colaborador
                .Include(c => c.Equipe)
                .Include(c => c.Ferias)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Colaborador> ConsultarColaboradorPorId(int id)
        {
            return await _context.Colaborador
                .Include(c => c.Equipe)
                .Include(c => c.Ferias)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Colaborador> IncluirColaborador(Colaborador colaborador)
        {
            _context.Colaborador.Add(colaborador);

            await _context.SaveChangesAsync();
            return colaborador;
        }
        public async Task<Colaborador> ConsultarParaAlteracao(int id)
        {
            return await _context.Colaborador.FindAsync(id);
        }

        public async Task SalvarMudancas()
        {
            // salva todas as alterações que foram feitas nas entidades rastreadas
            await _context.SaveChangesAsync();
        }

        public async Task<Colaborador> ExcluirColaborador(int id)
        {
            var colaborador = await _context.Colaborador.FindAsync(id);

            _context.Colaborador.Remove(colaborador);
            await _context.SaveChangesAsync();
            return colaborador;
        }
       
    }
}
