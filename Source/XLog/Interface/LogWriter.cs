using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Collections.Concurrent;
using XLugia.Lib.XLog.IO;
using System.Threading;
using System.Threading.Tasks;
using XLugia.Lib.XLog.Lib;

namespace XLugia.Lib.XLog
{
    /// <summary>
    /// 日志保存器。
    /// </summary>
    public class LogWriter : IDisposable
    {
        #region properties
        private static LogWriter instance = new LogWriter();

        /// <summary>
        /// 获取日志保存器静态实例。
        /// </summary>
        /// <returns>日志保存器。</returns>
        public static LogWriter getIns()
        {
            try
            {
                //日志控件每次初始化后，唯一标识会更改，日志编号会重新开始计数
                if (string.IsNullOrEmpty(LogAttribute.logFileNameGuid))
                {
                    LogAttribute.logFileNameGuid = DateTime.Now.Ticks.ToString();
                }
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogWriter getIns");
            }
            return instance;
        }

        bool isEnableTimer = true;
        Task task;
        #endregion

        public LogWriter()
        {
            try
            {
                startTimer();
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogWriter InitializeComponent");
            }
        }

        #region private
        /// <summary>
        /// 循环计时器，每100毫秒保存一次日志
        /// </summary>
        private void startTimer()
        {
            try
            {
                task = new Task(() =>
                {
                    startTimerTask();
                });
                task.Start();
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogWriter startTimer");
            }
        }

        /// <summary>
        /// 计时器任务
        /// </summary>
        private void startTimerTask()
        {
            try
            {
                while (isEnableTimer)
                {
                    try
                    {
                        if (LogDictionaryQueue.getIns().allCount() > 0)
                        {
                            FileWriter.getIns().saveLogDictionaryQueue();
                        }
                    }
                    catch (Exception ex)
                    {
                        SimpleLogController.writeErrorLog(ex, "LogWriter startTimerTask while");
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogWriter startTimerTask");
            }
        }
        #endregion

        #region public
        /// <summary>
        /// 保存日志。
        /// </summary>
        /// <param name="log">要记录的日志内容。</param>
        /// <param name="logTypeObject">LogTypeObject日志类型对象，默认类型为Error.Application。</param>
        /// <param name="logCategory">自定义日志类别，日志保存器会为每一种类别创建一个目录，并将相同类别的日志存放到对应目录中。</param>
        public void writeLog(string log, LogTypeObject logTypeObject = null, string logCategory = "")
        {
            try
            {
                if (logTypeObject == null) logTypeObject = LogType.getIns().error.application;
                if (!logTypeObject.isEnabled) return;
                List<LogModel> logModelList = LogModel.convertToList(log, logTypeObject.logType, logCategory);
                foreach (var logModel in logModelList)
                {
                    LogDictionaryQueue.getIns().enqueue(logModel);
                }
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogWriter writeLog");
            }
        }

        /// <summary>
        /// 保存日志。
        /// </summary>
        /// <param name="exception">保存Exception对象中错误消息。</param>
        /// <param name="logTypeObject">LogTypeObject日志类型对象，默认类型为Error.Application。</param>
        /// <param name="logCategory">自定义日志类别，日志保存器会为每一种类别创建一个目录，并将相同类别的日志存放到对应目录中。</param>
        public void writeLog(Exception exception, LogTypeObject logTypeObject = null, string logCategory = "")
        {
            try
            {
                if (logTypeObject == null) logTypeObject = LogType.getIns().error.application;
                if (!logTypeObject.isEnabled) return;
                List<LogModel> logModelList = LogModel.convertToList(exception, logTypeObject.logType, logCategory);
                foreach (var logModel in logModelList)
                {
                    LogDictionaryQueue.getIns().enqueue(logModel);
                }
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogWriter writeExceptionLog ");
            }
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            try
            {
                isEnableTimer = false;
                LogDictionaryQueue.getIns().Dispose();
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogWriter dispose ");
            }
        }
        #endregion
    }
}
