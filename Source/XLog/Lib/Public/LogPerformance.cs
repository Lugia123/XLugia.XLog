using XLugia.Lib.XLog.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLugia.Lib.XLog
{
    /// <summary>
    /// 日志性能信息
    /// </summary>
    public class LogPerformance
    {
        /// <summary>
        /// 每次日志保存数量
        /// </summary>
        public static int saveNumberPerTime = 0;

        /// <summary>
        /// 每次日志保存耗时（毫秒）
        /// </summary>
        public static double useMillisecondsPerTime = 0;

        /// <summary>
        /// 日志队列总长度
        /// </summary>
        public static double logQueueCount
        {
            get
            {
                try
                {
                    return LogDictionaryQueue.getIns().allCount();
                }
                catch (Exception ex)
                {
                    SimpleLogController.writeErrorLog(ex, "LogPerformance get logQueueCount");
                    return 0;
                }
            }
        }

        /// <summary>
        /// 每次日志最大保存数（文件型），默认20000
        /// </summary>
        public static int maxProcessCountPerTimeByFileType = 20000;

        /// <summary>
        /// 每次日志最大保存数（SQLServer），默认2000
        /// </summary>
        internal static int maxProcessCountPerTimeBySQLServerType = 2000;

        /// <summary>
        /// 每次日志最大保存数（SQLCompact），默认2000
        /// </summary>
        internal static int maxProcessCountPerTimeBySQLCompactType = 2000;

        /// <summary>
        /// 每次日志最大保存数（MySQL），默认2000
        /// </summary>
        internal static int maxProcessCountPerTimeByMySQLType = 2000;
    }
}
