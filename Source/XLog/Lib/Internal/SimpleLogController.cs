using XLugia.Lib.XLog.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLugia.Lib.XLog.Lib
{
    internal class SimpleLogController
    {
        public static void writeErrorLog(Exception error, string codeRemarks)
        {
            try
            {
                SimpleLog.getIns().writeLog(codeRemarks + " Error:" + error.Message, SimpleLog.LogTypes.Error, "ExtendedLog");
                SimpleLog.getIns().writeLog(error, SimpleLog.LogTypes.Error, "ExtendedLog");
            }
            catch { }
        }
    }
}
