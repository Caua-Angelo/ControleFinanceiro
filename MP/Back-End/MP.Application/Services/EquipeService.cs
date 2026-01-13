using AutoMapper;
using ControleFerias.Application.DTO;
using ControleFerias.Application.Interfaces;
using ControleFerias.Domain.Interfaces;
using ControleFerias.Domain.Models;


namespace ControleFerias.Application.Services
{
    public class EquipeService : IEquipeService
    {
        private readonly IEquipeRepository _equipeRepository;
        private readonly IMapper _mapper;

        public EquipeService(IEquipeRepository equipeRepository, IMapper mapper)
        {
            _equipeRepository = equipeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EquipeConsultarDTO>> ConsultarEquipes()
        {
            var equipes = await _equipeRepository.ConsultarEquipes();

             var equipeConsultarDTO = _mapper.Map<IEnumerable<EquipeConsultarDTO>>(equipes);
            return equipeConsultarDTO;
        }

        public async Task<EquipeConsultarDTO> ConsultarEquipePorId(int id)
        {
            var equipe = await _equipeRepository.ConsultarEquipePorId(id);
            if (equipe == null)
                throw new KeyNotFoundException($"Equipe com ID {id} não encontrada.");

            var equipeConsultarDTO = _mapper.Map<EquipeConsultarDTO>(equipe);
            return equipeConsultarDTO;
        }
        public async Task<IEnumerable<ColaboradorConsultarDTO>> ConsultarColaboradoresPorEquipe(int id)
        {
            var colaboradores = await _equipeRepository.ConsultarColaboradoresPorEquipe(id);
            if (colaboradores == null)
                throw new KeyNotFoundException($"Equipe com ID {id} não encontrada.");

            if (!colaboradores.Any())
                throw new KeyNotFoundException($"Nenhum colaborador encontrado para a equipe com ID {id}.");


            var colaboradorConsultarDTO = _mapper.Map<IEnumerable<ColaboradorConsultarDTO>>(colaboradores);
            return colaboradorConsultarDTO;
        }

        public async Task<EquipeConsultarDTO> IncluirEquipe(EquipeIncluirDTO dto)
        {
            var equipe = new Equipe(dto.sNome);
            await _equipeRepository.IncluirEquipe(equipe);
            await _equipeRepository.SalvarMudancas();
            return _mapper.Map<EquipeConsultarDTO>(equipe);
        }

        public async Task<EquipeConsultarDTO> AlterarEquipe(int id, EquipeAlterarDTO dto)
        {
            var equipeExistente = await _equipeRepository.ConsultarEquipePorId(id);
            if (equipeExistente == null)
                throw new KeyNotFoundException($"Equipe com ID {id} não encontrada.");

            equipeExistente.Update(dto.sNome);
            await _equipeRepository.SalvarMudancas();

            return _mapper.Map<EquipeConsultarDTO>(equipeExistente);
        }

        public async Task<EquipeConsultarDTO> ExcluirEquipe(int id)
        {
            var equipeExistente = await _equipeRepository.ExcluirEquipe(id);
            if (equipeExistente == null)
                throw new KeyNotFoundException($"Equipe com ID {id} não encontrada.");
            await _equipeRepository.SalvarMudancas();
            return _mapper.Map<EquipeConsultarDTO>(equipeExistente);
        }
    }
}
