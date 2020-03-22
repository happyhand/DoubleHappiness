using Smtp.Service.Models;
using System.Threading.Tasks;

namespace Smtp.Service.Interfaces
{
    /// <summary>
    /// 郵件服務
    /// </summary>
    public interface ISmtpService
    {
        /// <summary>
        /// 發送郵件
        /// </summary>
        /// <param name="emailContext">emailContext</param>
        /// <returns>string</returns>
        Task<string> SendEmail(EmailContext emailContext);
    }
}