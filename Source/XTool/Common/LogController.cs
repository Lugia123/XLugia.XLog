using XLugia.Lib.XLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLugia.Lib.XTool.Common
{
    internal class LogController
    {
        public static void writeErrorLog(Exception error, string codeRemarks = "")
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

                LogWriter.getIns().writeLog(sb.ToString(), LogType.getIns().error.system, "XLugia.Lib.XTool");
            }
            catch { }
        }
    }
}
