using CommonUtility.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using Tools.Config;

namespace Tools.Saying
{
    class SayingManager
    {
        static Logger mLogger = new Logger(MethodBase.GetCurrentMethod().DeclaringType);

        #region DB表名列名
        public static readonly string SayingsTableName = "Sayings";
        public static readonly string IndexTableName = "SayingsIndex";
        public static readonly string SayingsTableIdColumn = "Id";
        public static readonly string SayingsTableSayingColumn = "Saying";
        public static readonly string IndexTableIdColumn = "Id";
        #endregion

        static ConfigurationSaying mConfig;
        static bool mConfigValuable = false;
        // 数据库是否可用
        static bool mDBValuable = false;
        // 数据库中缓存的名言条数
        static int mSayingsCount;
        // 数据库中保存的已经显示的名言条数
        static int mGlobalIndex = 0;
        static int mCacheIndex = 0;
        static List<string> mSayingCache;
        static readonly string mConnectionString;
        static readonly string DefaultSaying = "脚下的路，如果不是你自己的选择，那旅程的终点在哪儿，也没人知道";

        static SayingManager()
        {
            mConfig = ToolsConfig.GetInstance().Saying;
            mConfigValuable = Validate(mConfig);
            if (mConfigValuable)
            {
                string connectionString = mConfig.CacheDB.ConnectionString;
                if (!string.IsNullOrEmpty(connectionString))
                {
                    try
                    {
                        if (DataReader.EnsureDBSchemaAsync(connectionString).Result)
                        {
                            mGlobalIndex = DataReader.QuerySayingsIndexFromDB(connectionString);
                            mSayingsCount = DataReader.QuerySayingsCountFromDB(connectionString);
                            mConnectionString = connectionString;
                            mDBValuable = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        mLogger.Error($"Error while validating DB schema:{connectionString}, due to:{ex}");
                    }
                }
            }
        }

        private static bool Validate(ConfigurationSaying mConfig)
        {
            if (mConfig != null && !string.IsNullOrEmpty(mConfig.Url) &&
                !string.IsNullOrEmpty(mConfig.Param) &&
                !string.IsNullOrEmpty(mConfig.XPath))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// ==================
        /// 获取下一条名言名句
        /// ==================
        /// 1、从Web获取名句（50条/页），缓存到内存与数据库，之后从缓存读取
        /// 2、所有名句都缓存到数据库之后，从第一条开始显示，之后从数据库读取
        /// 3、名句条数通过配置文件配置
        /// 4、一个全局index，指示当前显示名言在所有名句的位置
        /// 5、一个局部index，指示当前显示名言在内存中的位置
        /// 6、在退出时将全局index写入数据库
        /// </summary>
        /// <returns></returns>
        internal static string GetNextSaying()
        {
            if (mConfigValuable)
            {
                if (++mGlobalIndex > mConfig.TotleItems)
                {
                    mGlobalIndex = 0;
                }
                if (mSayingCache == null && mDBValuable)
                {
                    // DB中已经缓存，从DB中取
                    if (mGlobalIndex < mSayingsCount)
                    {
                        try
                        {
                            var saying = DataReader.GetSayingFromDBAsync(mConnectionString, mGlobalIndex);
                            if (saying != null && !string.IsNullOrEmpty(saying.Result))
                            {
                                return saying.Result;
                            }
                        }
                        catch (Exception ex)
                        {
                            mLogger.Error($"Error while getting saying from DB:{mConnectionString}, index:{mGlobalIndex}, due to:{ex}");
                            mDBValuable = false;
                        }
                        return DefaultSaying;
                    }
                }
                if (mSayingCache == null)
                {
                    // 从Web获取名言名句
                    var page = mGlobalIndex / 50 + 1;
                    var url = mConfig.Url + string.Format(mConfig.Param, page);
                    mSayingCache = DataReader.GetSayingsFromUrl(url, mConfig.XPath);

                    // 如果DB可用，将名句缓存到DB中
                    if (mDBValuable)
                    {
                        try
                        {
                            var count = DataWriter.UpdateSayingsToDBAsync(mConnectionString, mSayingCache).Result;
                            mSayingsCount += count;
                        }
                        catch (Exception ex)
                        {
                            mLogger.Error($"Error while updating sayings to DB:{mConnectionString}, due to:{ex}");
                            mDBValuable = false;
                        }
                    }
                }
                if (mCacheIndex >= mSayingCache.Count)
                {
                    mCacheIndex = 0;
                    mSayingCache = null;
                }
                if (mSayingCache == null || mSayingCache.Count == 0)
                {
                    return DefaultSaying;
                }

                return mSayingCache[mCacheIndex++];
            }

            return DefaultSaying;
        }

        public static bool UpdateSayingsIndex()
        {
            try
            {
                if (mDBValuable)
                {
                    return DataWriter.UpdateSayingsIndexToDBAsync(mConnectionString, mGlobalIndex).Result;
                }
                return false;
            }
            catch (Exception ex)
            {
                mLogger.Error($"Error while updating sayings index:{mGlobalIndex} to DB:{mConnectionString}, due to:{ex}");
                mDBValuable = false;
                return false;
            }
        }
    }
}
