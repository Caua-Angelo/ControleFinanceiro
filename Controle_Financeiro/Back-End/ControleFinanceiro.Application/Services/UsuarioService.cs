using AutoMapper;
using ControleFinanceiro.Application.DTO.Usuario;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public UsuarioService(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        public async Task<UsuarioConsultarDTO> GetByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

            return _mapper.Map<UsuarioConsultarDTO>(usuario);
        }

        
        public async Task<UsuarioConsultarDTO> AddAsync(UsuarioIncluirDTO dto)
        {
            var existente = await _usuarioRepository.GetByEmailAsync(dto.Email);
            if (existente != null)
                throw new InvalidOperationException("Já existe um usuário cadastrado com este e-mail.");

            var usuario = _mapper.Map<Usuario>(dto);

            await _usuarioRepository.AddAsync(usuario);
            await _usuarioRepository.SaveAsync();

            return _mapper.Map<UsuarioConsultarDTO>(usuario);
        }

        
        public async Task<UsuarioConsultarDTO> UpdateAsync(int id, UsuarioAlterarDTO dto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

            usuario.Update(dto.Nome,dto.Idade); 

            await _usuarioRepository.SaveAsync();

            return _mapper.Map<UsuarioConsultarDTO>(usuario);
        }

        
        public async Task DeleteAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

            await _usuarioRepository.DeleteAsync(usuario);
            await _usuarioRepository.SaveAsync();
        }
    }
}
