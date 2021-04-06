using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Member.Request;
using DataInfo.Core.Models.Dto.Member.View;
using DataInfo.Core.Models.Enum;
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
            #region Content To Content

            CreateMap<MemberForgetPasswordContent, MemberUpdatePasswordContent>()
             .ForMember(content => content.NewPassword, options => options.MapFrom(content => content.Password));

            #endregion Content To Content

            #region Content To Request

            CreateMap<MemberLoginContent, MemberLoginRequest>();

            #endregion Content To Request

            #region Dao To View

            CreateMap<MemberDao, MemberSimpleInfoView>()
             .ForMember(view => view.Avatar, options => options.MapFrom(dao => Utility.GetMemberImageCdn(dao.Avatar)))
             .ForMember(view => view.Nickname, options => options.MapFrom(dao => string.IsNullOrEmpty(dao.Nickname) ? dao.MemberID : dao.Nickname));
            CreateMap<MemberDao, MemberDetailInfoView>()
             .ForMember(view => view.HasMobile, options => options.MapFrom(dao => string.IsNullOrEmpty(dao.Mobile) ? (int)BindMobileStatusType.None : (int)BindMobileStatusType.Bind))
             .ForMember(view => view.FrontCover, options => options.MapFrom(dao => Utility.GetMemberImageCdn(dao.FrontCover)))
             .ForMember(view => view.Nickname, options => options.MapFrom(dao => string.IsNullOrEmpty(dao.Nickname) ? dao.MemberID : dao.Nickname));

            CreateMap<MemberDao, MemberHomeInfoView>()
             .ForMember(view => view.Avatar, options => options.MapFrom(dao => Utility.GetMemberImageCdn(dao.Avatar)))
             .ForMember(view => view.Photo, options => options.MapFrom(dao => Utility.GetMemberImageCdn(dao.Photo)))
             .ForMember(view => view.Nickname, options => options.MapFrom(dao => string.IsNullOrEmpty(dao.Nickname) ? dao.MemberID : dao.Nickname));
            CreateMap<MemberDao, MemberCardInfoView>()
             .ForMember(view => view.HasTeam, options => options.MapFrom(dao => dao.TeamList.Any() ? (int)JoinStatusType.Join : (int)JoinStatusType.None))
             .ForMember(view => view.Avatar, options => options.MapFrom(dao => Utility.GetMemberImageCdn(dao.Avatar)))
             .ForMember(view => view.FrontCover, options => options.MapFrom(dao => Utility.GetMemberImageCdn(dao.FrontCover)))
             .ForMember(view => view.Nickname, options => options.MapFrom(dao => string.IsNullOrEmpty(dao.Nickname) ? dao.MemberID : dao.Nickname));

            #endregion Dao To View
        }
    }
}