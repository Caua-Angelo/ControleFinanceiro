using ControleFerias.Domain.Interfaces;
using ControleFerias.Domain.Models;
using ControleFerias.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ControleFerias.Infra.Data.Repositories
{
    public class EquipeRepository : IEquipeRepository
    {
        private readonly ApplicationDBContext _context;

        public EquipeRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Equipe>> ConsultarEquipes()
        {
            return await _context.Equipe.ToListAsync();
        }
        public async Task<Equipe> ConsultarEquipePorId(int id)
        {
            return await _context.Equipe.FindAsync(id);
        }
        public async Task<IEnumerable<Colaborador>> ConsultarColaboradoresPorEquipe(int equipeId)
        {
            var equipe = await _context.Equipe
                .Include(e => e.Colaboradores)
                .FirstOrDefaultAsync(e => e.Id == equipeId);

            return equipe?.Colaboradores ?? new List<Colaborador>();
        }

        public async Task<Equipe> IncluirEquipe(Equipe equipe)
        {
            _context.Equipe.Add(equipe);
            await _context.SaveChangesAsync();
            return equipe;
        }
        public async Task SalvarMudancas()
        {
            // salva todas as alterações que foram feitas nas entidades rastreadas
            await _context.SaveChangesAsync();
        }
        public async Task<Equipe> ConsultarExistente(int id)
        {
            return await _context.Equipe.FindAsync(id);
        }

        public async Task<Equipe> ExcluirEquipe(int id)
        {
            var equipe = await _context.Equipe.FindAsync(id);

            _context.Equipe.Remove(equipe);
            await _context.SaveChangesAsync();
            return equipe;
        }
    }
}
