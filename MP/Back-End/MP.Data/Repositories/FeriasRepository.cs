using ControleFerias.Domain.Enums;
using ControleFerias.Domain.Interfaces;
using ControleFerias.Domain.Models;
using ControleFerias.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ControleFerias.Infra.Data.Repositories
{
    public class FeriasRepository : IFeriasRepository
    {
        private readonly ApplicationDBContext _context;

        public FeriasRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ferias>> ConsultarFerias()
        {
            return await _context.Ferias
            .Include(f => f.ColaboradorFerias)
                .ThenInclude(cf => cf.Colaborador)
            .ToListAsync();
        }
        public async Task<Ferias> ConsultarFeriasPorId(int id)
        {
            return await _context.Ferias
        .Include(f => f.ColaboradorFerias)
            .ThenInclude(cf => cf.Colaborador)
        .FirstOrDefaultAsync(f => f.Id == id);
        }
        public async Task<IEnumerable<Ferias>> ConsultarFeriasPorColaborador(int id)
        {
            return await _context.Ferias
        .Include(f => f.ColaboradorFerias)
            .ThenInclude(cf => cf.Colaborador)
        .Where(f => f.ColaboradorFerias.Any(cf => cf.ColaboradorId == id))
        .ToListAsync();
        }
        public async Task<IEnumerable<Ferias>> ConsultarFeriasPorEquipe(int id)
        {
            return await _context.Ferias
        .Include(f => f.ColaboradorFerias)
            .ThenInclude(cf => cf.Colaborador)
        .Where(f => f.ColaboradorFerias.Any(cf => cf.Colaborador.EquipeId == id)
                    && f.Status == StatusFerias.Aprovado)
        .ToListAsync();

        }
        public async Task<Ferias> IncluirFerias(Ferias ferias)
        {

            _context.Ferias.Add(ferias);
            await _context.SaveChangesAsync();
            return ferias;
        }
        //public async Task<Ferias> AlterarFerias(Ferias ferias)
        //{
        //    _context.Ferias.Update(ferias);
        //    await _context.SaveChangesAsync();
        //    return ferias;
        //}
        public async Task<Ferias> ConsultarParaAlteracao(int id)
        {
            return await _context.Ferias.FindAsync(id);
        }

        public async Task SalvarMudancas()
        {
            // salva todas as alterações que foram feitas nas entidades rastreadas
            await _context.SaveChangesAsync();
        }

        public async Task<Ferias> ExcluirFerias(int id)
        {
            var ferias = await _context.Ferias
                .Include(f => f.ColaboradorFerias)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (ferias == null) return null;

            _context.Ferias.Remove(ferias);
            await _context.SaveChangesAsync();
            return ferias;
        }

    }
}
