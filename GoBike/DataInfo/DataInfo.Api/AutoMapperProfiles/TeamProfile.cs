﻿using AutoMapper;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Dao.Team.Table;
using DataInfo.Core.Models.Dto.Team.Content;
using DataInfo.Core.Models.Dto.Team.Request;
using DataInfo.Core.Models.Dto.Team.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
            CreateMap<TeamActivityContent, TeamJoinOrLeaveActivityRequest>();
            CreateMap<TeamAddActivityContent, TeamUpdateActivityRequest>()
             .ForMember(request => request.ActDate, options => options.MapFrom(content => Convert.ToDateTime(content.ActDate).ToString("yyyy-MM-dd HH:mm:ss)")));
            CreateMap<TeamChangeLeaderContent, TeamChangeLeaderRequest>();
            CreateMap<TeamResponseApplyJoinContent, TeamJoinOrLeaveRequest>()
             .ForMember(request => request.Action, options => options.MapFrom(content => content.ResponseType));
            CreateMap<TeamResponseInviteJoinContent, TeamJoinOrLeaveRequest>()
             .ForMember(request => request.Action, options => options.MapFrom(content => content.ResponseType));
            CreateMap<TeamUpdateViceLeaderContent, TeamUpdateViceLeaderRequest>();
            CreateMap<TeamContent, TeamDisbandRequest>();

            CreateMap<TeamData, TeamDao>()
             .ForMember(dao => dao.CreateDate, options => options.MapFrom(table => Convert.ToDateTime(table.CreateDate)))
             .ForMember(dao => dao.TeamViceLeaderIDs, options => options.MapFrom(table => JsonConvert.DeserializeObject<IEnumerable<string>>(table.TeamViceLeaderIDs)))
             .ForMember(dao => dao.TeamMemberIDs, options => options.MapFrom(table => JsonConvert.DeserializeObject<IEnumerable<string>>(table.TeamMemberIDs)))
             .ForMember(dao => dao.ApplyJoinList, options => options.MapFrom(table => JsonConvert.DeserializeObject<IEnumerable<string>>(table.ApplyJoinList)))
             .ForMember(dao => dao.InviteJoinList, options => options.MapFrom(table => JsonConvert.DeserializeObject<IEnumerable<string>>(table.InviteJoinList)));

            CreateMap<TeamActivity, TeamActivityDao>();

            CreateMap<TeamDao, TeamDropMenuView>();
            CreateMap<TeamDao, TeamInfoView>();
            CreateMap<TeamDao, TeamSearchView>();
            CreateMap<TeamActivityDao, TeamActivityListView>();
            CreateMap<TeamActivityDao, TeamActivityDetailView>();
        }
    }
}