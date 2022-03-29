using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Dao.Ride;
using DataInfo.Core.Models.Dao.Ride.Table;
using DataInfo.Core.Models.Dto.Ride.Content;
using DataInfo.Core.Models.Dto.Ride.Request;
using DataInfo.Core.Models.Dto.Ride.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
            #region Content To Request

            CreateMap<AddRideDataContent, AddRideInfoRequest>()
            .ForMember(data => data.ShareContent, options => options.MapFrom(dto => JsonConvert.SerializeObject(dto.ShareContent)));

            CreateMap<AddRideRouteDataContent, AddRideRouteRequest>()
            .ForMember(data => data.Route, options => options.MapFrom(dto => JsonConvert.SerializeObject(dto.Route)));

            #endregion Content To Request

            #region Table To Dao

            CreateMap<RideRecord, RideDao>()
                .ForMember(dao => dao.CreateDate, options => options.MapFrom(table => Convert.ToDateTime(table.CreateDate)));

            #endregion Table To Dao

            #region Dao To View

            CreateMap<RideDao, RideSimpleRecordView>();
            CreateMap<RideDao, RideDetailRecordView>()
                .ForMember(view => view.Photo, options => options.MapFrom(dao => Utility.GetRideImageCdn(dao.Photo)))
                .ForMember(view => view.ShareContent, options => options.MapFrom(dao => this.ShareContentHandler(dao.ShareContent)));
            CreateMap<RideRouteDao, RideRouteView>();

            #endregion Dao To View
        }

        /// <summary>
        /// 騎乘分享內容處理
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>string list</returns>
        private IEnumerable<IEnumerable<string>> ShareContentHandler(string data)
        {
            List<List<string>> shareContent = JsonConvert.DeserializeObject<List<List<string>>>(data);
            foreach (List<string> content in shareContent)
            {
                content[1] = Utility.GetRideImageCdn(content[1]);
            }

            return shareContent;
        }
    }
}