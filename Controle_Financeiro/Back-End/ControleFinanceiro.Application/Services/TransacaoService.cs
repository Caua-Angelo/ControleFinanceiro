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
        public async Task<IEnumerable<TransacaoConsultarDTO>> ListarAsync()
        {
            var transacoes = await _transacaoRepository.ListarAsync();
            return _mapper.Map<IEnumerable<TransacaoConsultarDTO>>(transacoes);
        }

        //  OBTER POR ID
        public async Task<TransacaoConsultarDTO> ObterPorIdAsync(int id)
        {
            var transacao = await _transacaoRepository.ObterPorIdAsync(id);

            if (transacao == null)
                throw new KeyNotFoundException($"Transação com ID {id} não encontrada.");

            return _mapper.Map<TransacaoConsultarDTO>(transacao);
        }

        //  LISTAR POR USUÁRIO
        public async Task<IEnumerable<TransacaoConsultarDTO>> ListarPorUsuarioAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {usuarioId} não encontrado.");

            var transacoes = await _transacaoRepository.ListarPorUsuarioAsync(usuarioId);
            return _mapper.Map<IEnumerable<TransacaoConsultarDTO>>(transacoes);
        }

        //  LISTAR POR CATEGORIA
        public async Task<IEnumerable<TransacaoConsultarDTO>> ListarPorCategoriaAsync(int categoriaId)
        {
            var categoria = await _categoriaRepository.ConsultarPorIdAsync(categoriaId);
            if (categoria == null)
                throw new KeyNotFoundException($"Categoria com ID {categoriaId} não encontrada.");

            var transacoes = await _transacaoRepository.ListarPorCategoriaAsync(categoriaId);
            return _mapper.Map<IEnumerable<TransacaoConsultarDTO>>(transacoes);
        }

        //  CRIAR
        public async Task<TransacaoConsultarDTO> CriarAsync(TransacaoCriarDTO dto)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(dto.UsuarioId);
            if (usuario == null)
                throw new InvalidOperationException("Usuário não encontrado.");

            var categoria = await _categoriaRepository.ConsultarPorIdAsync(dto.CategoriaId);
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

            await _transacaoRepository.AdicionarAsync(transacao);
            await _transacaoRepository.SalvarAsync();

            return _mapper.Map<TransacaoConsultarDTO>(transacao);
        }

        //  ATUALIZAR
        public async Task<TransacaoConsultarDTO> AtualizarAsync(int id, TransacaoAtualizarDTO dto)
        {
            var transacao = await _transacaoRepository.ObterPorIdAsync(id);
            if (transacao == null)
                throw new KeyNotFoundException("Transação não encontrada.");

            var categoria = await _categoriaRepository.ConsultarPorIdAsync(dto.CategoriaId);
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

            await _transacaoRepository.SalvarAsync();

            return _mapper.Map<TransacaoConsultarDTO>(transacao);
        }

        public async Task ExcluirAsync(int id)
        {
            var transacao = await _transacaoRepository.ObterPorIdAsync(id);
            if (transacao == null)
                throw new KeyNotFoundException("Transação não encontrada.");

            await _transacaoRepository.RemoverAsync(transacao);
            await _transacaoRepository.SalvarAsync();
        }
    }
}
