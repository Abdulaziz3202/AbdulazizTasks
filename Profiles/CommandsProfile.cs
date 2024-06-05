using AutoMapper;
using MVCRESTAPI.Dtos;
using MVCRESTAPI.Modles;

namespace MVCRESTAPI.Profiles
{
    public class CommandsProfile:Profile
    {

        public CommandsProfile()
        {

            //Source -> Target
            CreateMap<Command, CommandReadDto>().ReverseMap(); ;
            CreateMap<CommandCreateDto, Command>().ReverseMap();
            CreateMap<CommandUpdateDto, Command>().ReverseMap();
            CreateMap<Command, CommandUpdateDto>().ReverseMap();


            /*CreateMap<GovHospitalTotal, BIHospitalResultDto>()
            .ForMember(targetObj => targetObj.Wplace, sourceObj => sourceObj.MapFrom(i => i.GhtWplace));*/
        }



    }
}
