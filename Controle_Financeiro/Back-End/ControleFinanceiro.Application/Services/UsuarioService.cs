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

        //  CONSULTAR TODOS
        public async Task<IEnumerable<UsuarioConsultarDTO>> ListAsync()
        {
            var usuarios = await _usuarioRepository.ListAsync();
            return _mapper.Map<IEnumerable<UsuarioConsultarDTO>>(usuarios);
        }

        //  CONSULTAR POR ID
        public async Task<UsuarioConsultarDTO> GetByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

            return _mapper.Map<UsuarioConsultarDTO>(usuario);
        }

        //  CRIAR
        public async Task<UsuarioConsultarDTO> AddAsync(UsuarioIncluirDTO dto)
        {
            var usuario = _mapper.Map<Usuario>(dto);

            await _usuarioRepository.AddAsync(usuario);
            await _usuarioRepository.SaveAsync();

            return _mapper.Map<UsuarioConsultarDTO>(usuario);
        }

        //  ALTERAR
        public async Task<UsuarioConsultarDTO> UpdateAsync(int id, UsuarioAlterarDTO dto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

            usuario.Update(dto.Nome,dto.Idade,dto.Email); // ajuste conforme sua entidade

            await _usuarioRepository.SaveAsync();

            return _mapper.Map<UsuarioConsultarDTO>(usuario);
        }

        //  EXCLUIR
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
