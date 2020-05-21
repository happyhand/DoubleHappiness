using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Enum;
using FluentValidation;

namespace DataInfo.Core.Models.Dto.Team.Content
{
    /// <summary>
    /// 瀏覽車隊內容
    /// </summary>
    public class TeamBrowseContent
    {
        /// <summary>
        /// Gets or sets 所在地
        /// </summary>
        public int County { get; set; }
    }

    /// <summary>
    /// 驗證瀏覽車隊內容
    /// </summary>
    public class TeamBrowseContentValidator : AbstractValidator<TeamBrowseContent>
    {
        public TeamBrowseContentValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(content => content.County)
             .Must(County =>
             {
                 return County != (int)CountyType.None;
             }).WithMessage(MessageHelper.Message.ResponseMessage.Member.CountyEmpty);
        }
    }
}