using AutoMapper;
using ControleFinanceiro.Application.DTO;
using ControleFinanceiro.Domain.Models;

namespace ControleFinanceiro.AutoMapper
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Usuario -> colaboradorconsultadto
            CreateMap<Usuario, ColaboradorConsultarDTO>()
    .ForMember(dest => dest.EquipeNome,
               opt => opt.MapFrom(src => src.Equipe.sNome));
            CreateMap<Usuario, ColaboradorConsultarFeriasDTO>()
    .ForMember(dest => dest.EquipeNome,
               opt => opt.MapFrom(src => src.Equipe.sNome));
            CreateMap<Usuario, ColaboradorIncluirDTO>();
            CreateMap<Usuario, ColaboradorAlterarDTO>();


            // dto -> entidade (incluir Usuario)
            CreateMap<ColaboradorIncluirDTO, Usuario>();

            // ferias -> feriasdto (com datas formatadas)
            //CreateMap<Ferias, FeriasConsultarDTO>().ReverseMap()
            //    .ForMember(dest => dest.dDataInicio,
            //        opt => opt.MapFrom(src => src.dDataInicio.ToString("dd/MM/yyyy")))
            //    .ForMember(dest => dest.dDataFinal,
            //        opt => opt.MapFrom(src => src.dDataFinal.ToString("dd/MM/yyyy")));

            //CreateMap<Ferias, FeriasIncluirDTO>().ReverseMap()
            //    .ForMember(dest => dest.dDataInicio,
            //        opt => opt.MapFrom(src => src.dDataInicio.ToString("dd/MM/yyyy")))
            //    .ForMember(dest => dest.dDataFinal,
            //        opt => opt.MapFrom(src => src.dDataFinal.ToString("dd/MM/yyyy")));
            //CreateMap<Ferias,FeriasAlterarDTO>().ReverseMap()
            //    .ForMember(dest => dest.dDataInicio,
            //        opt => opt.MapFrom(src => src.dDataInicio.ToString("dd/MM/yyyy")))
            //    .ForMember(dest => dest.dDataFinal,
            //        opt => opt.MapFrom(src => src.dDataFinal.ToString("dd/MM/yyyy")));

          

            // Mapeamento para FeriasIncluirDTO
            CreateMap<Ferias, FeriasIncluirDTO>()
                .ForMember(dest => dest.dDataInicio,
                           opt => opt.MapFrom(src => src.dDataInicio.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.dDataFinal,
                           opt => opt.MapFrom(src => src.dDataFinal.ToString("dd/MM/yyyy")))
                .ReverseMap();

            // Mapeamento para FeriasAlterarDTO
            CreateMap<Ferias, FeriasAlterarDTO>()
                .ForMember(dest => dest.dDataInicio,
                           opt => opt.MapFrom(src => src.dDataInicio.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.dDataFinal,
                           opt => opt.MapFrom(src => src.dDataFinal.ToString("dd/MM/yyyy")))
                .ReverseMap();

            //equipe -> equipedto
            CreateMap<Equipe, EquipeIncluirDTO>().ReverseMap();
            CreateMap<Equipe, EquipeAlterarDTO>().ReverseMap();
            CreateMap<Equipe, EquipeConsultarDTO>().ReverseMap();
            //CreateMap<Equipe, Ferias>().ReverseMap();
        }
    }
}