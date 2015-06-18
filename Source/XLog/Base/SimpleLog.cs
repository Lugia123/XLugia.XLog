using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace XLugia.Lib.XLog.Base
{
    internal class SimpleLog
    {
        private static SimpleLog _instance = new SimpleLog();
        public static SimpleLog getIns() { return _instance; }

        /// <summary>
        /// 日志目录
        /// </summary>
        public string logPath
        {
            get
            {
                try
                {
                    return DirectoryController.getIns().repairDirPath(AppDomain.CurrentDomain.BaseDirectory) + "SimpleLog\\";
                }
                catch { }
                return "";
            }
        }

        public enum LogTypes
        {
            /// <summary>
            /// 系统
            /// </summary>
            System = 1,
            /// <summary>
            /// 调试
            /// </summary>
            Debug = 2,
            /// <summary>
            /// 错误
            /// </summary>
            Error = 3,
            /// <summary>
            /// 警告
            /// </summary>
            Warning = 4
        }

        private class LogModel
        {
            public string logContent;
            public LogTypes logType;
            public string logCategory;
            public DateTime logTime;

            public LogModel()
            {
                try
                {
                    this.logContent = "";
                    this.logType = LogTypes.Error;
                    this.logCategory = "";
                    this.logTime = DateTime.Now;
                }
                catch { }
            }

            public LogModel(string logContent, LogTypes logType, string logCategory = "")
            {
                try
                {
                    this.logContent = logContent;
                    this.logType = logType;
                    this.logCategory = logCategory;
                    this.logTime = DateTime.Now;
                }
                catch { }
            }

            public LogModel(Exception error, LogTypes logType, string logCategory = "")
            {
                try
                {
                    this.logContent = errorLog(error);
                    this.logType = logType;
                    this.logCategory = logCategory;
                    this.logTime = DateTime.Now;
                }
                catch { }
            }

            private string errorLog(Exception ex)
            {

                StringBuilder sb = new StringBuilder();
                try
                {
                    sb.AppendLine("▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼");
                    sb.AppendLine("Error Msg：" + ex.GetBaseException().Message);
                    sb.AppendLine("Stack：" + ex.GetBaseException().StackTrace);
                    sb.AppendLine("Ect：");
                    sb.AppendLine(ex.Message.ToString());
                    sb.AppendLine(ex.Source);
                    sb.AppendLine(ex.StackTrace);
                    sb.AppendLine("▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲");
                }
                catch { }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        public void writeLog(string logContent, LogTypes logType = LogTypes.System, string logCategory = "")
        {
            try
            {
                writeLogToFile(new LogModel(logContent, logType, logCategory));
            }
            catch { }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        public void writeLog(Exception error, LogTypes logType = LogTypes.Error, string logCategory = "")
        {
            try
            {
                writeLogToFile(new LogModel(error, logType, logCategory));
            }
            catch { }
        }

        /// <summary>
        /// 将日志写入到文件。
        /// </summary>
        /// <param name="log"></param>
        private void writeLogToFile(LogModel log)
        {
            try
            {
                string path = logPath;
                switch (log.logType)
                {
                    case LogTypes.Debug:
                        path += "Debug\\";
                        break;
                    case LogTypes.Error:
                        path += "Error\\";
                        break;
                    case LogTypes.Warning:
                        path += "Warning\\";
                        break;
                    default:
                        path += "System\\";
                        break;
                }
                path += string.IsNullOrEmpty(log.logCategory) ? "" : log.logCategory + "\\";

                string fileName = log.logTime.ToString("yyyy-MM-dd") + ".log";

                string content = "[" + log.logTime.ToString("yyyy-MM-dd HH:mm:ss:") + log.logTime.Millisecond.ToString() + "]" + log.logContent + "\r\n";

                FileController.getIns().saveFile(path, fileName, content);
            }
            catch { }
        }
    }
}
