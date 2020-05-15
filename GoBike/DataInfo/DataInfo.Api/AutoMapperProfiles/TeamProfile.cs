using AutoMapper;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Dao.Team.Table;
using DataInfo.Core.Models.Dto.Team.Content;
using DataInfo.Core.Models.Dto.Team.Request;
using DataInfo.Core.Models.Dto.Team.View;
using Newtonsoft.Json;

namespace DataInfo.AutoMapperProfiles
{
    /// <summary>
    /// Team AutoMapper
    /// </summary>
    public class TeamProfile : Profile
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public TeamProfile()
        {
            CreateMap<TeamCreateContent, TeamCreateRequest>();

            CreateMap<TeamData, TeamDao>()
             .ForMember(dao => dao.TeamViceLeaderIDs, options => options.MapFrom(data => JsonConvert.DeserializeObject<TeamViceLeaderIDListDao>(data.TeamViceLeaderIDs).ViceLeader))
             .ForMember(dao => dao.TeamMemberIDs, options => options.MapFrom(data => JsonConvert.DeserializeObject<TeamMemberIDListDao>(data.TeamMemberIDs).Member))
             .ForMember(dao => dao.ApplyJoinList, options => options.MapFrom(data => JsonConvert.DeserializeObject<TeamApplyJoinListDao>(data.ApplyJoinList).Apply))
             .ForMember(dao => dao.InviteJoinList, options => options.MapFrom(data => JsonConvert.DeserializeObject<TeamInviteJoinListDao>(data.InviteJoinList).Invite));

            CreateMap<TeamDao, TeamDropMenuView>();
        }
    }
}