using AutoMapper;

namespace NZWalks.API.Mappings
{
    public class RegionsProfile : Profile
    {
        public RegionsProfile()
        {
            CreateMap<Models.Domain.Region, Models.DTO.Region>().ReverseMap();

        }
    }
}
