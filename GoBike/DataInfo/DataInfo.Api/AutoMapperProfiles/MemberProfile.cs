using AutoMapper;
using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Member.View;
using Newtonsoft.Json;

namespace DataInfo.AutoMapperProfiles
{
    /// <summary>
    /// Member AutoMapper
    /// </summary>
    public class MemberProfile : Profile
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public MemberProfile()
        {
            CreateMap<MemberDao, MemberSimpleInfoView>();
            CreateMap<MemberDao, MemberDetailInfoView>();

            CreateMap<RideModel, RideInfoContent>()
            .ForMember(dto => dto.ShareContent, options => options.MapFrom(data => JsonConvert.DeserializeObject(data.ShareContent)))
            .ForMember(dto => dto.Route, options => options.MapFrom(data => JsonConvert.DeserializeObject(data.Route)));
            CreateMap<RideInfoContent, RideModel>()
            .ForMember(data => data.ShareContent, options => options.MapFrom(dto => JsonConvert.SerializeObject(dto.ShareContent)))
            .ForMember(data => data.Route, options => options.MapFrom(dto => JsonConvert.SerializeObject(dto.Route)));
            CreateMap<RideModel, RideInfoView>()
            .ForMember(dto => dto.ShareContent, options => options.MapFrom(data => JsonConvert.DeserializeObject(data.ShareContent)))
            .ForMember(dto => dto.Route, options => options.MapFrom(data => JsonConvert.DeserializeObject(data.Route)));
        }
    }
}