using XLugia.Lib.XLog;
using XLugia.Lib.XTool.Common;
using XLugia.Lib.XTool.OS;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XLugia.Lib.XLog.Demo.Model.LogWriteDemo
{
    public partial class LogWriteTest : UserControl
    {
        ConcurrentQueue<Task> taskQueue;
        Task monitorTask;
        bool isEnableTestTask = true;
        DateTime testStartTime = DateTime.Now;
        double maxLogNumber = 0;
        double maxLogQueueCount = 0;
        double maxSaveNumberPerTime = 0;
        double maxUseMillisecondsPerTime = 0;
        double totalLogNumber = 0;
        double totalUseMilliseconds = 0;
        double avgSaveNumberPerSecond = 0;
        double maxAvgSaveNumberPerSecond = 0;
        double maxTestNumber = 0;
        const string testLogStr = "测试测试一二三四五六七八九十一二三四五六七八九十";
        string logStr = "";
        double logNumber = 0;
        bool isSingleFileTest = true;

        public LogWriteTest()
        {
            InitializeComponent();
        }

        private void bodyGrid_Loaded(object sender, RoutedEventArgs e)
        {
            taskNumberTextBox.MaxLength = 5;
            testLogLengthTextBox.MaxLength = 4;
            maxTestNumberTextBox.MaxLength = 8;
            clearTaskButton.IsEnabled = false;

            taskQueue = new ConcurrentQueue<Task>();
            monitorTask = new Task(() =>
            {
                while (true)
                {
                    refreshMessage();
                    logNumber = 0;
                    Thread.Sleep(1000);
                }
            });
            monitorTask.Start();
        }

        #region event
        private void addTaskButton_Click_1(object sender, RoutedEventArgs e)
        {
            startTest();
        }

        private void deleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            int num = taskNumberTextBox.Text.toInt32();
            ParallelLoopResult result = Parallel.For(0, num, (i) =>
            {
                deleteTask();
            });
        }

        private void clearTaskButton_Click(object sender, RoutedEventArgs e)
        {
            stopTest();
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(messageTextBox.Text);
        }

        private void singleFileRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            isSingleFileTest = true;
        }

        private void multiFileRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            isSingleFileTest = false;
        }

        private void disposeButton_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
        }
        #endregion

        #region test
        private void startTest()
        {
            isEnableTestTask = true;
            clearTaskButton.IsEnabled = true;
            addTaskButton.IsEnabled = false;
            taskNumberTextBox.IsEnabled = false;
            testLogLengthTextBox.IsEnabled = false;
            maxTestNumberTextBox.IsEnabled = false;
            singleFileRadioButton.IsEnabled = false;
            multiFileRadioButton.IsEnabled = false;

            int num = taskNumberTextBox.Text.toInt32();
            int len = testLogLengthTextBox.Text.toInt32();
            maxTestNumber = maxTestNumberTextBox.Text.toInt32();
            if (num == 0) { num = 5000; taskNumberTextBox.Text = "5000"; }
            if (len == 0) { len = 1000; testLogLengthTextBox.Text = "1000"; }
            if (maxTestNumber == 0) { maxTestNumber = 3000000; maxTestNumberTextBox.Text = "3000000"; }

            while (logStr.Length < len)
            {
                logStr += testLogStr;
            }
            logStr = logStr.Substring(0, len);

            totalUseMilliseconds = 0;
            avgSaveNumberPerSecond = 0;
            totalLogNumber = 0;
            testStartTime = DateTime.Now;

            ParallelLoopResult result = Parallel.For(0, num, (i) =>
            {
                addTask();
            });
        }

        private void stopTest()
        {
            isEnableTestTask = false;
            clearTask();
            clearTaskButton.IsEnabled = false;
            addTaskButton.IsEnabled = true;
            taskNumberTextBox.IsEnabled = true;
            testLogLengthTextBox.IsEnabled = true;
            maxTestNumberTextBox.IsEnabled = true;
            singleFileRadioButton.IsEnabled = true;
            multiFileRadioButton.IsEnabled = true;
        }

        private void addTask()
        {
            Task task = new Task(() =>
            {
                int testType = 0;
                while (isEnableTestTask && maxTestNumber > totalLogNumber)
                {
                    if (testType == 0) LogWriter.getIns().writeLog(logStr, LogType.getIns().debug.application);
                    if (testType == 1) LogWriter.getIns().writeLog(logStr, LogType.getIns().info.application);
                    if (testType == 2) LogWriter.getIns().writeLog(logStr, LogType.getIns().warn.application);
                    if (testType == 3) LogWriter.getIns().writeLog(logStr, LogType.getIns().error.application);
                    if (testType == 4) LogWriter.getIns().writeLog(logStr, LogType.getIns().fatal.application);
                    if (testType == 5) LogWriter.getIns().writeLog(logStr, LogType.getIns().debug.plc);
                    if (testType == 6) LogWriter.getIns().writeLog(logStr, LogType.getIns().info.plc);
                    if (testType == 7) LogWriter.getIns().writeLog(logStr, LogType.getIns().warn.plc);
                    if (!isSingleFileTest)
                    {
                        testType++;
                        if (testType > 7) testType = 0;
                    }

                    IOLock.getIns().acquireWriterLock();
                    try
                    {
                        logNumber++;
                        totalLogNumber++;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        IOLock.getIns().releaseWriterLock();
                    }
                   
                    Thread.Sleep(10);
                }
            });
            task.Start();
            taskQueue.Enqueue(task);
        }

        private bool deleteTask()
        {
            int tryCount = 100;
            Task task = null;
            while (tryCount > 0)
            {
                if (taskQueue.TryDequeue(out task))
                {
                    //RanToCompletion、Faulted 或 Canceled
                    while (tryCount > 0)
                    {
                        if (task.Status == TaskStatus.RanToCompletion
                        || task.Status == TaskStatus.Faulted
                        || task.Status == TaskStatus.Canceled)
                        {
                            task.Dispose();
                            break;
                        }
                        tryCount--;
                        Thread.Sleep(10);
                    }
                    break;
                }
                tryCount--;
                Thread.Sleep(10);
            }
            if (tryCount == 0) return false;
            return true;
        }

        private void clearTask()
        {
            int num = taskQueue.Count;
            ParallelLoopResult result = Parallel.For(0, num, (i) =>
            {
                deleteTask();
            });
        }

        private void refreshMessage()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                double taskCount = taskQueue.Count;
                double logQueueCount = LogPerformance.logQueueCount;
                double saveNumberPerTime = LogPerformance.saveNumberPerTime;
                double useMillisecondsPerTime = LogPerformance.useMillisecondsPerTime;
                if (maxLogNumber < logNumber) maxLogNumber = logNumber;
                if (maxLogQueueCount < logQueueCount) maxLogQueueCount = logQueueCount;
                if (maxSaveNumberPerTime < saveNumberPerTime) maxSaveNumberPerTime = saveNumberPerTime;
                if (maxUseMillisecondsPerTime < useMillisecondsPerTime) maxUseMillisecondsPerTime = useMillisecondsPerTime;
                if (maxAvgSaveNumberPerSecond < avgSaveNumberPerSecond) maxAvgSaveNumberPerSecond = avgSaveNumberPerSecond;
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(logStr);
                int logStrLength = bytes.Length;

                if (logQueueCount > 0)
                {
                    totalUseMilliseconds = (DateTime.Now - testStartTime).TotalMilliseconds;
                    if (useMillisecondsPerTime > 0) avgSaveNumberPerSecond = saveNumberPerTime / useMillisecondsPerTime * 1000;
                }

                StringBuilder message = new StringBuilder();
                message.AppendLine("[日志保存性能测试（文件日志）]");
                if (isSingleFileTest)
                {
                    message.AppendLine("单文件写入测试:Log\\Error\\Application");
                }
                else
                {
                    message.AppendLine("多文件写入测试:");
                    message.AppendLine("Log\\Error\\Application");
                    message.AppendLine("Log\\System\\Application");
                    message.AppendLine("Log\\Warning\\Application");
                    message.AppendLine("Log\\Debug\\Application");
                    message.AppendLine("Log\\Error\\Print");
                    message.AppendLine("Log\\System\\Print");
                    message.AppendLine("Log\\Warning\\Print");
                    message.AppendLine("Log\\Debug\\Print");
                }
                message.AppendLine("");
                message.AppendLine("当前时间:" + DateTime.Now.ToString());
                message.AppendLine("并发数:" + taskCount.ToString() + "个");
                message.AppendLine("每秒日志产生数:" + logNumber.ToString("###,###,###,###") + "个");
                message.AppendLine("单个测试日志长度:" + logStrLength.ToString("###,###,###,###") + "（双字节）");
                message.AppendLine("总日志数:" + totalLogNumber.ToString("###,###,###,###") + "个");
                message.AppendLine("总用时:" + Convert.ToInt64(totalUseMilliseconds / 1000).ToString("###,###,###,###") + "秒");
                message.AppendLine("");
                message.AppendLine("[日志保存性能参数]");
                message.AppendLine("日志保存等待队列:" + logQueueCount.ToString("###,###,###,###") + "个");
                message.AppendLine("每次保存数:" + saveNumberPerTime.ToString("###,###,###,###") + "个");
                message.AppendLine("每次保存花费时间:" + useMillisecondsPerTime.ToString() + "（毫秒）");
                message.AppendLine("平均每秒日志保存数:" + Convert.ToInt64(avgSaveNumberPerSecond).ToString("###,###,###,###") + "个");
                message.AppendLine("");
                message.AppendLine("[性能峰值]");
                message.AppendLine("每秒日志产生数峰值:" + maxLogNumber.ToString("###,###,###,###") + "个");
                message.AppendLine("日志保存等待队列峰值:" + maxLogQueueCount.ToString("###,###,###,###") + "个");
                message.AppendLine("每次保存数峰值:" + maxSaveNumberPerTime.ToString("###,###,###,###") + "个");
                message.AppendLine("每次保存花费时间峰值:" + maxUseMillisecondsPerTime.ToString() + "（毫秒）");
                message.AppendLine("平均每秒日志保存数峰值:" + Convert.ToInt64(maxAvgSaveNumberPerSecond).ToString("###,###,###,###") + "个");
                
                messageTextBox.Text = message.ToString();
            }));
        }
        #endregion
    }
}
