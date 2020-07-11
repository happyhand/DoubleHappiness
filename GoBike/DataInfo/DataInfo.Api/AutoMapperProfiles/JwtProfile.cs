using AutoMapper;
using DataInfo.Core.Models.Dao.Member;
using DataInfo.Core.Models.Dto.Common;

namespace DataInfo.AutoMapperProfiles
{
    /// <summary>
    /// Jwt AutoMapper
    /// </summary>
    public class JwtProfile : Profile
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public JwtProfile()
        {
            CreateMap<MemberDao, JwtClaims>();
        }
    }
}