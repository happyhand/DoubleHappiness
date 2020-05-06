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
            CreateMap<MemberDao, MemberHomeInfoView>();
        }
    }
}