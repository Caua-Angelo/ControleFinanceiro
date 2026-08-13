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


        public async Task<IEnumerable<CategoriaConsultarDTO>> ListAsync(int usuarioId)
        {
            var categorias = await _categoriaRepository.ListAsync(usuarioId);
            return _mapper.Map<IEnumerable<CategoriaConsultarDTO>>(categorias);
        }

        
        public async Task<CategoriaConsultarDTO> GetByIdAsync(int id, int usuarioId)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id, usuarioId);
            if (categoria == null)
                throw new KeyNotFoundException($"Categoria com ID {id} não encontrada.");

            return _mapper.Map<CategoriaConsultarDTO>(categoria);
        }

        
        public async Task<CategoriaConsultarDTO> AddAsync(CategoriaIncluirDTO dto, int usuarioId)
        {
            var categoria = _mapper.Map<Categoria>(dto);
            categoria.UsuarioId = usuarioId;

            await _categoriaRepository.AddAsync(categoria);
            await _categoriaRepository.SaveAsync();

            return _mapper.Map<CategoriaConsultarDTO>(categoria);
        }

        
        public async Task<CategoriaConsultarDTO> UpdateAsync(int id, CategoriaAlterarDTO dto, int usuarioId)
        {
            var categoriaExistente = await _categoriaRepository.GetByIdAsync(id, usuarioId);
            if (categoriaExistente == null)
                throw new KeyNotFoundException($"Categoria com ID {id} não encontrada.");

            if (categoriaExistente.UsuarioId == null)
                throw new InvalidOperationException(
                    "Categorias básicas não podem ser alteradas.");

            categoriaExistente.Update(dto.Descricao, dto.Finalidade);

            await _categoriaRepository.SaveAsync();

            return _mapper.Map<CategoriaConsultarDTO>(categoriaExistente);
        }

        
        public async Task DeleteAsync(int id, int usuarioId)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id, usuarioId);
            if (categoria == null)
                throw new KeyNotFoundException($"Categoria com ID {id} não encontrada.");

            if (categoria.UsuarioId == null)
                throw new InvalidOperationException(
                    "Categorias básicas não podem ser excluídas.");

            if (categoria.Transacoes.Any())
            {
                throw new InvalidOperationException("Categoria possui transações vinculadas");
            }

            await _categoriaRepository.DeleteAsync(categoria);
            await _categoriaRepository.SaveAsync();
        }
    }
}
