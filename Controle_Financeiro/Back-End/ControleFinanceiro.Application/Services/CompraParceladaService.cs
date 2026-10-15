using ControleFinanceiro.Application.DTO.CompraParcelada;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Application.Services
{
    public class CompraParceladaService : ICompraParceladaService
    {
        private readonly ICompraParceladaRepository _compraRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICartaoRepository _cartaoRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public CompraParceladaService(
            ICompraParceladaRepository compraRepository,
            IUsuarioRepository usuarioRepository,
            ICartaoRepository cartaoRepository,
            ICategoriaRepository categoriaRepository)
        {
            _compraRepository = compraRepository;
            _usuarioRepository = usuarioRepository;
            _cartaoRepository = cartaoRepository;
            _categoriaRepository = categoriaRepository;
        }

        public async Task<CompraParceladaResponseDTO> AddAsync(
            CompraParceladaCriarDTO dto,
            int userId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(userId);

            if (usuario == null)
                throw new InvalidOperationException("Usuário não encontrado.");

            var cartao = await _cartaoRepository.GetByIdAsync(
                dto.CartaoId,
                userId);

            if (cartao == null)
                throw new InvalidOperationException(
                    "Cartão não encontrado para este usuário.");

            var categoria = await _categoriaRepository.GetByIdAsync(
                dto.CategoriaId,
                userId);

            if (categoria == null)
                throw new InvalidOperationException(
                    "Categoria não encontrada para este usuário.");

            if (dto.ValorTotal <= 0)
                throw new InvalidOperationException(
                    "O valor total deve ser maior que zero.");

            if (dto.QuantidadeParcelas < 2 || dto.QuantidadeParcelas > 60)
                throw new InvalidOperationException(
                    "A quantidade de parcelas deve estar entre 2 e 60.");

            var compra = new CompraParcelada(
                dto.Descricao,
                dto.ValorTotal,
                dto.QuantidadeParcelas,
                dto.DataCompra,
                cartao,
                categoria,
                usuario);

            var valorBase = Math.Floor(
                dto.ValorTotal * 100 / dto.QuantidadeParcelas) / 100;

            var centavosRestantes = (int)Math.Round(
                (dto.ValorTotal - valorBase * dto.QuantidadeParcelas) * 100,
                MidpointRounding.AwayFromZero);

            var primeiroVencimento = CalcularPrimeiroVencimento(
                dto.DataCompra,
                cartao.DiaFechamento,
                cartao.DiaVencimento);

            for (var numero = 1; numero <= dto.QuantidadeParcelas; numero++)
            {
                var valorParcela = valorBase +
                    (numero <= centavosRestantes ? 0.01m : 0m);

                var vencimento = primeiroVencimento.AddMonths(numero - 1);

                var parcela = new Parcela(
                    numero,
                    valorParcela,
                    vencimento,
                    compra);

                compra.AdicionarParcela(parcela);
            }

            await _compraRepository.AddAsync(compra);
            await _compraRepository.SaveAsync();

            return MapearResposta(compra);
        }

        public async Task<IEnumerable<CompraParceladaResponseDTO>> ListByUserAsync(
            int userId)
        {
            var compras = await _compraRepository.ListByUserAsync(userId);

            return compras.Select(MapearResposta);
        }

        public async Task<CompraParceladaResponseDTO> GetByIdAsync(
            int id,
            int userId)
        {
            var compra = await _compraRepository.GetByIdAsync(id);

            if (compra == null)
                throw new KeyNotFoundException(
                    "Compra parcelada não encontrada.");

            if (compra.UsuarioId != userId)
                throw new ForbiddenException("Acesso negado.");

            return MapearResposta(compra);
        }

        public async Task MarcarParcelaComoPagaAsync(
            int parcelaId,
            int usuarioId)
        {
            var parcela = await _compraRepository.GetParcelaByIdAsync(parcelaId);

            if (parcela is null)
                throw new KeyNotFoundException("Parcela não encontrada.");

            if (parcela.CompraParcelada.UsuarioId != usuarioId)
                throw new UnauthorizedAccessException(
                    "Você não tem permissão para alterar esta parcela.");

            parcela.MarcarComoPaga();

            await _compraRepository.SaveAsync();
        }

        public async Task ReabrirParcelaAsync(
            int parcelaId,
            int usuarioId)
        {
            var parcela = await _compraRepository.GetParcelaByIdAsync(parcelaId);

            if (parcela is null)
                throw new KeyNotFoundException("Parcela não encontrada.");

            if (parcela.CompraParcelada.UsuarioId != usuarioId)
                throw new UnauthorizedAccessException(
                    "Você não tem permissão para alterar esta parcela.");

            parcela.Reabrir();

            await _compraRepository.SaveAsync();
        }

        public async Task<CompraParceladaResponseDTO> UpdateAsync(
            int id,
            CompraParceladaAtualizarDTO dto,
            int userId)
        {
            var compra = await _compraRepository.GetByIdAsync(id);

            if (compra is null)
                throw new KeyNotFoundException(
                    "Compra parcelada não encontrada.");

            if (compra.UsuarioId != userId)
                throw new ForbiddenException("Acesso negado.");

            if (compra.Parcelas.Any(p => p.Paga))
                throw new InvalidOperationException(
                    "Não é possível editar uma compra com parcelas pagas.");

            var cartao = await _cartaoRepository.GetByIdAsync(
                dto.CartaoId,
                userId);

            if (cartao is null)
                throw new InvalidOperationException(
                    "Cartão não encontrado para este usuário.");

            var categoria = await _categoriaRepository.GetByIdAsync(
                dto.CategoriaId,
                userId);

            if (categoria is null)
                throw new InvalidOperationException(
                    "Categoria não encontrada para este usuário.");

            compra.Atualizar(
                dto.Descricao,
                dto.ValorTotal,
                dto.QuantidadeParcelas,
                dto.DataCompra,
                cartao,
                categoria);

            await _compraRepository.RemoverParcelasAsync(
                compra.Parcelas.ToList());

            compra.Parcelas.Clear();

            var valorBase = Math.Floor(
                dto.ValorTotal * 100 / dto.QuantidadeParcelas) / 100;

            var centavosRestantes = (int)Math.Round(
                (dto.ValorTotal - valorBase * dto.QuantidadeParcelas) * 100,
                MidpointRounding.AwayFromZero);

            var primeiroVencimento = CalcularPrimeiroVencimento(
                dto.DataCompra,
                cartao.DiaFechamento,
                cartao.DiaVencimento);

            for (var numero = 1; numero <= dto.QuantidadeParcelas; numero++)
            {
                var valorParcela = valorBase +
                    (numero <= centavosRestantes ? 0.01m : 0m);

                var vencimento = primeiroVencimento.AddMonths(numero - 1);

                compra.AdicionarParcela(new Parcela(
                    numero,
                    valorParcela,
                    vencimento,
                    compra));
            }

            await _compraRepository.SaveAsync();

            return MapearResposta(compra);
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var compra = await _compraRepository.GetByIdAsync(id);

            if (compra == null)
                throw new KeyNotFoundException(
                    "Compra parcelada não encontrada.");

            if (compra.UsuarioId != userId)
                throw new ForbiddenException("Acesso negado.");

            await _compraRepository.DeleteAsync(compra);
            await _compraRepository.SaveAsync();
        }

        private static DateOnly CalcularPrimeiroVencimento(
            DateOnly dataCompra,
            int diaFechamento,
            int diaVencimento)
        {
            var mesFatura = dataCompra;

            if (dataCompra.Day >= diaFechamento)
                mesFatura = dataCompra.AddMonths(1);

            var dataFatura = CriarDataNoMes(
                mesFatura.Year,
                mesFatura.Month,
                diaVencimento);

            if (dataFatura <= dataCompra)
            {
                var mesSeguinte = mesFatura.AddMonths(1);

                dataFatura = CriarDataNoMes(
                    mesSeguinte.Year,
                    mesSeguinte.Month,
                    diaVencimento);
            }

            return dataFatura;
        }

        private static DateOnly CriarDataNoMes(
            int ano,
            int mes,
            int dia)
        {
            var ultimoDia = DateTime.DaysInMonth(ano, mes);

            return new DateOnly(
                ano,
                mes,
                Math.Min(dia, ultimoDia));
        }

        private static CompraParceladaResponseDTO MapearResposta(
            CompraParcelada compra)
        {
            return new CompraParceladaResponseDTO
            {
                Id = compra.Id,
                Descricao = compra.Descricao,
                ValorTotal = compra.ValorTotal,
                QuantidadeParcelas = compra.QuantidadeParcelas,
                DataCompra = compra.DataCompra,
                CartaoId = compra.CartaoId,
                CategoriaId = compra.CategoriaId,

                Parcelas = compra.Parcelas
                    .OrderBy(p => p.Numero)
                    .Select(p => new ParcelaResponseDTO
                    {
                        Id = p.Id,
                        Numero = p.Numero,
                        Valor = p.Valor,
                        DataVencimento = p.DataVencimento,
                        Paga = p.Paga
                    })
                    .ToList()
            };
        }
    }
}
