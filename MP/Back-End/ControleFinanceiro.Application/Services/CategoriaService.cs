using AutoMapper;
using ControleFinanceiro.Application.DTO.Categoria;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;

        public CategoriaService(ICategoriaRepository categoriaRepository, IMapper mapper)
        {
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
        }

        // 🔹 CONSULTAR TODAS
        public async Task<IEnumerable<CategoriaConsultarDTO>> ConsultarAsync()
        {
            var categorias = await _categoriaRepository.ConsultarAsync();
            return _mapper.Map<IEnumerable<CategoriaConsultarDTO>>(categorias);
        }

        // 🔹 CONSULTAR POR ID
        public async Task<CategoriaConsultarDTO> ConsultarPorIdAsync(int id)
        {
            var categoria = await _categoriaRepository.ConsultarPorIdAsync(id);
            if (categoria == null)
                throw new KeyNotFoundException($"Categoria com ID {id} não encontrada.");

            return _mapper.Map<CategoriaConsultarDTO>(categoria);
        }

        // 🔹 CRIAR
        public async Task<CategoriaConsultarDTO> CriarAsync(CategoriaIncluirDTO dto)
        {
            var categoria = _mapper.Map<Categoria>(dto);

            await _categoriaRepository.AdicionarAsync(categoria);
            await _categoriaRepository.SalvarAsync();

            return _mapper.Map<CategoriaConsultarDTO>(categoria);
        }

        // 🔹 ALTERAR
        public async Task<CategoriaConsultarDTO> AlterarAsync(int id, CategoriaAlterarDTO dto)
        {
            var categoriaExistente = await _categoriaRepository.ConsultarPorIdAsync(id);
            if (categoriaExistente == null)
                throw new KeyNotFoundException($"Categoria com ID {id} não encontrada.");

            categoriaExistente.Update(dto.Descricao, dto.Finalidade);

            await _categoriaRepository.SalvarAsync();

            return _mapper.Map<CategoriaConsultarDTO>(categoriaExistente);
        }

        // 🔹 EXCLUIR
        public async Task ExcluirAsync(int id)
        {
            var categoria = await _categoriaRepository.ConsultarPorIdAsync(id);
            if (categoria == null)
                throw new KeyNotFoundException($"Categoria com ID {id} não encontrada.");

            await _categoriaRepository.RemoverAsync(categoria);
            await _categoriaRepository.SalvarAsync();
        }
    }
}
