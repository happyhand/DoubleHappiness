using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Repository.Interfaces;
using DataInfo.Repository.Models.Member;
using DataInfo.Service.Enums;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Service.Models.Member.Content;
using DataInfo.Service.Models.Member.View;
using DataInfo.Service.Models.Response;
using FluentValidation.Results;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.Member
{
    /// <summary>
    /// 騎乘服務
    /// </summary>
    public class RideService : IRideService
    {
        /// <summary>
        /// uploadService
        /// </summary>
        private readonly IUploadService uploadService;

        /// <summary>
        /// logger
        /// </summary>
        protected readonly ILogger logger = LogManager.GetLogger("RideService");

        /// <summary>
        /// mapper
        /// </summary>
        protected readonly IMapper mapper;

        /// <summary>
        /// rideRepository
        /// </summary>
        protected readonly IRideRepository rideRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="redisRepository">redisRepository</param>
        public RideService(IMapper mapper, IUploadService uploadService, IRideRepository rideRepository)
        {
            this.mapper = mapper;
            this.uploadService = uploadService;
            this.rideRepository = rideRepository;
        }

        #region 騎乘資料

        /// <summary>
        /// 騎乘資料更新處理
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="rideModel">rideModel</param>
        /// <returns>string</returns>
        private async Task<string> UpdateInfoHandler(RideUpdateInfoContent content, RideModel rideModel)
        {
            if (content.CountyID != (int)CityType.None)
            {
                rideModel.CountyID = content.CountyID;
            }

            if (content.Level != (int)RideLevelType.None)
            {
                rideModel.Level = content.Level;
            }

            if (content.Route != null && content.Route.Any())
            {
                rideModel.Route = JsonConvert.SerializeObject(content.Route);
            }

            if (content.ShareContent != null || !string.IsNullOrEmpty(content.Photo))
            {
                List<string> imgBase64s = new List<string>();
                if (content.ShareContent != null)
                {
                    imgBase64s.AddRange(content.ShareContent.Select(data => data.ElementAt(1)));
                }

                if (!string.IsNullOrEmpty(content.Photo))
                {
                    imgBase64s.Add(content.Photo);
                }

                ResponseResultDto uploadResponseResult = await this.uploadService.UploadImages(imgBase64s).ConfigureAwait(false);
                if (!uploadResponseResult.Result)
                {
                    return "上傳圖片失敗.";
                }

                IEnumerable<string> imgUrls = uploadResponseResult.Content;
                if (content.ShareContent != null && content.ShareContent.Any())
                {
                    content.ShareContent = content.ShareContent.Select((data, index) =>
                    {
                        string imgUrl = imgUrls.ElementAt(index);
                        return new List<string>()
                        {
                            data.ElementAt(0),
                            imgUrls.ElementAt(index)
                        };
                    });
                    rideModel.ShareContent = JsonConvert.SerializeObject(content.ShareContent);
                }

                if (!string.IsNullOrEmpty(content.Photo))
                {
                    rideModel.Photo = imgUrls.LastOrDefault();
                }
            }

            if (!string.IsNullOrEmpty(content.Title))
            {
                rideModel.Title = content.Title;
            }

            return string.Empty;
        }

        /// <summary>
        /// 新增騎乘資料
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <param name="content">content</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> AddRideData(string memberID, RideInfoContent content)
        {
            try
            {
                #region 驗證騎乘資料

                RideInfoContentValidator rideInfoContentValidator = new RideInfoContentValidator();
                ValidationResult validationResult = rideInfoContentValidator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("新增騎乘資料結果", $"Result: 驗證失敗({errorMessgae}) MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                if (content.ShareContent == null)
                {
                    content.ShareContent = new string[][] { };
                }

                if (string.IsNullOrEmpty(content.Title))
                {
                    content.Title = $"{DateTime.UtcNow:yyyy/MM/dd}";
                }

                #endregion 驗證騎乘資料

                #region 上傳圖片

                List<string> imgBase64s = content.ShareContent.Select(data => data.ElementAt(1)).ToList();
                imgBase64s.Add(content.Photo);
                ResponseResultDto uploadResponseResult = await this.uploadService.UploadImages(imgBase64s).ConfigureAwait(false);
                if (!uploadResponseResult.Result)
                {
                    this.logger.LogWarn("新增騎乘資料結果", $"Result: 上傳圖片失敗 MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "上傳圖片失敗."
                    };
                }

                IEnumerable<string> imgUrls = uploadResponseResult.Content;
                content.ShareContent = content.ShareContent.Select((data, index) =>
                {
                    string imgUrl = imgUrls.ElementAt(index);
                    if (string.IsNullOrEmpty(imgUrl))
                    {
                        this.logger.LogWarn("新增騎乘資料結果", $"Result: 騎乘分享內容圖片轉換失敗 MemberID: {memberID} Data: {JsonConvert.SerializeObject(data)}", null);
                    }

                    return new List<string>()
                    {
                        data.ElementAt(0),
                        imgUrls.ElementAt(index)
                    };
                });
                content.Photo = imgUrls.LastOrDefault();

                #endregion 上傳圖片

                #region 新增資料

                RideModel rideModel = this.mapper.Map<RideModel>(content);
                DateTime createDate = DateTime.UtcNow;
                rideModel.MemberID = memberID;
                rideModel.CreateDate = createDate;
                rideModel.RideID = Utility.GetSerialID(createDate);
                bool isSuccess = await this.rideRepository.Create(rideModel).ConfigureAwait(false);
                this.logger.LogInfo("新增騎乘資料結果", $"Result: {isSuccess} MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)} RideModel: {JsonConvert.SerializeObject(rideModel)}", null);
                if (!isSuccess)
                {
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.CreateFail,
                        Content = "新增資料失敗."
                    };
                }

                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = "新增資料成功."
                };

                #endregion 新增資料
            }
            catch (Exception ex)
            {
                this.logger.LogError("新增騎乘資料發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "新增資料發生錯誤."
                };
            }
        }

        /// <summary>
        /// 取得騎乘資料
        /// </summary>
        /// <param name="rideID">rideID</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> GetRideData(string rideID)
        {
            try
            {
                if (string.IsNullOrEmpty(rideID))
                {
                    this.logger.LogWarn("取得騎乘資料失敗", $"Result: 騎乘編號無效", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        Content = "騎乘編號無效."
                    };
                }

                RideModel rideModel = await this.rideRepository.Get(rideID);
                if (rideModel == null)
                {
                    this.logger.LogWarn("取得騎乘資料失敗", $"Result: 無騎乘資料 RideID: {rideID}", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        Content = "無騎乘資料."
                    };
                }

                return new ResponseResultDto()
                {
                    Result = true,
                    Content = this.mapper.Map<RideInfoViewDto>(rideModel) //// TODO 待轉換客端所需資料
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得騎乘資料發生錯誤", $"RideID: {rideID}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    Content = "取得資料發生錯誤."
                };
            }
        }

        /// <summary>
        /// 取得會員的騎乘資料列表
        /// </summary>
        /// <param name="memberID">memberID</param>
        /// <returns>ResponseResultDto</returns>
        public async Task<ResponseResultDto> GetRideDataListOfMember(string memberID)
        {
            try
            {
                if (string.IsNullOrEmpty(memberID))
                {
                    this.logger.LogWarn("取得會員的騎乘資料列表失敗", $"Result: 會員編號無效", null);
                    return new ResponseResultDto()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = "會員編號無效."
                    };
                }

                IEnumerable<RideModel> rideModels = await this.rideRepository.GetListOfMember(memberID);
                return new ResponseResultDto()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = this.mapper.Map<IEnumerable<RideInfoViewDto>>(rideModels) //// TODO 待轉換客端所需資料
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError("取得會員的騎乘資料列表發生錯誤", $"MemberID: {memberID}", ex);
                return new ResponseResultDto()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = "取得資料發生錯誤."
                };
            }
        }

        ///// <summary>
        ///// 更新騎乘資料 (TODO)
        ///// </summary>
        ///// <param name="memberID">memberID</param>
        ///// <param name="content">content</param>
        ///// <returns>ResponseResultDto</returns>
        //public async Task<ResponseResultDto> UpdateRideData(string memberID, RideUpdateInfoContent content)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(memberID))
        //        {
        //            return new ResponseResultDto()
        //            {
        //                Result = false,
        //                ResultCode = (int)ResponseResultType.InputError,
        //                Content = "會員編號無效."
        //            };
        //        }

        // if (string.IsNullOrEmpty(content.RideID)) { return new ResponseResultDto() { Result =
        // false, ResultCode = (int)ResponseResultType.InputError, Content = "騎乘資料編號無效." }; }

        // RideModel rideModel = await
        // this.rideRepository.Get(content.RideID).ConfigureAwait(false); if (rideModel == null) {
        // this.logger.LogWarn("更新騎乘資料結果", $"Result: 搜尋失敗，無騎乘資料 MemberID: {memberID} Content:
        // {JsonConvert.SerializeObject(content)}", null); return new ResponseResultDto() { Result =
        // false, ResultCode = (int)ResponseResultType.InputError, Content = "無騎乘資料." }; }

        // string updateInfoHandlerResult = await this.UpdateInfoHandler(content,
        // rideModel).ConfigureAwait(false); if (!string.IsNullOrEmpty(updateInfoHandlerResult)) {
        // this.logger.LogWarn("更新騎乘資料結果", $"Result: 更新失敗({updateInfoHandlerResult}) MemberID:
        // {memberID} Content: {JsonConvert.SerializeObject(content)}", null); return new
        // ResponseResultDto() { Result = false, ResultCode = (int)ResponseResultType.UpdateFail,
        // Content = updateInfoHandlerResult }; }

        // bool isSuccess = await this.rideRepository.Update(rideModel).ConfigureAwait(false);
        // this.logger.LogInfo("更新騎乘資料結果", $"Result: {isSuccess} MemberID: {memberID} Content:
        // {JsonConvert.SerializeObject(content)} RideModel:
        // {JsonConvert.SerializeObject(rideModel)}", null); if (!isSuccess) { return new
        // ResponseResultDto() { Result = false, ResultCode = (int)ResponseResultType.UpdateFail,
        // Content = "更新資料失敗." }; }

        //        return new ResponseResultDto()
        //        {
        //            Result = true,
        //            ResultCode = (int)ResponseResultType.Success,
        //            Content = "更新資料成功."
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogError("更新騎乘資料結果發生錯誤", $"MemberID: {memberID} Content: {JsonConvert.SerializeObject(content)}", ex);
        //        return new ResponseResultDto()
        //        {
        //            Result = false,
        //            ResultCode = (int)ResponseResultType.UnknownError,
        //            Content = "更新資料發生錯誤."
        //        };
        //    }
        //}

        #endregion 騎乘資料
    }
}