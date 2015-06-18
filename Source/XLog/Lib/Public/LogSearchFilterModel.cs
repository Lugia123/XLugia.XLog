using XLugia.Lib.XLog.Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace XLugia.Lib.XLog
{
    /// <summary>
    /// 日志检索条件对象
    /// </summary>
    public class LogSearchFilterModel
    {
        /// <summary>
        /// 当前页索引号
        /// </summary>
        public long pageIndex
        {
            get { return _pageIndex; }
            set
            {
                if (value >= 0) _pageIndex = value;
            }
        }
        long _pageIndex = 0;

        /// <summary>
        /// 每页记录数，必须大于1小于10000，默认200。
        /// </summary>
        public int pageSize
        {
            get { return _pageSize; }
            set
            {
                try
                {
                    if (value >= 1 && value <= 10000)
                    {
                        _pageSize = value;
                    }
                    else
                    {
                        _pageSize = 200;
                    }
                }
                catch (Exception ex)
                {
                    SimpleLogController.writeErrorLog(ex, "LogSearchFilterModel set pageSize");
                }
            }
        }
        int _pageSize;

        /// <summary>
        /// 筛选日志开始时间
        /// </summary>
        public DateTime? startDateTime { get; set; }

        /// <summary>
        /// 筛选日志开始时间
        /// </summary>
        public DateTime? endDateTime { get; set; }


        /// <summary>
        /// 排序方式
        /// </summary>
        public enum OrderTypes
        {
            /// <summary>
            /// 降序
            /// </summary>
            Desc = 0,

            /// <summary>
            /// 升序
            /// </summary>
            Asc = 1,
        }

        /// <summary>
        /// 筛选日志时间排序方式
        /// </summary>
        public OrderTypes logDateTimeOrder = OrderTypes.Asc;

        /// <summary>
        /// 首条记录所在行索引号
        /// </summary>
        internal long startRowIndex
        {
            get { return _startRowIndex; }
            set
            {
                try
                {
                    if (value < _startRowIndex || _startRowIndex == -1) _startRowIndex = value;
                }
                catch (Exception ex)
                {
                    SimpleLogController.writeErrorLog(ex, "LogSearchFilterModel set startRowIndex");
                }
            }
        }
        long _startRowIndex = -1;

        /// <summary>
        /// 最末条记录所在行索引号
        /// </summary>
        internal long endRowIndex
        {
            get { return _endRowIndex; }
            set
            {
                try
                {
                    if (value > _endRowIndex || _endRowIndex == -1) _endRowIndex = value;
                }
                catch (Exception ex)
                {
                    SimpleLogController.writeErrorLog(ex, "LogSearchFilterModel set endRowIndex");
                }
            }
        }
        long _endRowIndex = -1;
    }
}
