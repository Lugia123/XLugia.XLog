using XLugia.Lib.XLog.Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace XLugia.Lib.XLog
{
    /// <summary>
    /// 日志检索结果对象
    /// </summary>
    public class LogSearchResultModel : IDisposable
    {
        /// <summary>
        /// 日志数据集合
        /// </summary>
        public List<LogModel> data { get; set; }

        /// <summary>
        /// 当前页索引号
        /// </summary>
        public long pageIndex { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// 日志数据总页数
        /// </summary>
        public long totalPageCount { get; set; }

        /// <summary>
        /// 日志数据总记录数
        /// </summary>
        public long totalDataCount { get; set; }

        /// <summary>
        /// 日志检索结果对象
        /// </summary>
        public LogSearchResultModel()
        {
            try
            {
                data = new List<LogModel>();
                pageIndex = 0;
                pageSize = 100;
                totalPageCount = 0;
                totalDataCount = 0;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogSearchResultModel InitializeComponent");
            }
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            try
            {
                this.data = null;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "LogSearchResultModel Dispose");
            }
        }
    }
}
