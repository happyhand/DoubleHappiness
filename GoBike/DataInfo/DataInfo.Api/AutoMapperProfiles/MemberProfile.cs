using System;
using System.Threading.Tasks;
using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Repository.Interfaces;
using DataInfo.Repository.Models.Member;
using DataInfo.Service.Enums;
using DataInfo.Service.Models.Member.Content;
using DataInfo.Service.Models.Member.View;
using Newtonsoft.Json;
using NLog;

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
            CreateMap<MemberModel, MemberSimpleInfoViewDto>();
            CreateMap<MemberModel, MemberDetailInfoViewDto>();

            CreateMap<RideModel, RideInfoContent>()
            .ForMember(dto => dto.ShareContent, options => options.MapFrom(data => JsonConvert.DeserializeObject(data.ShareContent)))
            .ForMember(dto => dto.Route, options => options.MapFrom(data => JsonConvert.DeserializeObject(data.Route)));
            CreateMap<RideInfoContent, RideModel>()
            .ForMember(data => data.ShareContent, options => options.MapFrom(dto => JsonConvert.SerializeObject(dto.ShareContent)))
            .ForMember(data => data.Route, options => options.MapFrom(dto => JsonConvert.SerializeObject(dto.Route)));
            CreateMap<RideModel, RideInfoViewDto>()
            .ForMember(dto => dto.ShareContent, options => options.MapFrom(data => JsonConvert.DeserializeObject(data.ShareContent)))
            .ForMember(dto => dto.Route, options => options.MapFrom(data => JsonConvert.DeserializeObject(data.Route)));
        }
    }
}