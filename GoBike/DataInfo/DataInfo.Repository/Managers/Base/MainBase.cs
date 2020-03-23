using DataInfo.Core.Applibs;
using SqlSugar;

namespace DataInfo.Repository.Managers.Base
{
    /// <summary>
    /// MainBase SqlSugar 設定
    /// </summary>
    public class MainBase
    {
        /// <summary>
        /// Db
        /// </summary>
        protected readonly SqlSugarClient Db;

        /// <summary>
        /// 建構式
        /// </summary>
        public MainBase()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = AppSettingHelper.Appsetting.Sql.ConnectionStrings,
                DbType = DbType.SqlServer,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
            });
        }

        ///// <summary>
        ///// Gets Member 資料表
        ///// </summary>
        //protected SimpleClient<Member> MemberTable { get { return new SimpleClient<Member>(Db); } }

        ///// <summary>
        ///// Gets Ride 資料表
        ///// </summary>
        //protected SimpleClient<Ride> RideTable { get { return new SimpleClient<Ride>(Db); } }
    }
}