using Database.Controller.Interface.Sync;
using Database.Entities.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extended.Log.IO
{
    internal class DBWriter
    {
        private static DBWriter _instance = new DBWriter();
        public static DBWriter getIns() { return _instance; }

        ParallelLoopResult parallelLoopResult = new ParallelLoopResult();

        public void saveLogQueue()
        {
            int count = LogQueue.count();
            parallelLoopResult = Parallel.For(0, count, (i) =>
            {
                LogModel logModel = LogQueue.dequeue();
                if (logModel == null) return;
                writeToDB(logModel);
            });
        }

        public bool isCompleted()
        {
            return parallelLoopResult.IsCompleted;
        }

        public void writeToDB(LogModel logModel)
        {
            using (logModel)
            {
                if (!logModel.logTypeObject.isEnabled) return;
                DBSwitch.DBVersion dbVersion = DBSwitch.DBVersion.SqlServer;
                if (LogSaveMode.modeType == LogSaveMode.ModeType.SqlServerCompact) dbVersion = DBSwitch.DBVersion.SqlServerCompact;

                var result = (new ISystemLogSync(dbVersion)).save(logModel.systemLogModel);
                if (!result.isSuccessed)
                {
                    FileWriter.getIns().writeToFile(logModel);
                    FileWriter.getIns().writeToFile(new LogModel(result.exception, LogType.getIns().error.sql));
                }
            }
        }
    }
}
