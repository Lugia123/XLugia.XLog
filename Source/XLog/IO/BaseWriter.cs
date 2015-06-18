using XLugia.Lib.XLog.Lib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XLugia.Lib.XLog.IO
{
    /// <summary>
    /// 保存日志
    /// </summary>
    internal class BaseWriter
    {
        /// <summary>
        /// 并发处理结果
        /// </summary>
        ParallelLoopResult parallelLoopResult;

        /// <summary>
        /// 写日志开始时间
        /// </summary>
        DateTime startTime;

        public BaseWriter()
        {
            try
            {
                parallelLoopResult = Parallel.For(0, 1, (i) => { });
                startTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "BaseWriter InitializeComponent");
            }
        }

        /// <summary>
        /// 保存日志
        /// </summary>
        public void saveLogDictionaryQueue()
        {
            try
            {
                if (!parallelLoopResult.IsCompleted) return;
                startTime = DateTime.Now;
                LogPerformance.saveNumberPerTime = 0;
                List<string> logTypes = LogDictionaryQueue.getIns().getLogTypes();
                parallelLoopResult = Parallel.ForEach(logTypes, logType =>
                {
                    try
                    {
                        ConcurrentQueue<StrongBox<LogModel>> logQueue = LogDictionaryQueue.getIns().getLogQueue(logType);
                        saveLogQueue(logQueue);
                    }
                    catch (Exception ex)
                    {
                        SimpleLogController.writeErrorLog(ex, "BaseWriter saveLogDictionaryQueue Parallel.ForEach");
                    }
                });

                LogPerformance.useMillisecondsPerTime = (DateTime.Now - startTime).TotalMilliseconds;
                logTypes = null;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "BaseWriter saveLogDictionaryQueue");
            }
        }

        /// <summary>
        /// 将日志保存到文件
        /// </summary>
        /// <param name="logQueue">日志队列</param>
        virtual public void saveLogQueue(ConcurrentQueue<StrongBox<LogModel>> logQueue)
        {
        }
    }
}
