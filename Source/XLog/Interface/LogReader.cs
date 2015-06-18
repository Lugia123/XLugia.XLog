using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Collections.Concurrent;
using XLugia.Lib.XLog.IO;
using System.Threading;
using System.Threading.Tasks;
using XLugia.Lib.XLog.Lib;

namespace XLugia.Lib.XLog
{
    /// <summary>
    /// 日志读取器
    /// </summary>
    public class LogReader
    {
        private static LogReader instance = new LogReader();

        /// <summary>
        /// 获取日志读取器静态实例。
        /// </summary>
        /// <returns>日志读取器。</returns>
        public static LogReader getIns() { return instance; }

        /// <summary>
        /// 读取日志。
        /// </summary>
        /// <param name="logFilePath">日志文件路径。</param>
        /// <param name="logSearchFilterModel">LogSearchFilterModel检索条件对象。</param>
        /// <returns>LogSearchResultModel检索结果对象。</returns>
        public LogSearchResultModel readLog(string logFilePath, LogSearchFilterModel logSearchFilterModel)
        {
            LogSearchResultModel logSearchResultModel = new LogSearchResultModel();
            try
            {
                logSearchResultModel = FileReader.getIns().readLog(logFilePath, logSearchFilterModel);
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogReader readLog");
            }
            return logSearchResultModel;
        }
    }
}
