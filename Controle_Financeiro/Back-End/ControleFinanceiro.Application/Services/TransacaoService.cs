using AutoMapper;
using ControleFinanceiro.Application.DTO.Transacao;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Enums;
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Application.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;

        public TransacaoService(
            ITransacaoRepository transacaoRepository,
            IUsuarioRepository usuarioRepository,
            ICategoriaRepository categoriaRepository,
            IMapper mapper)
        {
            _transacaoRepository = transacaoRepository;
            _usuarioRepository = usuarioRepository;
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
        }


        public async Task<TransacaoConsultarDTO> GetByIdAsync(int id, int userId)
        {
            var transacao = await _transacaoRepository.GetByIdAsync(id);

            if (transacao == null)
                throw new KeyNotFoundException("Transação não encontrada.");

            if (transacao.UsuarioId != userId)
                throw new ForbiddenException("Acesso negado.");

            return _mapper.Map<TransacaoConsultarDTO>(transacao);
        }


        public async Task<IEnumerable<TransacaoConsultarDTO>> ListByUserAsync(int userId)
        {
            var transacoes = await _transacaoRepository.ListByUserAsync(userId);
            return _mapper.Map<IEnumerable<TransacaoConsultarDTO>>(transacoes);
        }

        public async Task<TransacaoConsultarDTO> AddAsync(TransacaoCriarDTO dto, int userId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(userId);
            if (usuario == null)
                throw new InvalidOperationException("Usuário não encontrado.");

            var categoria = await _categoriaRepository.GetByIdAsync(dto.CategoriaId);
            if (categoria == null)
                throw new InvalidOperationException("Categoria não encontrada.");

            // menor de idade só pode despesa
            if (usuario.Idade < 18 && dto.Tipo != TipoTransacao.Despesa)
                throw new InvalidOperationException(
                    "Usuários menores de 18 anos podem registrar apenas despesas.");

            // categoria compatível com tipo
            if (dto.Tipo == TipoTransacao.Despesa &&
                categoria.Finalidade == FinalidadeCategoria.Receita)
                throw new InvalidOperationException(
                    "Categoria de receita não pode ser usada em despesas.");

            if (dto.Tipo == TipoTransacao.Receita &&
                categoria.Finalidade == FinalidadeCategoria.Despesa)
                throw new InvalidOperationException(
                    "Categoria de despesa não pode ser usada em receitas.");

            if (dto.Valor <= 0)
                throw new InvalidOperationException("Valor deve ser maior que zero.");

            var transacao = new Transacao(
                dto.Descricao,
                dto.Valor,
                dto.Tipo,
                categoria,
                usuario, 
                dto.Data
            );

            await _transacaoRepository.AddAsync(transacao);
            await _transacaoRepository.SaveAsync();

            return _mapper.Map<TransacaoConsultarDTO>(transacao);
        }

        public async Task<TransacaoConsultarDTO> UpdateAsync(int id, TransacaoAtualizarDTO dto, int userId)
        {
            var transacao = await _transacaoRepository.GetByIdAsync(id);
            if (transacao == null)
                throw new KeyNotFoundException("Transação não encontrada.");

            if (transacao.UsuarioId != userId)
                throw new ForbiddenException("Acesso negado.");

            var categoria = await _categoriaRepository.GetByIdAsync(dto.CategoriaId);
            if (categoria == null)
                throw new InvalidOperationException("Categoria não encontrada.");

            if (dto.Valor <= 0)
                throw new InvalidOperationException("Valor deve ser maior que zero.");

            transacao.Update(
                dto.Descricao,
                dto.Valor,
                dto.Tipo,
                categoria,
                transacao.Usuario,
                dto.Data
            );

            await _transacaoRepository.SaveAsync();

            return _mapper.Map<TransacaoConsultarDTO>(transacao);
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var transacao = await _transacaoRepository.GetByIdAsync(id);
            if (transacao == null)
                throw new KeyNotFoundException("Transação não encontrada.");

            if (transacao.UsuarioId != userId)
                throw new ForbiddenException("Acesso negado.");

            await _transacaoRepository.DeleteAsync(transacao);
            await _transacaoRepository.SaveAsync();
        }
    }
}
