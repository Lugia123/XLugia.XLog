using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLugia.Lib.XLog.Base;
using XLugia.Lib.XLog.Lib;

namespace XLugia.Lib.XLog
{
    internal class LogAttribute
    {
        /// <summary>
        /// 文件型日志编号
        /// 日志控件每次初始化后，都会重新开始计数
        /// </summary>
        static long logID = 0;

        /// <summary>
        /// 获取新日志编号
        /// </summary>
        public static long newLogID
        {
            get
            {
                IOLock.getIns().acquireWriterLock();
                try
                {
                    logID++;
                }
                catch (Exception ex)
                {
                    SimpleLogController.writeErrorLog(ex, "LogAttribute get newLogID");
                }
                finally
                {
                    IOLock.getIns().releaseWriterLock();
                }
                return logID;
            }
        }

        /// <summary>
        /// 文件型日志名称唯一标识
        /// 日志控件每次初始化后，唯一标识都会更改
        /// </summary>
        public static string logFileNameGuid = "";

        /// <summary>
        /// 日志目录
        /// </summary>
        public static string logPath
        {
            get
            {
                try
                {
                    return DirectoryController.getIns().repairDirPath(AppDomain.CurrentDomain.BaseDirectory) + "Log\\";
                }
                catch (Exception ex)
                {
                    SimpleLogController.writeErrorLog(ex, "LogAttribute get newLogID");
                    return "";
                }
            }
        }

        /// <summary>
        /// 日志读取超时时间（秒），默认30秒。
        /// </summary>
        public static int readLogTimeOut = 30000;

        /// <summary>
        /// 日志缓冲区命令超时时间（秒），默认30秒。
        /// </summary>
        public static int logBufferCommandTimeOut = 30;
    }
}
