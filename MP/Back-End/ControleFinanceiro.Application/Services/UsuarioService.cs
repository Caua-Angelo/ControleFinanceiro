using AutoMapper;
using ControleFinanceiro.Application.DTO;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Application.Services
{
    public class ColaboradorService : IUsuarioService
    {
        private readonly IColaboradorRepository _colaboradorRepository;
        private readonly IEquipeRepository _equipeRepository;
        private readonly IMapper _mapper;

        public ColaboradorService(IColaboradorRepository colaboradorRepository, IMapper mapper, IEquipeRepository equipeRepository)
        {
            _colaboradorRepository = colaboradorRepository;
            _mapper = mapper;
            _equipeRepository = equipeRepository;
        }

        public async Task<IEnumerable<ColaboradorConsultarDTO>> ConsultarColaboradores()
        {
            var colaboradores = await _colaboradorRepository.ConsultarColaboradores();
            return _mapper.Map<IEnumerable<ColaboradorConsultarDTO>>(colaboradores);
        }

        public async Task<ColaboradorConsultarDTO> ConsultarColaboradorPorId(int id)
        {
            var Usuario = await _colaboradorRepository.ConsultarColaboradorPorId(id);
            if (Usuario == null)
                throw new KeyNotFoundException($"Usuario com ID {id} não encontrado.");

            return _mapper.Map<ColaboradorConsultarDTO>(Usuario);
        }

        public async Task<ColaboradorConsultarDTO> IncluirColaborador(ColaboradorIncluirDTO dto)
        {
            // Chama o Repositório de Equipe para verificar se o ID existe
            var equipeExiste = await _equipeRepository.ConsultarEquipePorId(dto.EquipeId);

            if (equipeExiste == null) { throw new KeyNotFoundException($"Nenhuma equipe encontrada com o ID {dto.EquipeId}."); }

            var Usuario = new Usuario(dto.sNome, dto.EquipeId);

            await _colaboradorRepository.IncluirColaborador(Usuario);

            return _mapper.Map<ColaboradorConsultarDTO>(Usuario);
        }

        public async Task<ColaboradorConsultarDTO> AlterarColaborador(int id, ColaboradorAlterarDTO dto)
        {
          
                var colaboradorExistente = await _colaboradorRepository.ConsultarColaboradorPorId(id);
                if (colaboradorExistente == null)
                    throw new KeyNotFoundException($"Usuario com ID {id} não encontrado.");
                var equipeExiste = await _equipeRepository.ConsultarEquipePorId(dto.EquipeId);
                if (equipeExiste == null)
                    throw new KeyNotFoundException($"Nenhuma equipe encontrada com o ID {dto.EquipeId}.");

                colaboradorExistente.Update(dto.sNome, dto.EquipeId);

                await _colaboradorRepository.SalvarMudancas();
                return _mapper.Map<ColaboradorConsultarDTO>(colaboradorExistente);
        }

        public async Task<ColaboradorConsultarDTO> ExcluirColaborador(int id)
        {
            var Usuario = await _colaboradorRepository.ConsultarColaboradorPorId(id);
            if (Usuario == null)
            {
                throw new Exception($"O Usuario com ID {id} não foi encontrado.");
            }
            var colaboradorExcluido = await _colaboradorRepository.ExcluirColaborador(id);

            return _mapper.Map<ColaboradorConsultarDTO>(colaboradorExcluido);
        }
    }
}
