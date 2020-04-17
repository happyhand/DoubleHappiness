namespace DataInfo.Core.Models.Dto.Response
{
    /// <summary>
    /// 回覆結果類別資料
    /// </summary>
    public enum ResponseResultType
    {
        /// <summary>
        /// 未知錯誤
        /// </summary>
        UnknownError = 900,

        /// <summary>
        /// 輸入錯誤
        /// </summary>
        InputError = 901,

        /// <summary>
        /// 已存在
        /// </summary>
        Existed = 902,

        /// <summary>
        /// 創建失敗
        /// </summary>
        CreateFail = 903,

        /// <summary>
        /// 更新失敗
        /// </summary>
        UpdateFail = 904,

        /// <summary>
        /// 刪除失敗
        /// </summary>
        DeleteFail = 905,

        /// <summary>
        /// 拒絕訪問
        /// </summary>
        DenyAccess = 906,

        /// <summary>
        /// 不存在
        /// </summary>
        Missed = 907,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 1000
    }

    /// <summary>
    /// 回應資料
    /// </summary>
    public class ResponseResultDto
    {
        /// <summary>
        /// Gets or sets Content
        /// </summary>
        public dynamic Content { get; set; }

        /// <summary>
        /// Gets or sets Result
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// Gets or sets ResultCode
        /// </summary>
        public int ResultCode { get; set; }
    }
}