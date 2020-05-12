using AutoMapper;
using DataInfo.Core.Applibs;
using DataInfo.Core.Extensions;
using DataInfo.Core.Models.Dao;
using DataInfo.Core.Models.Dto.Common.Data;
using DataInfo.Core.Models.Dto.Response;
using DataInfo.Core.Models.Dto.User.Content;
using DataInfo.Core.Models.Dto.User.Request;
using DataInfo.Core.Models.Enum;
using DataInfo.Repository.Interfaces;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.User;
using FluentValidation.Results;
using Newtonsoft.Json;
using NLog;
using System;
using System.Threading.Tasks;

namespace DataInfo.Service.Managers.User
{
    /// <summary>
    /// 會員服務
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// jwtService
        /// </summary>
        private readonly IJwtService jwtService;

        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("MemberService");

        /// <summary>
        /// mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// userRepository
        /// </summary>
        private readonly IUserRepository userRepository;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <param name="jwtService">jwtService</param>
        /// <param name="userRepository">userRepository</param>
        public UserService(IMapper mapper, IJwtService jwtService, IUserRepository userRepository)
        {
            this.mapper = mapper;
            this.jwtService = jwtService;
            this.userRepository = userRepository;
        }

        /// <summary>
        /// 生產 Jwt Token
        /// </summary>
        /// <param name="memberDao">memberDao</param>
        /// <returns>string</returns>
        private string GenerateJwtToken(UserDao userDao)
        {
            JwtClaims jwtClaims = new JwtClaims()
            {
                UserID = userDao.Id,
                Email = userDao.Email,
                Mobile = userDao.Mobile
            };
            return this.jwtService.GenerateToken(jwtClaims);
        }

        /// <summary>
        /// 使用者登入
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Login(UserLoginContent content)
        {
            try
            {
                #region 驗證資料

                UserLoginContentValidator validator = new UserLoginContentValidator();
                ValidationResult validationResult = validator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("使用者登入結果", $"Result: 驗證失敗({errorMessgae}) Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 使用者登入

                UserDao userDao = await this.userRepository.Get(content.Email).ConfigureAwait(false);
                if (userDao == null)
                {
                    this.logger.LogWarn("使用者登入結果", $"Result: 無使用者資料 Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.Missed,
                        Content = MessageHelper.Message.ResponseMessage.Login.Fail
                    };
                }

                if (!userDao.Password.Equals(content.Password))
                {
                    this.logger.LogWarn("使用者登入結果", $"Result: 密碼錯誤 Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = MessageHelper.Message.ResponseMessage.Login.Fail
                    };
                }

                this.logger.LogWarn("使用者登入結果", $"Result: 登入成功 Content: {JsonConvert.SerializeObject(content)}", null);
                return new ResponseResult()
                {
                    Result = true,
                    ResultCode = (int)ResponseResultType.Success,
                    Content = this.GenerateJwtToken(userDao)
                };

                #endregion 使用者登入
            }
            catch (Exception ex)
            {
                this.logger.LogError("使用者登入發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Login.Error
                };
            }
        }

        /// <summary>
        /// 使用者註冊
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>ResponseResult</returns>
        public async Task<ResponseResult> Register(UserRegisterContent content)
        {
            try
            {
                #region 驗證資料

                UserRegisterContentValidator validator = new UserRegisterContentValidator();
                ValidationResult validationResult = validator.Validate(content);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("使用者註冊結果", $"Result: 驗證失敗({errorMessgae}) Content: {JsonConvert.SerializeObject(content)}", null);
                    return new ResponseResult()
                    {
                        Result = false,
                        ResultCode = (int)ResponseResultType.InputError,
                        Content = errorMessgae
                    };
                }

                #endregion 驗證資料

                #region 使用者註冊

                UserRegisterRequest reuest = this.mapper.Map<UserRegisterRequest>(content);
                UserRegisterResultType result = await this.userRepository.Create(reuest).ConfigureAwait(false);
                this.logger.LogWarn("使用者註冊結果", $"Result: {result} Content: {JsonConvert.SerializeObject(content)} Reuest: {JsonConvert.SerializeObject(reuest)}", null);
                switch (result)
                {
                    case UserRegisterResultType.MobileRepeat:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.Existed,
                            Content = MessageHelper.Message.ResponseMessage.Register.MobileExist
                        };

                    case UserRegisterResultType.EmailRepeat:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.Existed,
                            Content = MessageHelper.Message.ResponseMessage.Register.EmailExist
                        };

                    case UserRegisterResultType.Fail:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.CreateFail,
                            Content = MessageHelper.Message.ResponseMessage.Register.Fail
                        };

                    case UserRegisterResultType.Success:
                        return new ResponseResult()
                        {
                            Result = true,
                            ResultCode = (int)ResponseResultType.Success,
                            Content = MessageHelper.Message.ResponseMessage.Register.Success
                        };

                    default:
                        return new ResponseResult()
                        {
                            Result = false,
                            ResultCode = (int)ResponseResultType.UnknownError,
                            Content = MessageHelper.Message.ResponseMessage.Register.Fail
                        };
                }

                #endregion 使用者註冊
            }
            catch (Exception ex)
            {
                this.logger.LogError("使用者註冊發生錯誤", $"Content: {JsonConvert.SerializeObject(content)}", ex);
                return new ResponseResult()
                {
                    Result = false,
                    ResultCode = (int)ResponseResultType.UnknownError,
                    Content = MessageHelper.Message.ResponseMessage.Register.Error
                };
            }
        }
    }
}