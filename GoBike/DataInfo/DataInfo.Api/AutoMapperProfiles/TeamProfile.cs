using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Dao.Team;
using DataInfo.Core.Models.Dao.Team.Table;
using DataInfo.Core.Models.Dto.Team.Content;
using DataInfo.Core.Models.Dto.Team.Content.data;
using DataInfo.Core.Models.Dto.Team.Request;
using DataInfo.Core.Models.Dto.Team.View;
using DataInfo.Core.Models.Dto.Team.View.data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
            #region Content To Request

            CreateMap<TeamCreateContent, TeamCreateRequest>();
            CreateMap<TeamActivityContent, TeamJoinOrLeaveActivityRequest>();
            CreateMap<TeamAddActivityContent, TeamUpdateActivityRequest>()
             .ForMember(request => request.Route, options => options.MapFrom(content => JsonConvert.SerializeObject(content.Routes)))
             .ForMember(request => request.ActDate, options => options.MapFrom(content => Convert.ToDateTime(content.ActDate).ToString("yyyy-MM-dd")))
             .ForMember(request => request.MeetTime, options => options.MapFrom(content => Convert.ToDateTime(content.MeetTime).ToString("HH:mm:ss")))
             .ForMember(request => request.TotalDistance, options => options.MapFrom(content => JsonConvert.SerializeObject(content.TotalDistance)))
             .ForMember(request => request.LoadMap, options => options.MapFrom(content => this.LoadMapJsonHandler(content.LoadMap)));
            CreateMap<TeamChangeLeaderContent, TeamChangeLeaderRequest>();
            CreateMap<TeamResponseApplyJoinContent, TeamJoinOrLeaveRequest>()
             .ForMember(request => request.Action, options => options.MapFrom(content => content.ResponseType));
            CreateMap<TeamUpdateViceLeaderContent, TeamUpdateViceLeaderRequest>();
            CreateMap<TeamContent, TeamDisbandRequest>();
            CreateMap<TeamBulletinContent, TeamUpdateBulletinRequest>();
            CreateMap<TeamAddBulletinContent, TeamUpdateBulletinRequest>();

            #endregion Content To Request

            #region Table To Dao

            CreateMap<TeamData, TeamDao>()
             .ForMember(dao => dao.CreateDate, options => options.MapFrom(table => Convert.ToDateTime(table.CreateDate)))
             .ForMember(dao => dao.TeamViceLeaderIDs, options => options.MapFrom(table => JsonConvert.DeserializeObject<IEnumerable<string>>(table.TeamViceLeaderIDs)))
             .ForMember(dao => dao.TeamMemberIDs, options => options.MapFrom(table => JsonConvert.DeserializeObject<IEnumerable<string>>(table.TeamMemberIDs)))
             .ForMember(dao => dao.ApplyJoinList, options => options.MapFrom(table => JsonConvert.DeserializeObject<IEnumerable<string>>(table.ApplyJoinList)));

            #endregion Table To Dao

            #region Dao To View

            CreateMap<TeamDao, TeamDropMenuView>()
                .ForMember(view => view.Avatar, options => options.MapFrom(dao => Utility.GetTeamImageCdn(dao.Avatar)));
            CreateMap<TeamDao, TeamInfoView>()
                .ForMember(view => view.Avatar, options => options.MapFrom(dao => Utility.GetTeamImageCdn(dao.Avatar)))
                .ForMember(view => view.FrontCover, options => options.MapFrom(dao => Utility.GetTeamImageCdn(dao.FrontCover)));
            CreateMap<TeamDao, TeamSettingView>()
                .ForMember(view => view.Avatar, options => options.MapFrom(dao => Utility.GetTeamImageCdn(dao.Avatar)))
                .ForMember(view => view.FrontCover, options => options.MapFrom(dao => Utility.GetTeamImageCdn(dao.FrontCover)));
            CreateMap<TeamDao, TeamSearchView>()
                .ForMember(view => view.Avatar, options => options.MapFrom(dao => Utility.GetTeamImageCdn(dao.Avatar)));
            CreateMap<TeamActivityDao, TeamActivityListView>()
                .ForMember(view => view.FounderAvatar, options => options.MapFrom(dao => Utility.GetMemberImageCdn(dao.FounderAvatar)))
                .ForMember(view => view.TotalDistance, options => options.MapFrom(dao => JsonConvert.DeserializeObject<IEnumerable<float>>(dao.TotalDistance)))
                .ForMember(view => view.LoadMap, options => options.MapFrom(dao => this.LoadMapViewHandler(dao.LoadMap)));
            CreateMap<TeamActivityDao, TeamActivityDetailView>()
                .ForMember(view => view.FounderAvatar, options => options.MapFrom(dao => Utility.GetMemberImageCdn(dao.FounderAvatar)))
                .ForMember(view => view.Routes, options => options.MapFrom(dao => this.RouteHandler(dao.Route)))
                .ForMember(view => view.TotalDistance, options => options.MapFrom(dao => JsonConvert.DeserializeObject<IEnumerable<float>>(dao.TotalDistance)))
                .ForMember(view => view.LoadMap, options => options.MapFrom(dao => this.LoadMapViewHandler(dao.LoadMap)));
            CreateMap<TeamBulletinDao, TeamBullentiListView>()
                .ForMember(view => view.Avatar, options => options.MapFrom(dao => Utility.GetMemberImageCdn(dao.Avatar)));

            #endregion Dao To View
        }

        /// <summary>
        /// 道路線圖資料 json 處理
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>string</returns>
        private string LoadMapJsonHandler(IEnumerable<IEnumerable<LoadMap>> data)
        {
            IEnumerable<IEnumerable<List<float>>> transformData = data.Select(item => item.Select(subItem => new List<float>() { subItem.Latitude, subItem.Longitude }));
            return JsonConvert.SerializeObject(transformData);
        }

        /// <summary>
        /// 道路線圖資料可視資料處理
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>string</returns>
        private IEnumerable<IEnumerable<LoadMapView>> LoadMapViewHandler(string json)
        {
            IEnumerable<IEnumerable<List<float>>> data = JsonConvert.DeserializeObject<IEnumerable<IEnumerable<List<float>>>>(json);
            return data.Select(item => item.Select(subItem => new LoadMapView() { Latitude = subItem[0], Longitude = subItem[1] }));
        }

        /// <summary>
        /// 路線資料可視資料處理
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>string list</returns>
        private IEnumerable<RouteView> RouteHandler(string data)
        {
            IEnumerable<RouteView> route = JsonConvert.DeserializeObject<IEnumerable<RouteView>>(data);
            foreach (RouteView view in route)
            {
                view.Photo = Utility.GetTeamActivityImageCdn(view.Photo);
            }

            return route;
        }
    }
}