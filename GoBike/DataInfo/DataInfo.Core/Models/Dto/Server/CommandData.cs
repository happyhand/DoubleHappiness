namespace DataInfo.Core.Models.Dto.Server
{
    /// <summary>
    /// 後端指令資料
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommandData<T>
    {
        /// <summary>
        /// Gets or sets CmdID
        /// </summary>
        public int CmdID { get; set; }

        /// <summary>
        /// Gets or sets Data
        /// </summary>
        public T Data { get; set; }
    }
}