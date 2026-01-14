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

        // 🔹 CONSULTAR TODOS
        public async Task<IEnumerable<UsuarioConsultarDTO>> ConsultarAsync()
        {
            var usuarios = await _usuarioRepository.ListarAsync();
            return _mapper.Map<IEnumerable<UsuarioConsultarDTO>>(usuarios);
        }

        // 🔹 CONSULTAR POR ID
        public async Task<UsuarioConsultarDTO> ConsultarPorIdAsync(int id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

            return _mapper.Map<UsuarioConsultarDTO>(usuario);
        }

        // 🔹 CRIAR
        public async Task<UsuarioConsultarDTO> CriarAsync(UsuarioIncluirDTO dto)
        {
            var usuario = _mapper.Map<Usuario>(dto);

            await _usuarioRepository.AdicionarAsync(usuario);
            await _usuarioRepository.SalvarAsync();

            return _mapper.Map<UsuarioConsultarDTO>(usuario);
        }

        // 🔹 ALTERAR
        public async Task<UsuarioConsultarDTO> AlterarAsync(int id, UsuarioAlterarDTO dto)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

            usuario.Update(dto.Nome,dto.Idade); // ajuste conforme sua entidade

            await _usuarioRepository.SalvarAsync();

            return _mapper.Map<UsuarioConsultarDTO>(usuario);
        }

        // 🔹 EXCLUIR
        public async Task ExcluirAsync(int id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");

            await _usuarioRepository.RemoverAsync(usuario);
            await _usuarioRepository.SalvarAsync();
        }
    }
}
