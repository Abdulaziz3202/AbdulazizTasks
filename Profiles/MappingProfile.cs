using AutoMapper;
using KPMGTask.Dtos;
using KPMGTask.Models;

namespace KPMGTask.Profiles
{
    public class MappingProfile:Profile
    {

        public MappingProfile()
        {

            //Source -> Target
         /*   CreateMap<Command, CommandReadDto>().ReverseMap(); ;
            CreateMap<CommandCreateDto, Command>().ReverseMap();
            CreateMap<CommandUpdateDto, Command>().ReverseMap();
            CreateMap<Command, CommandUpdateDto>().ReverseMap();*/


            /*CreateMap<GovHospitalTotal, BIHospitalResultDto>()
            .ForMember(targetObj => targetObj.Wplace, sourceObj => sourceObj.MapFrom(i => i.GhtWplace));*/
        }



    }
}
