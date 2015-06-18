using XLugia.Lib.XLog.Reader.Lib;
using XLugia.Lib.XTool.Common;
using Microsoft.Win32;
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
using XLugia.Lib.XLog;
using XLugia.Lib.XTool.OS;

namespace XLugia.Lib.XLog.Reader.Model.LogReader
{
    /// <summary>
    /// Default.xaml 的交互逻辑
    /// </summary>
    public partial class Default : UserControl
    {
        string filePath = "";
        DateTime? logDateTime;
        double totalPageCount = 0;
        double totalDataCount = 0;


        public Default()
        {
            try
            {
                InitializeComponent();
                initUI();
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "LogReader.Default InitializeComponent");
            }
        }

        #region event
        private void bodyGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                startTimeSlider.ValueChanged += (obj, args) =>
                {
                    try
                    {
                        if (startTimeSlider.Value > endTimeSlider.Value) startTimeSlider.Value = endTimeSlider.Value;
                        refershMessage();
                    }
                    catch (Exception ex)
                    {
                        LogController.writeErrorLog(ex, "LogReader.Default startTimeSlider.ValueChanged");
                    }
                };

                endTimeSlider.ValueChanged += (obj, args) =>
                {
                    try
                    {
                        if (endTimeSlider.Value < startTimeSlider.Value) endTimeSlider.Value = startTimeSlider.Value;
                        refershMessage();
                    }
                    catch (Exception ex)
                    {
                        LogController.writeErrorLog(ex, "LogReader.Default endTimeSlider.ValueChanged");
                    }
                };

                pageSlider.ValueChanged += (obj, args) =>
                {
                    try
                    {
                        readLog();
                    }
                    catch (Exception ex)
                    {
                        LogController.writeErrorLog(ex, "LogReader.Default pageSlider.ValueChanged");
                    }
                };

                searchButton.Click += (obj, args) =>
                {
                    try
                    {
                        readLog();
                    }
                    catch (Exception ex)
                    {
                        LogController.writeErrorLog(ex, "LogReader.Default searchButton.Click");
                    }
                };

