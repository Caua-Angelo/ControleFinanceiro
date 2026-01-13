using AutoMapper;
using ControleFerias.Application.DTO;
using ControleFerias.Application.Interfaces;
using ControleFerias.Domain.Enums;
using ControleFerias.Domain.Interfaces;
using ControleFerias.Domain.Models;

namespace ControleFerias.Application.Services
{
    public class FeriasService : IFeriasService
    {
        private readonly IFeriasRepository _feriasRepository;
        private readonly IColaboradorRepository _colaboradorRepository;
        private readonly IEquipeRepository _equipeRepository;

        private readonly IMapper _mapper;

        public FeriasService(IFeriasRepository feriasRepository, IColaboradorRepository colaboradorRepository, IEquipeRepository equipeRepository, IMapper mapper)
        {
            _feriasRepository = feriasRepository;
            _colaboradorRepository = colaboradorRepository;
            _equipeRepository = equipeRepository;

            _mapper = mapper;
        }

        public async Task<IEnumerable<FeriasConsultarDTO>> ConsultarFerias()
        {
            var ferias = await _feriasRepository.ConsultarFerias();
            return _mapper.Map<IEnumerable<FeriasConsultarDTO>>(ferias);
        }

        public async Task<FeriasConsultarDTO> ConsultarFeriasPorId(int id)
        {
            var ferias = await _feriasRepository.ConsultarFeriasPorId(id);
            if (ferias == null)
                throw new KeyNotFoundException($"Registro de férias com ID {id} não encontrado.");

            return _mapper.Map<FeriasConsultarDTO>(ferias);
        }
        public async Task<IEnumerable<FeriasConsultarDTO>> ConsultarFeriasPorColaborador(int id)
        {
            var colaborador = await _colaboradorRepository.ConsultarColaboradorPorId(id);
            if (colaborador == null)
                throw new KeyNotFoundException($"Colaborador com ID {id} não encontrado.");

            var ferias = await _feriasRepository.ConsultarFeriasPorColaborador(id);
            if (ferias == null || !ferias.Any())
                throw new KeyNotFoundException($"Nenhum registro de férias encontrado para o colaborador com ID {id}.");

            return _mapper.Map<IEnumerable<FeriasConsultarDTO>>(ferias);
        }
        public async Task<IEnumerable<FeriasConsultarDTO>> ConsultarFeriasPorEquipe(int id)
        {
            var equipe = await _equipeRepository.ConsultarEquipePorId(id);
            if (equipe == null)
                throw new KeyNotFoundException($"Equipe com ID {id} não encontrada.");

            var ferias = await _feriasRepository.ConsultarFeriasPorEquipe(id);
            if (!ferias.Any())
                return Enumerable.Empty<FeriasConsultarDTO>();

            return _mapper.Map<IEnumerable<FeriasConsultarDTO>>(ferias);
        }

        public async Task<FeriasConsultarDTO> IncluirFerias(FeriasIncluirDTO dto)
        {
            var colaborador = await _colaboradorRepository.ConsultarColaboradorPorId(dto.ColaboradorId);
            if (colaborador == null)
                throw new InvalidOperationException($"Colaborador com ID {dto.ColaboradorId} não encontrado.");

            var ferias = new Ferias(dto.dDataInicio, dto.sDias, dto.dDataFinal, dto.sComentario, dto.ColaboradorId);

            var colaboradorFerias = new ColaboradorFerias
            {
                ColaboradorId = dto.ColaboradorId,
                Ferias = ferias
            };
            ferias.ColaboradorFerias = new List<ColaboradorFerias> { colaboradorFerias };

            await _feriasRepository.IncluirFerias(ferias);

            var feriasComRelacionamentos = await _feriasRepository.ConsultarFeriasPorId(ferias.Id);
            var result = _mapper.Map<FeriasConsultarDTO>(feriasComRelacionamentos);

            if (result.ColaboradorId == 0)
                result.ColaboradorId = feriasComRelacionamentos.ColaboradorFerias?.FirstOrDefault()?.ColaboradorId ?? 0;

            return result;
        }

        public async Task<FeriasConsultarDTO> AlterarFerias(int id, FeriasAlterarDTO dto)
        {
            var feriasExistente = await _feriasRepository.ConsultarFeriasPorId(id);

            if (feriasExistente == null)
                throw new KeyNotFoundException($"Registro de férias com ID {id} não encontrado.");

            if (feriasExistente.Status != StatusFerias.Pendente)
                throw new InvalidOperationException("Só é permitido alterar férias com status Pendente.");

            feriasExistente.UpdateAlterar(dto.dDataInicio, dto.dDataFinal, dto.sDias, dto.status, dto.sComentario);

            await _feriasRepository.SalvarMudancas();

            return _mapper.Map<FeriasConsultarDTO>(feriasExistente);
        }

        public async Task<FeriasConsultarDTO> ExcluirFerias(int id)
        {
            var feriasExcluida = await _feriasRepository.ExcluirFerias(id);

            if (feriasExcluida == null)
            {
                throw new KeyNotFoundException($"Registro de férias com ID {id} não encontrado para exclusão.");
            }

            await _feriasRepository.SalvarMudancas();

            return _mapper.Map<FeriasConsultarDTO>(feriasExcluida);
        }
    }
}