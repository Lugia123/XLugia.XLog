using Database.Controller.Interface.Sync;
using Database.Model;
using Extended.Log.Lib;
using Extended.Tool.Common;
using Extended.Tool.OS;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Extended.Log.IO
{
    /// <summary>
    /// 将日志写入到MySQL
    /// </summary>
    internal class MySQLWriter
    {
        private static MySQLWriter instance = new MySQLWriter();
        public static MySQLWriter getIns() { return instance; }

        /// <summary>
        /// 并发处理结果
        /// </summary>
        ParallelLoopResult parallelLoopResult;

        /// <summary>
        /// 写日志开始时间
        /// </summary>
        DateTime startTime;

        public MySQLWriter()
        {
            parallelLoopResult = Parallel.For(0, 1, (i) => { });
            startTime = DateTime.Now;
        }

        /// <summary>
        /// 保存日志
        /// </summary>
        public void saveLogDictionaryQueue()
        {
            if (!parallelLoopResult.IsCompleted) return;
            startTime = DateTime.Now;
            LogPerformance.saveNumberPerTime = 0;
            List<string> keys = LogDictionaryQueue.getIns().getKeys();
            parallelLoopResult = Parallel.ForEach(keys, key =>
            {
                ConcurrentQueue<StrongBox<LogModel>> logQueue = LogDictionaryQueue.getIns().getLogDictionaryQueueValue(key);
                saveLogQueue(logQueue);
            });

            while (true)
            {
                if (parallelLoopResult.IsCompleted)
                {
                    LogPerformance.useMillisecondsPerTime = (DateTime.Now - startTime).TotalMilliseconds;
                    keys = null;
                    break;
                }
            }
        }

        /// <summary>
        /// 将日志保存到MySQL
        /// </summary>
        /// <param name="logQueue">日志队列</param>
        private void saveLogQueue(ConcurrentQueue<StrongBox<LogModel>> logQueue)
        {
            int count = logQueue.Count;
            if (count > LogPerformance.maxProcessCountPerTimeByMySQLType) count = LogPerformance.maxProcessCountPerTimeByMySQLType;
            LogPerformance.saveNumberPerTime += count;

            List<SystemLogModel> systemLogModels = new List<SystemLogModel>();
            ParallelLoopResult result = Parallel.For(0, count, (i) =>
            {
                StrongBox<LogModel> strongBox = LogDictionaryQueue.getIns().dequeue(logQueue);
                if (strongBox == null) return;
                LogModel logModel = strongBox.Value;

                using (logModel)
                {
                    IOLock.getIns().acquireWriterLock();
                    try
                    {
                        systemLogModels.Add(logModel.convertToSystemLogModel());
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        IOLock.getIns().releaseWriterLock();
                    }
                }

                strongBox.Value = null;
            });

            while (true)
            {
                if (result.IsCompleted)
                {
                    ISystemLogSync logSync = new ISystemLogSync(Database.Entities.Lib.DBSwitch.DBVersion.MySQL);
                    logSync.save(systemLogModels);
                    systemLogModels = null;
                    break;
                }
                Thread.Sleep(1);
            }
        }
    }
}
