using Database.Controller.Interface.Sync;
using Database.Model;
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
    /// 将日志写入到SQLServer
    /// </summary>
    internal class DBWriter: BaseWriter
    {
        private static DBWriter instance = new DBWriter();
        public static DBWriter getIns() { return instance; }

        /// <summary>
        /// 每次日志最大保存数
        /// </summary>
        int maxProcessCount
        {
            get
            {
                switch (LogAttribute.saveModeType)
                {
                    case LogAttribute.SaveModeType.MySQL:
                        return LogPerformance.maxProcessCountPerTimeByMySQLType;
                    case LogAttribute.SaveModeType.SqlServer:
                        return LogPerformance.maxProcessCountPerTimeBySQLServerType;
                    default:
                        return LogPerformance.maxProcessCountPerTimeBySQLCompactType;
                }
            }
        }

        /// <summary>
        /// 将日志保存到数据库
        /// </summary>
        /// <param name="logQueue">日志队列</param>
        public override void saveLogQueue(ConcurrentQueue<StrongBox<LogModel>> logQueue)
        {
            int count = logQueue.Count;
            if (count > maxProcessCount) count = maxProcessCount;
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

            if (result.IsCompleted)
            {
                saveLogToDatabase(systemLogModels);
                systemLogModels = null;
            }
            else
            {
                throw new Exception("Result Not Completed.");
            }
        }

        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="systemLogModels">日志 List<SystemLogModel> 对象</param>
        private void saveLogToDatabase(List<SystemLogModel> systemLogModels)
        {
            //ToDo 
            //缺少保存出错时的处理！
            switch (LogAttribute.saveModeType)
            {
                case LogAttribute.SaveModeType.MySQL:
                    ISystemLogSync logSyncMySQL = new ISystemLogSync(Database.Entities.Lib.DBSwitch.DBVersion.MySQL);
                    logSyncMySQL.save(systemLogModels);
                    break;
                case LogAttribute.SaveModeType.SqlServer:
                    ISystemLogSync logSyncSQLServer = new ISystemLogSync(Database.Entities.Lib.DBSwitch.DBVersion.SqlServer);
                    logSyncSQLServer.bulkSave(systemLogModels);
                    break;
                default:
                    ISystemLogSync logSyncSQLCompact = new ISystemLogSync(Database.Entities.Lib.DBSwitch.DBVersion.SqlServerCompact);
                    logSyncSQLCompact.save(systemLogModels);
                    break;
            }
        }
    }
}
