﻿using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DataInfo.Core.Models.Dto.Ride.Content
{
    /// <summary>
    /// 更新組隊騎乘內容
    /// </summary>
    public class UpdateRideGroupContent
    {
        /// <summary>
        /// Gets or sets 會員 ID 列表
        /// </summary>
        public IEnumerable<string> MemberIDs { get; set; }
    }

    /// <summary>
    /// 驗證更新組隊騎乘內容
    /// </summary>
    public class UpdateRideGroupContentValidator : AbstractValidator<UpdateRideGroupContent>
    {
        /// <summary>
        /// 建構式
        /// </summary>
        public UpdateRideGroupContentValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.MemberIDs)
              .NotNull().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.MemberIDEmpty}";
              })
              .NotEmpty().WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.MemberIDEmpty}";
              })
              .Must(memberIDs =>
              {
                  return memberIDs.Count() <= AppSettingHelper.Appsetting.Rule.MaxGroupCount;
              }).WithMessage(content =>
              {
                  return $"{ResponseErrorMessageType.ExceedMaxPeople}|MemberIDs: {JsonConvert.SerializeObject(content.MemberIDs)}";
              });
        }
    }
}