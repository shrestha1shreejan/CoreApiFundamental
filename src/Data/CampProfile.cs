using AutoMapper;
using CoreCodeCamp.DTO;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampDTO>()
                .ForMember(c => c.Venue, o => o.MapFrom(m => m.Location.VenueName));
            this.CreateMap<CampDTO, Camp>()
                .ForMember(c => c.Location, o => o.MapFrom(m => new Location {
                    VenueName = m.Venue,
                    Address1 = m.LocationAddress1,
                    Address2 = m.LocationAddress2,
                    Address3 = m.LocationAddress3,
                    CityTown = m.LocationCityTown,
                    Country = m.LocationCountry,
                    PostalCode = m.LocationPostalCode,
                    StateProvince = m.LocationStateProvince
                }));
               
        }
    }
}
