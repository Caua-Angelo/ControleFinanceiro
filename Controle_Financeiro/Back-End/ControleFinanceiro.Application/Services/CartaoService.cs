
using AutoMapper;
using ControleFinanceiro.Application.DTO.Cartao;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Application.Services
{
    public class CartaoService : ICartaoService
    {
        private readonly ICartaoRepository _cartaoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public CartaoService(
            ICartaoRepository cartaoRepository,
            IUsuarioRepository usuarioRepository,
            IMapper mapper)
        {
            _cartaoRepository = cartaoRepository;
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CartaoConsultarDTO>> ListAsync(int usuarioId)
        {
            var cartoes = await _cartaoRepository.ListAsync(usuarioId);
            return _mapper.Map<IEnumerable<CartaoConsultarDTO>>(cartoes);
        }

        public async Task<CartaoConsultarDTO> GetByIdAsync(int id, int usuarioId)
        {
            var cartao = await _cartaoRepository.GetByIdAsync(id, usuarioId);

            if (cartao == null)
                throw new KeyNotFoundException(
                    $"Cartão com ID {id} não encontrado.");

            return _mapper.Map<CartaoConsultarDTO>(cartao);
        }

        public async Task<CartaoConsultarDTO> AddAsync(
            CartaoIncluirDTO dto, int usuarioId)
        {
            var usuario = await _usuarioRepository.GetByIdTrackedAsync(usuarioId);

            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            var cartao = new Cartao(
                dto.Nome,
                dto.Imagem,
                dto.DiaFechamento,
                dto.DiaVencimento,
                usuario);

            await _cartaoRepository.AddAsync(cartao);
            await _cartaoRepository.SaveAsync();

            return _mapper.Map<CartaoConsultarDTO>(cartao);
        }

        public async Task<CartaoConsultarDTO> UpdateAsync(
            int id, CartaoAlterarDTO dto, int usuarioId)
        {
            var usuario = await _usuarioRepository.GetByIdTrackedAsync(usuarioId);
            var cartao = await _cartaoRepository.GetByIdAsync(id, usuarioId);

            if (cartao == null)
                throw new KeyNotFoundException(
                    $"Cartão com ID {id} não encontrado.");

            cartao.Update(
                dto.Nome,
                dto.Imagem,
                dto.DiaFechamento,
                dto.DiaVencimento);

            await _cartaoRepository.SaveAsync();

            return _mapper.Map<CartaoConsultarDTO>(cartao);
        }

        public async Task DeleteAsync(int id, int usuarioId)
        {
            var cartao = await _cartaoRepository.GetByIdAsync(id, usuarioId);

            if (cartao == null)
                throw new KeyNotFoundException(
                    $"Cartão com ID {id} não encontrado.");

            await _cartaoRepository.DeleteAsync(cartao);
            await _cartaoRepository.SaveAsync();
        }
    }
}
