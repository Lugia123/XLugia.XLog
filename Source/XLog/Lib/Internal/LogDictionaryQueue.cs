using XLugia.Lib.XLog.Lib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace XLugia.Lib.XLog
{
    /// <summary>
    /// 日志队列
    /// </summary>
    internal class LogDictionaryQueue:IDisposable
    {
        private static LogDictionaryQueue instance = new LogDictionaryQueue();
        public static LogDictionaryQueue getIns() { return instance; }

        /// <summary>
        /// 队列字典，根据日志类型分成多个不同队列
        /// </summary>
        ConcurrentDictionary<string, ConcurrentQueue<StrongBox<LogModel>>> logDictionaryQueue = new ConcurrentDictionary<string, ConcurrentQueue<StrongBox<LogModel>>>();//分类队列
        
        /// <summary>
        /// 队列读取最大重试次数
        /// </summary>
        const int MAX_TRY_COUNT = 1000;

        /// <summary>
        /// 队列总长度
        /// </summary>
        public double allCount()
        {
            double allCount = 0;
            try
            {
                if (logDictionaryQueue == null) return 0;
                foreach (var logType in logDictionaryQueue.Keys)
                {
                    ConcurrentQueue<StrongBox<LogModel>> logQueue = getLogQueue(logType);
                    if (logQueue == null) continue;
                    allCount += logQueue.Count();
                }
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogDictionaryQueue allCount");
            }
            return allCount;
        }

        /// <summary>
        /// 获取队列字典中日志的类型
        /// </summary>
        public List<string> getLogTypes()
        {
            try
            {
                if (logDictionaryQueue == null) return null;
                return logDictionaryQueue.Keys.ToList();
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogDictionaryQueue getLogTypes");
                return null;
            }
        }

        /// <summary>
        /// 获取指定类型的日志队列
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="logCategory">日志类别</param>
        /// <returns>ConcurrentQueue<StrongBox<LogModel>> 对象</returns>
        public ConcurrentQueue<StrongBox<LogModel>> getLogQueue(string logType)
        {
            ConcurrentQueue<StrongBox<LogModel>> logQueue = null;
            try
            {
                if (logDictionaryQueue == null) return null;
                int tryCount = MAX_TRY_COUNT;
                while (tryCount > 0)
                {
                    if (logDictionaryQueue.TryGetValue(logType, out logQueue)) break;
                    tryCount--;
                }
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogDictionaryQueue getLogQueue");
            }
            return logQueue;
        }

        /// <summary>
        /// 往队列字典中添加日志队列
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="logQueue">日志队列</param>
        private void addLogQueue(string logType, ConcurrentQueue<StrongBox<LogModel>> logQueue)
        {
            try
            {
                if (logDictionaryQueue == null) return;
                int tryCount = MAX_TRY_COUNT;
                while (tryCount > 0)
                {
                    if (logDictionaryQueue.TryAdd(logType, logQueue)) break;
                    tryCount--;
                }
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogDictionaryQueue addLogQueue");
            }
        }

        /// <summary>
        /// 讲日志加入到队列中
        /// </summary>
        /// <param name="logModel">日志 LogModel 对象</param>
        public void enqueue(LogModel logModel)
        {
            try
            {
                string logType = logModel.logType + logModel.logCategory;
                ConcurrentQueue<StrongBox<LogModel>> logQueue = getLogQueue(logType);
                if (logQueue == null)
                {
                    logQueue = new ConcurrentQueue<StrongBox<LogModel>>();
                    logQueue.Enqueue(new StrongBox<LogModel>(logModel));
                    addLogQueue(logType, logQueue);
                    return;
                }
                logQueue.Enqueue(new StrongBox<LogModel>(logModel));
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogDictionaryQueue enqueue");
            }
        }

        /// <summary>
        /// 将日志移除队列
        /// </summary>
        /// <param name="logQueue">日志队列</param>
        /// <returns>StrongBox<LogModel> 对象</returns>
        public StrongBox<LogModel> dequeue(ConcurrentQueue<StrongBox<LogModel>> logQueue)
        {
            StrongBox<LogModel> strongBox = null;
            try
            {
                if (logQueue == null) return strongBox;

                int tryCount = MAX_TRY_COUNT;
                while (tryCount > 0)
                {
                    if (logQueue.TryDequeue(out strongBox)) break;
                    tryCount--;
                }
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogDictionaryQueue dequeue");
            }
            return strongBox;
        }

        public void Dispose()
        {
            try
            {
                logDictionaryQueue.Clear();
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogDictionaryQueue dispose");
            }
        }
    }
}
