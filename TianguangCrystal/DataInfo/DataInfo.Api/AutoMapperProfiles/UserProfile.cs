using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Dao;
using DataInfo.Core.Models.Dao.Table;
using DataInfo.Core.Models.Dto.User.Content;
using DataInfo.Core.Models.Dto.User.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Api.AutoMapperProfiles
{
    /// <summary>
    /// User AutoMapper
    /// </summary>
    public class UserProfile : Profile
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public UserProfile()
        {
            CreateMap<UserRegisterContent, UserRegisterRequest>();
            CreateMap<UserRegisterRequest, UserData>()
            .ForMember(data => data.Password, options => options.MapFrom(request => Utility.EncryptAES(request.Password)));
            CreateMap<UserData, UserDao>()
            .ForMember(dao => dao.Password, options => options.MapFrom(data => Utility.DecryptAES(data.Password)));
        }
    }
}