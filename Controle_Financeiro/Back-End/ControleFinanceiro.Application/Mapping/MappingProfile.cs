using AutoMapper;
using ControleFinanceiro.Application.DTO.Categoria;
using ControleFinanceiro.Application.DTO.Resumo;
using ControleFinanceiro.Application.DTO.Transacao;
using ControleFinanceiro.Application.DTO.Usuario;
using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Usuario, UsuarioConsultarDTO>();
            CreateMap<UsuarioIncluirDTO, Usuario>();
            CreateMap<UsuarioAlterarDTO, Usuario>();

            CreateMap<Categoria, CategoriaConsultarDTO>();
            CreateMap<CategoriaIncluirDTO, Categoria>();
            CreateMap<CategoriaAlterarDTO, Categoria>();

            CreateMap<Transacao, TransacaoConsultarDTO>()
                .ForMember(dest => dest.UsuarioNome,
                    opt => opt.MapFrom(src => src.Usuario.Nome))
                .ForMember(dest => dest.CategoriaDescricao,
                    opt => opt.MapFrom(src => src.Categoria.Descricao))
                .ForMember(dest => dest.Data,  // ← ADICIONAR ESSA LINHA
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<TransacaoCriarDTO, Transacao>()
                .ForMember(dest => dest.Usuario, opt => opt.Ignore())
                .ForMember(dest => dest.Categoria, opt => opt.Ignore());

            CreateMap<TransacaoAtualizarDTO, Transacao>()
                .ForMember(dest => dest.Usuario, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore());

            CreateMap<Usuario, UsuarioResumoFinanceiroDTO>()
                .ForMember(dest => dest.UsuarioId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome,
                    opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.TotalReceitas,
                    opt => opt.MapFrom(src =>
                        src.Transacao
                           .Where(t => t.Tipo == Domain.Enums.TipoTransacao.Receita)
                           .Sum(t => t.Valor)))
                .ForMember(dest => dest.TotalDespesas,
                    opt => opt.MapFrom(src =>
                        src.Transacao
                           .Where(t => t.Tipo == Domain.Enums.TipoTransacao.Despesa)
                           .Sum(t => t.Valor)));
        }
    }
}