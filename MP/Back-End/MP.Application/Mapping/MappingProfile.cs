using AutoMapper;
using ControleFerias.Application.DTO;
using ControleFerias.Domain.Models;

namespace ControleFerias.AutoMapper
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // colaborador -> colaboradorconsultadto
            CreateMap<Colaborador, ColaboradorConsultarDTO>()
    .ForMember(dest => dest.EquipeNome,
               opt => opt.MapFrom(src => src.Equipe.sNome));
            CreateMap<Colaborador, ColaboradorConsultarFeriasDTO>()
    .ForMember(dest => dest.EquipeNome,
               opt => opt.MapFrom(src => src.Equipe.sNome));
            CreateMap<Colaborador, ColaboradorIncluirDTO>();
            CreateMap<Colaborador, ColaboradorAlterarDTO>();


            // dto -> entidade (incluir colaborador)
            CreateMap<ColaboradorIncluirDTO, Colaborador>();

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

            CreateMap<Ferias, FeriasConsultarDTO>()
    .ForMember(dest => dest.ColaboradorId,
               opt => opt.MapFrom(src => src.ColaboradorFerias.FirstOrDefault().ColaboradorId))
    .ForMember(dest => dest.dDataInicio,
               opt => opt.MapFrom(src => src.dDataInicio.ToString("dd/MM/yyyy")))
    .ForMember(dest => dest.dDataFinal,
               opt => opt.MapFrom(src => src.dDataFinal.ToString("dd/MM/yyyy")))
    .ReverseMap(); // Se precisar de mapeamento reverso

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