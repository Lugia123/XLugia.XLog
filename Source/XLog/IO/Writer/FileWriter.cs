using XLugia.Lib.XLog.Base;
using XLugia.Lib.XLog.Lib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XLugia.Lib.XLog.IO
{
    /// <summary>
    /// 将日志写入到文件
    /// </summary>
    internal class FileWriter : BaseWriter
    {
        private static FileWriter instance = new FileWriter();
        public static FileWriter getIns() { return instance; }

        /// <summary>
        /// 将日志保存到文件
        /// </summary>
        /// <param name="logQueue">日志队列</param>
        public override void saveLogQueue(ConcurrentQueue<StrongBox<LogModel>> logQueue)
        {
            try
            {
                int count = logQueue.Count;
                if (count <= 0) return;
                if (count > LogPerformance.maxProcessCountPerTimeByFileType) count = LogPerformance.maxProcessCountPerTimeByFileType;
                LogPerformance.saveNumberPerTime += count;

                string dirPath = "";
                string fileName = "";

                byte[] logTotalBuff = new byte[LogModel.logTotalDoubleByteLength * count];
                long index = 0;
                for (int i = 0; i < count; i++)
                {
                    StrongBox<LogModel> strongBox = LogDictionaryQueue.getIns().dequeue(logQueue);
                    if (strongBox == null) return;
                    LogModel logModel = strongBox.Value;

                    using (logModel)
                    {
                        if ("".Equals(dirPath))
                        {
                            dirPath = LogAttribute.logPath + logModel.logType.Replace('_', '\\') + "\\" + logModel.logCategory.Replace('_', '\\') + "\\";
                            fileName = logModel.logDateString + "[" + LogAttribute.logFileNameGuid + "]" + ".exlog";
                        }

                        logModel.logBuffer.CopyTo(logTotalBuff, index);
                        index += LogModel.logTotalDoubleByteLength;
                    }

                    strongBox.Value = null;
                }

                DirectoryController.getIns().createDirectory(dirPath);
                using (FileStream fs = new FileStream(dirPath + fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Write(logTotalBuff, 0, logTotalBuff.Count());
                    fs.Close();
                }
                dirPath = null;
                fileName = null;
                logTotalBuff = null;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "FileWriter saveLogQueue");
            }
        }
    }
}
