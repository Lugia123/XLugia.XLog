using XLugia.Lib.XLog.Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using XLugia.Lib.XLog.Base;

namespace XLugia.Lib.XLog
{
    /// <summary>
    /// 日志数据模型对象
    /// </summary>
    public class LogModel : IDisposable
    {
        public LogModel() { }

        #region 属性
        /// <summary>
        /// 顺序编号
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 日志编号
        /// </summary>
        public long logID { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public string logType { get; set; }

        /// <summary>
        /// 日志类别
        /// </summary>
        public string logCategory { get; set; }

        /// <summary>
        /// 日志内容分割编号
        /// </summary>
        public int logIndex { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string logContent { get; set; }

        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime logDateTime { get; set; }

        /// <summary>
        /// 日志时间字符串 格式：yyyy-MM-dd HH:mm:ss.fff
        /// </summary>
        public string logDateTimeString { get; set; }

        /// <summary>
        /// 日志时间字符串 格式：yyyy-MM-dd
        /// </summary>
        public string logDateString { get; set; }

        /// <summary>
        /// 日志 byte[] 对象，文件型日志保存时用
        /// </summary>
        public byte[] logBuffer { get; set; }
        #endregion

        #region 日志各属性最大长度，文件型保存时用
        /// <summary>
        /// 单条日志总长度
        /// </summary>
        private const int LOG_TOTAL_LENGTH = 256;

        /// <summary>
        /// 日志编号长度
        /// </summary>
        private const int LOG_ID_LENGTH = 19;

        /// <summary>
        /// 日志类型长度
        /// </summary>
        private const int LOG_TYPE_LENGTH = 36;

        /// <summary>
        /// 日志类别长度
        /// </summary>
        private const int LOG_CATEGORY_LENGTH = 36;

        /// <summary>
        /// 日志内容分割用索引长度
        /// </summary>
        private const int LOG_INDEX_LENGTH = 10;

        /// <summary>
        /// 日志内容长度
        /// </summary>
        private const int LOG_CONTENT_LENGTH = 100;

        /// <summary>
        /// 日志时间长度
        /// </summary>
        private const int LOG_DATETIME_LENGTH = 23;

        /// <summary>
        /// 备用
        /// </summary>
        private const int LOG_ETC_LENGTH = 32;
        #endregion

        #region 日志各属性Unicode编码下最大长度，文件型保存时用
        /// <summary>
        /// 单条日志总长度
        /// </summary>
        internal static int logTotalDoubleByteLength { get { return LOG_TOTAL_LENGTH * 2; } }

        /// <summary>
        /// 日志编号长度
        /// </summary>
        internal static int logIdDoubleByteLength { get { return LOG_ID_LENGTH * 2; } }

        /// <summary>
        /// 日志类型长度
        /// </summary>
        internal static int logTypeDoubleByteLength { get { return LOG_TYPE_LENGTH * 2; } }

        /// <summary>
        /// 日志类别长度
        /// </summary>
        internal static int logCategoryDoubleByteLength { get { return LOG_CATEGORY_LENGTH * 2; } }

        /// <summary>
        /// 日志内容分割用索引长度
        /// </summary>
        internal static int logIndexDoubleByteLength { get { return LOG_INDEX_LENGTH * 2; } }

        /// <summary>
        /// 日志内容长度
        /// </summary>
        internal static int logContentDoubleByteLength { get { return LOG_CONTENT_LENGTH * 2; } }

        /// <summary>
        /// 日志时间长度
        /// </summary>
        internal static int logDateTimeDoubleByteLength { get { return LOG_DATETIME_LENGTH * 2; } }

        /// <summary>
        /// 备用
        /// </summary>
        internal static int logEtcDoubleByteLength { get { return LOG_ETC_LENGTH * 2; } }
        #endregion

        #region private
        private LogModel(long logID
                        , string logType
                        , string logCategory
                        , int logIndex
                        , string logContent
                        , DateTime logDateTime)
        {
            try
            {
                this.logID = logID;
                this.logType = logType;
                this.logCategory = logCategory;
                this.logIndex = logIndex;
                this.logContent = logContent;
                this.logDateTime = logDateTime;
                this.logDateTimeString = logDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                this.logDateString = logDateTime.ToString("yyyy-MM-dd");
                initLogBuffer();
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogModel InitializeComponent");
            }
        }

        /// <summary>
        /// 字节数组，日志文件保存用
        /// </summary>
        private void initLogBuffer()
        {
            try
            {
                this.logBuffer = new byte[logTotalDoubleByteLength];
                int offsetIndex = 0;

                //logID
                this.logBuffer.insertUnicodeString(this.logID.ToString(), offsetIndex);
                offsetIndex += logIdDoubleByteLength;

                //logType
                this.logBuffer.insertUnicodeString(this.logType, offsetIndex);
                offsetIndex += logTypeDoubleByteLength;

                //logCategory
                this.logBuffer.insertUnicodeString(this.logCategory, offsetIndex);
                offsetIndex += logCategoryDoubleByteLength;

                //logIndex
                this.logBuffer.insertUnicodeString(this.logIndex.ToString(), offsetIndex);
                offsetIndex += logIndexDoubleByteLength;

                //logContent
                this.logBuffer.insertUnicodeString(this.logContent, offsetIndex);
                offsetIndex += logContentDoubleByteLength;

                //logDateTimeString
                this.logBuffer.insertUnicodeString(this.logDateTimeString, offsetIndex);
                offsetIndex += logDateTimeDoubleByteLength;

                //logEtc
                this.logBuffer.insertUnicodeString("", offsetIndex);
                offsetIndex += logEtcDoubleByteLength;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogModel initLogBuffer");
            }
        }
        #endregion

        #region internal
        internal LogModel(byte[] logBuffer)
        {
            try
            {
                int offsetIndex = 0;
                //logID
                this.logID = Convert.ToInt64(logBuffer.toUnicodeString(offsetIndex, logIdDoubleByteLength));
                offsetIndex += logIdDoubleByteLength;

                //logType
                this.logType = logBuffer.toUnicodeString(offsetIndex, logTypeDoubleByteLength);
                offsetIndex += logTypeDoubleByteLength;

                //logCategory
                this.logCategory = logBuffer.toUnicodeString(offsetIndex, logCategoryDoubleByteLength);
                offsetIndex += logCategoryDoubleByteLength;

                //logIndex
                this.logIndex = Convert.ToInt32(logBuffer.toUnicodeString(offsetIndex, logIndexDoubleByteLength));
                offsetIndex += logIndexDoubleByteLength;

                //logContent
                this.logContent = logBuffer.toUnicodeString(offsetIndex, logContentDoubleByteLength);
                offsetIndex += logContentDoubleByteLength;

                //logDateTimeString
                this.logDateTimeString = logBuffer.toUnicodeString(offsetIndex, logDateTimeDoubleByteLength);
                offsetIndex += logDateTimeDoubleByteLength;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogModel InitializeComponent");
            }
        }

        /// <summary>
        /// 将要保存的日志内容转换成List<LogModel>对象
        /// </summary>
        /// <param name="logContent">日志内容</param>
        /// <param name="logType">日志类型</param>
        /// <param name="logCategory">日志类别</param>
        /// <returns>List<LogModel>对象</returns>
        internal static List<LogModel> convertToList(string logContent
            , string logType
            , string logCategory = "")
        {
            List<LogModel> logModelList = new List<LogModel>();

            try
            {
                //判断日志长度是否大于单条日志记录最大长度（100）
                //如果大于最大长度（100），把要保存的日志切割成多份
                int dataLength = LOG_CONTENT_LENGTH;
                bool isLargeData = logContent.Length > dataLength;
                int dataCount = logContent.Length / dataLength + 1;

                long logID = LogAttribute.newLogID; //DateTime.Now.Ticks;
                DateTime logDateTime = DateTime.Now;
                logType = logType.Length > LOG_TYPE_LENGTH ? logType.Substring(0, LOG_TYPE_LENGTH) : logType;
                logCategory = logCategory.Length > LOG_CATEGORY_LENGTH ? logCategory.Substring(0, LOG_CATEGORY_LENGTH) : logCategory;

                for (int k = 0; k < dataCount; k++)
                {
                    int subStringLength = dataLength;
                    if (logContent.Length - dataLength * (k + 1) < 0)
                    {
                        subStringLength = logContent.Length - dataLength * k;
                    }

                    if (string.IsNullOrEmpty(logContent.Substring(k * dataLength, subStringLength))) continue;

                    LogModel logModel = new LogModel(logID
                                                        , logType
                                                        , logCategory
                                                        , (k + 1 > 999999 ? 999999 : k + 1)
                                                        , logContent.Substring(k * dataLength, subStringLength)
                                                        , logDateTime);

                    logModelList.Add(logModel);
                }
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogModel convertToList");
            }
            return logModelList;
        }

        /// <summary>
        /// 将要保存的报错信息转换成List<LogModel>对象
        /// </summary>
        /// <param name="exception">报错信息</param>
        /// <param name="logType">日志类型</param>
        /// <param name="logCategory">日志类别</param>
        /// <returns>List<LogModel>对象</returns>
        internal static List<LogModel> convertToList(Exception exception
            , string logType
            , string logCategory = "")
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendLine("[Base Message]");
                sb.AppendLine(exception.GetBaseException().Message);
                sb.AppendLine("[Base StackTrace]");
                sb.AppendLine(exception.GetBaseException().StackTrace);
                sb.AppendLine("[Message]");
                sb.AppendLine(exception.Message.ToString());
                sb.AppendLine("[Source]");
                sb.AppendLine(exception.Source);
                sb.AppendLine("[StackTrace]");
                sb.AppendLine(exception.StackTrace);
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogModel convertExceptionToList");
            }

            return convertToList(sb.ToString()
                                , logType
                                , logCategory);
        }
        #endregion

        public void Dispose()
        {
            try
            {
                this.logType = null;
                this.logCategory = null;
                this.logContent = null;
                this.logDateTimeString = null;
                this.logDateString = null;
                this.logBuffer = null;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogPerformance Dispose");
            }
        }
    }
}
