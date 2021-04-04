using AutoMapper;
using DataInfo.Core.Models.Dao.Ride;
using DataInfo.Core.Models.Dao.Ride.Table;
using DataInfo.Core.Models.Dto.Ride.Content;
using DataInfo.Core.Models.Dto.Ride.Request;
using DataInfo.Core.Models.Dto.Ride.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
            .ForMember(data => data.ShareContent, options => options.MapFrom(dto => JsonConvert.SerializeObject(dto.ShareContent)))
            .ForMember(data => data.Route, options => options.MapFrom(dto => JsonConvert.SerializeObject(dto.Route)));

            #endregion Content To Request

            #region Table To Dao

            CreateMap<RideRecord, RideDao>()
                .ForMember(dao => dao.CreateDate, options => options.MapFrom(table => Convert.ToDateTime(table.CreateDate)));

            #endregion Table To Dao

            #region Dao To View

            CreateMap<RideDao, RideSimpleRecordView>();
            CreateMap<RideDao, RideDetailRecordView>()
                .ForMember(view => view.Route, options => options.MapFrom(dao => JsonConvert.DeserializeObject<IEnumerable<IEnumerable<string>>>(dao.Route)))
                .ForMember(view => view.ShareContent, options => options.MapFrom(dao => JsonConvert.DeserializeObject<IEnumerable<IEnumerable<string>>>(dao.ShareContent)));

            #endregion Dao To View
        }
    }
}