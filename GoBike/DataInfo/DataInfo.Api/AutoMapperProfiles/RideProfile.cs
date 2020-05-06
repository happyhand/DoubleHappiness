using AutoMapper;
using DataInfo.Core.Models.Dao.Ride;
using DataInfo.Core.Models.Dto.Ride.Content;
using DataInfo.Core.Models.Dto.Ride.Request;
using Newtonsoft.Json;

namespace DataInfo.AutoMapperProfiles
{
    /// <summary>
    /// Ride AutoMapper
    /// </summary>
    public class RideProfile : Profile
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public RideProfile()
        {
            CreateMap<AddRideInfoContent, AddRideInfoRequest>()
            .ForMember(data => data.ShareContent, options => options.MapFrom(dto => JsonConvert.SerializeObject(dto.ShareContent)))
            .ForMember(data => data.Route, options => options.MapFrom(dto => JsonConvert.SerializeObject(dto.Route)));
        }
    }
}