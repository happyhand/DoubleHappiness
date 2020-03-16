using System;
using System.Collections.Generic;
using System.Text;

namespace UploadFiles.Core.Applibs
{
    /// <summary>
    /// APP 設定資料
    /// </summary>
    public class AppSettingHelper
    {
        /// <summary>
        /// Appsetting
        /// </summary>
        public static AppSettingHelper Appsetting;

        /// <summary>
        /// Gets or sets CdnPath
        /// </summary>
        public string CdnPath { get; set; }
    }
}