                pageSizeListBox.Items.Add("100");
                pageSizeListBox.Items.Add("500");
                pageSizeListBox.Items.Add("1000");
                pageSizeListBox.Items.Add("2000");
                pageSizeListBox.SelectedIndex = 0;
                pageSizeListBox.SelectionChanged += (obj, args) =>
                {
                    try
                    {
                        readLog();
                    }
                    catch (Exception ex)
                    {
                        LogController.writeErrorLog(ex, "LogReader.Default pageSizeListBox.SelectionChanged");
                    }
                };
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "LogReader.Default bodyGrid_Loaded");
            }
        }
        #endregion

        #region menu event
        private void openLogFileMenuButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                openlogFile();
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "LogReader.Default openLogFileMenuButton_Click");
            }
        }

        private void disposeMenuButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dispose();
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "LogReader.Default disposeMenuButton_Click");
            }
        }

        private void writeLogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "LogReader.Default writeLogButton_Click");
            }
        }
        #endregion

        #region funcation
        private void initUI()
        {
            try
            {
                logDateTime = null;
                logFileBlock.Text = "";
                startTimeTextBlock.Text = "开始：00:00:00";
                startTimeSlider.Maximum = 86400 - 1;
                startTimeSlider.SmallChange = 60;
                startTimeSlider.LargeChange = 60;
                startTimeSlider.TickFrequency = 60;
                startTimeSlider.IsSnapToTickEnabled = true;

                endTimeTextBlock.Text = "结束：00:00:00";
                endTimeSlider.Maximum = 86400;
                endTimeSlider.Value = 86400;
                endTimeSlider.SmallChange = 60;
                endTimeSlider.LargeChange = 60;
                endTimeSlider.TickFrequency = 60;
                endTimeSlider.IsSnapToTickEnabled = true;

                pageSlider.Minimum = 1;
                pageSlider.Maximum = 1;
                pageSlider.Value = 1;
                pageSlider.SmallChange = 1;
                pageSlider.LargeChange = 1;
                pageSlider.IsSnapToTickEnabled = true;
                pageDataCountTextBlock.Text = "1/10页 共0条";

                searchButton.IsEnabled = false;

                startTimeSlider.IsEnabled = false;
                endTimeSlider.IsEnabled = false;
                pageSlider.IsEnabled = false;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "LogReader.Default initUI");
            }
        }

        private void openlogFile()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "日志(*.exlog)|*.exlog";
                openFileDialog.InitialDirectory = DirectoryController.getIns().repairDirPath(AppDomain.CurrentDomain.BaseDirectory) + "Log\\";
                openFileDialog.Multiselect = false;
                openFileDialog.FileOk += (obj, args) =>
                {
                    try
                    {
                        if (!filePath.Equals(openFileDialog.FileName))
                        {
                            initUI();
                        }

                        filePath = openFileDialog.FileName;
                        logFileBlock.Text = filePath;

                        readLog();
                    }
                    catch (Exception ex)
                    {
                        LogController.writeErrorLog(ex, "LogReader.Default openFileDialog.FileOk");
                    }
                };
                openFileDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "LogReader.Default openlogFile");
            }
        }

        private void dispose()
        {
            try
            {
                GC.Collect(2);
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "LogReader.Default dispose");
            }
        }

        private void refershMessage()
        {
            try
            {
                string logDateTimeString = logDateTime.HasValue ? logDateTime.Value.ToString("yyyy-MM-dd 00:00:00") : "00:00:00";
                startTimeTextBlock.Text = DateTime.Parse(logDateTimeString).AddSeconds(startTimeSlider.Value).ToString("开始：yyyy-MM-dd HH:mm:ss");
                endTimeTextBlock.Text = DateTime.Parse(logDateTimeString).AddSeconds(endTimeSlider.Value).ToString("结束：yyyy-MM-dd HH:mm:ss");

                pageDataCountTextBlock.Text = string.Format("{0:###,###,###,###,###}/{1:###,###,###,###,###}页  共{2:###,###,###,###,###}条", pageSlider.Value, totalPageCount, totalDataCount);
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "LogReader.Default refershMessage");
            }
        }
        #endregion

        #region readLog
        private void readLog()
        {
            try
            {
                if ("".Equals(filePath)) return;

                DateTime? startTime = null;
                DateTime? endTime = null;

                if (logDateTime.HasValue)
                {
                    string logDateTimeString = logDateTime.HasValue ? logDateTime.Value.ToString("yyyy-MM-dd 00:00:00") : "00:00:00";
                    startTime = DateTime.Parse(logDateTimeString).AddSeconds(startTimeSlider.Value);
                    endTime = DateTime.Parse(logDateTimeString).AddSeconds(endTimeSlider.Value);
                }

                LogSearchFilterModel logSearchFilterModel = new LogSearchFilterModel()
                {
                    pageIndex = (long)pageSlider.Value - 1,
                    pageSize = Convert.ToInt32(pageSizeListBox.SelectedValue),
                    startDateTime = startTime,
                    endDateTime = endTime,
                    logDateTimeOrder = LogSearchFilterModel.OrderTypes.Asc
                };

                var logSearchResultModel = XLugia.Lib.XLog.LogReader.getIns().readLog(filePath, logSearchFilterModel);
                if (logSearchResultModel.totalDataCount > 0 && logSearchResultModel.data.Count > 0)
                {
                    searchButton.IsEnabled = true;

                    startTimeSlider.IsEnabled = true;
                    endTimeSlider.IsEnabled = true;
                    pageSlider.IsEnabled = true;

                    pageSlider.Maximum = logSearchResultModel.totalPageCount;
                    if (totalPageCount != logSearchResultModel.totalPageCount)
                    {
                        pageSlider.Value = 1;
                    }

                    logDateTime = DateTime.Parse(logSearchResultModel.data[0].logDateTimeString);
                    totalPageCount = logSearchResultModel.totalPageCount;
                    totalDataCount = logSearchResultModel.totalDataCount;
                }

                refershMessage();
                convertLogToHtml(logSearchResultModel);
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "LogReader.Default readLog");
            }
        }
        #endregion

        #region logWebViewer
        string headHtml = "<head><title></title><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head>";
        string logContentStyle = @"padding:10px 10px 10px 10px;color:#000000;background-color:#e5e4e2;font-size:20px;word-break:break-all;line-height:1.5;font-family:微软雅黑;";
        string logTimeStyle = @"color:#006ca5;font-size:26px;word-break:break-all;line-height:1.5;font-family:微软雅黑;";

        private void convertLogToHtml(LogSearchResultModel result)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<!DOCTYPE html>");
                sb.Append(@"<html lang=""zh-cn"" xmlns=""http://www.w3.org/1999/xhtml"">");
                sb.Append(headHtml);
                sb.Append(@"<body style=""background-color:#d4d2cf;"">");

                StringBuilder logContent = new StringBuilder();
                long logID = 0;
                int logIndex = 0;
                foreach (var logData in result.data.OrderBy((e) =>
                {
                    try
                    {
                        return e.id;
                    }
                    catch (Exception ex)
                    {
                        LogController.writeErrorLog(ex, "LogReader.Default convertLogToHtml result.data.OrderBy");
                        return 0;
                    }
                }))
                {
                    if (logID != logData.logID)
                    {
                        if (logID != 0)
                        {
                            logContent.Append("</div><br/>");
                            sb.Append(logContent.ToString());
                        }

                        logID = logData.logID;
                        logIndex = logData.logIndex;
                        logContent = new StringBuilder();
                        logContent.Append(@"<div style=""");
                        logContent.Append(logContentStyle);
                        logContent.Append(@""">");

                        //006ca5 LogTimeColor
                        logContent.Append(@"<div style=""");
                        logContent.Append(logTimeStyle);
                        logContent.Append(@""">");
                        logContent.Append(logData.logDateTimeString + "&nbsp;&nbsp;&nbsp;&nbsp;" + logData.logCategory);
                        logContent.Append(@"</div>");
                    }

                    logContent.Append(convertStringToHtml(logData.logContent));
                }

                if (logContent.ToString().Length > 0)
                {
                    logContent.Append("</p>");
                    sb.Append(logContent.ToString());
                }

                sb.Append("</body>");
                sb.Append("</html>");
                logViewerWebBrowser.NavigateToString(sb.ToString());
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "LogReader.Default convertLogToHtml");
            }
        }

        private string convertStringToHtml(string content)
        {
            try
            {
                content = content.Replace(" ", "&nbsp;");
                content = content.Replace("\r", "");
                content = content.Replace("\n", "<br/>");
                content = content.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                return content;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "LogReader.Default convertStringToHtml");
                return "";
            }
        }
        #endregion
    }
}
