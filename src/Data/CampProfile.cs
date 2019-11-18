using AutoMapper;
using CoreCodeCamp.DTO;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampDTO>()
                .ForMember(c => c.Venue, o => o.MapFrom(m => m.Location.VenueName))
                .ReverseMap();

            this.CreateMap<Talk, TalksDTO>().ReverseMap()
                .ForMember(t => t.Camp, o => o.Ignore())
                .ForMember(t => t.Speaker, o => o.Ignore());

            this.CreateMap<Speaker, SpeakerDTO>().ReverseMap();            
               
        }
    }
}
