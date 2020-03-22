using FluentValidation.Results;
using Newtonsoft.Json;
using NLog;
using Smtp.Core.Applibs;
using Smtp.Core.Extensions;
using Smtp.Service.Interfaces;
using Smtp.Service.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Smtp.Service.Managers
{
    /// <summary>
    /// 郵件服務
    /// </summary>
    public class SmtpService : ISmtpService
    {
        /// <summary>
        /// logger
        /// </summary>
        private readonly ILogger logger = LogManager.GetLogger("SmtpService");

        /// <summary>
        /// 發送郵件
        /// </summary>
        /// <param name="emailContext">emailContext</param>
        /// <returns>string</returns>
        public async Task<string> SendEmail(EmailContext emailContext)
        {
            try
            {
                #region 驗證郵件資料

                EmailContextValidator emailContextValidator = new EmailContextValidator();
                ValidationResult validationResult = emailContextValidator.Validate(emailContext);
                if (!validationResult.IsValid)
                {
                    string errorMessgae = validationResult.Errors[0].ErrorMessage;
                    this.logger.LogWarn("發送郵件結果", $"Result: 驗證失敗({errorMessgae}) EmailContext: {JsonConvert.SerializeObject(emailContext)}", null);
                    return errorMessgae;
                }

                #endregion 驗證郵件資料

                using (MailMessage message = new MailMessage())
                {
                    message.To.Add(new MailAddress(emailContext.Address));
                    message.From = new MailAddress(AppSettingHelper.Appsetting.SmtpConfig.SmtpMail, AppSettingHelper.Appsetting.SmtpConfig.SmtpUser);
                    message.Subject = emailContext.Subject;
                    message.Body = emailContext.Body;
                    message.IsBodyHtml = true;

                    using (SmtpClient client = new SmtpClient(AppSettingHelper.Appsetting.SmtpConfig.SmtpServer))
                    {
                        client.Port = 587;//// Google Port 587
                        client.Credentials = new NetworkCredential(AppSettingHelper.Appsetting.SmtpConfig.SmtpMail, AppSettingHelper.Appsetting.SmtpConfig.SmtpPassword);
                        client.EnableSsl = true;
                        await client.SendMailAsync(message);
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                this.logger.LogError("發送郵件發生錯誤", $"EmailContext: {JsonConvert.SerializeObject(emailContext)}", ex);
                return "發送郵件發生錯誤.";
            }
        }
    }
}