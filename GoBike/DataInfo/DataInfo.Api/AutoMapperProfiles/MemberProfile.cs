using AutoMapper;
using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Member.View;
using Newtonsoft.Json;
using System.Linq;

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
            CreateMap<MemberDao, MemberCardInfoView>()
             .ForMember(view => view.HasTeam, options => options.MapFrom(dao => dao.TeamList.Any() ? 1 : 0));
        }
    }
}