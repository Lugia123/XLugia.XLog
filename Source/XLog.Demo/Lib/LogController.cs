using XLugia.Lib.XLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLugia.Lib.XLog.Demo.Lib
{
    internal class LogController
    {
        public static bool isShowConsoleWriteLine = false;
        public static bool IsEnableSystemLog = false;

        public static void writeSystemLog(string log, string codeRemarks)
        {
            try
            {
                if (isShowConsoleWriteLine)
                {
                    Console.WriteLine(codeRemarks);
                    Console.WriteLine(log);
                }

                if (IsEnableSystemLog)
                {
                    LogWriter.getIns().writeLog(codeRemarks, LogType.getIns().info.system, "Ex日志工具");
                    LogWriter.getIns().writeLog(log, LogType.getIns().info.system, "Ex日志工具");
                }
            }
            catch { }
        }

        public static void writeErrorLog(string log, string codeRemarks)
        {
            try
            {
                if (isShowConsoleWriteLine)
                {
                    Console.WriteLine(codeRemarks);
                    Console.WriteLine(log);
                }

                if (IsEnableSystemLog)
                {
                    LogWriter.getIns().writeLog(codeRemarks, LogType.getIns().error.system, "Ex日志工具");
                    LogWriter.getIns().writeLog(log, LogType.getIns().error.system, "Ex日志工具");
                }
            }
            catch { }
        }

        public static void writeErrorLog(Exception error, string codeRemarks)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(codeRemarks);
                sb.AppendLine("▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼");
                sb.AppendLine("Error Msg：" + error.GetBaseException().Message);
                sb.AppendLine("Stack：" + error.GetBaseException().StackTrace);
                sb.AppendLine("Ect：");
                sb.AppendLine(error.Message.ToString());
                sb.AppendLine(error.Source);
                sb.AppendLine(error.StackTrace);
                sb.AppendLine("▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲");

                if (isShowConsoleWriteLine) Console.WriteLine(sb.ToString());
                LogWriter.getIns().writeLog(sb.ToString(), LogType.getIns().error.system, "Ex日志工具");
            }
            catch { }
        }
    }
}
