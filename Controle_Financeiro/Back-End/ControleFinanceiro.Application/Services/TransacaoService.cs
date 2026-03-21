using AutoMapper;
using ControleFinanceiro.Application.DTO.Transacao;
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

        //  LISTAR TODAS
        public async Task<IEnumerable<TransacaoConsultarDTO>> ListAsync()
        {
            var transacoes = await _transacaoRepository.ListAsync();
            return _mapper.Map<IEnumerable<TransacaoConsultarDTO>>(transacoes);
        }

        //  OBTER POR ID
        public async Task<TransacaoConsultarDTO> GetByIdAsync(int id)
        {
            var transacao = await _transacaoRepository.GetByIdAsync(id);

            if (transacao == null)
                throw new KeyNotFoundException($"Transação com ID {id} não encontrada.");

            return _mapper.Map<TransacaoConsultarDTO>(transacao);
        }

        //  LISTAR POR USUÁRIO
        public async Task<IEnumerable<TransacaoConsultarDTO>> ListByUserAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {usuarioId} não encontrado.");

            var transacoes = await _transacaoRepository.ListByUserAsync(usuarioId);
            return _mapper.Map<IEnumerable<TransacaoConsultarDTO>>(transacoes);
        }

        //  LISTAR POR CATEGORIA
        public async Task<IEnumerable<TransacaoConsultarDTO>> ListByCategoryAsync(int categoriaId)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(categoriaId);
            if (categoria == null)
                throw new KeyNotFoundException($"Categoria com ID {categoriaId} não encontrada.");

            var transacoes = await _transacaoRepository.ListByCategoryAsync(categoriaId);
            return _mapper.Map<IEnumerable<TransacaoConsultarDTO>>(transacoes);
        }

        //  CRIAR
        public async Task<TransacaoConsultarDTO> AddAsync(TransacaoCriarDTO dto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(dto.UsuarioId);
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

        //  ATUALIZAR
        public async Task<TransacaoConsultarDTO> UpdateAsync(int id, TransacaoAtualizarDTO dto)
        {
            var transacao = await _transacaoRepository.GetByIdAsync(id);
            if (transacao == null)
                throw new KeyNotFoundException("Transação não encontrada.");

            var categoria = await _categoriaRepository.GetByIdAsync(dto.CategoriaId);
            if (categoria == null)
                throw new InvalidOperationException("Categoria não encontrada.");

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

        public async Task DeleteAsync(int id)
        {
            var transacao = await _transacaoRepository.GetByIdAsync(id);
            if (transacao == null)
                throw new KeyNotFoundException("Transação não encontrada.");

            await _transacaoRepository.DeleteAsync(transacao);
            await _transacaoRepository.SaveAsync();
        }
    }
}
