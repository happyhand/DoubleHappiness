using AutoMapper;
using DataInfo.Core.Models.Dto.Post.Content;
using DataInfo.Core.Models.Dto.Ride.Request;
using Newtonsoft.Json;

namespace DataInfo.AutoMapperProfiles
{
    /// <summary>
    /// Post AutoMapper
    /// </summary>
    public class PostProfile : Profile
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public PostProfile()
        {
            #region Content To Request

            CreateMap<AddPostLikeContent, AddPostLikeRequest>();
            CreateMap<AddPostContent, AddPostRequest>()
             .ForMember(data => data.Photo, options => options.MapFrom(dto => JsonConvert.SerializeObject(dto.Photo)));
            CreateMap<UpdatePostContent, UpdatePostRequest>()
             .ForMember(data => data.Photo, options => options.MapFrom(dto => JsonConvert.SerializeObject(dto.Photo)));

            #endregion Content To Request
        }
    }
}