using XLugia.Lib.XLog.Base;
using XLugia.Lib.XLog.Lib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XLugia.Lib.XLog.IO
{
    /// <summary>
    /// 读取日志
    /// </summary>
    internal class FileReader
    {
        private static FileReader instance = new FileReader();
        public static FileReader getIns() { return instance; }

        /// <summary>
        /// 读取开始时间，用于计算耗时以及读取超时用
        /// </summary>
        DateTime readLogStartTime = DateTime.Now;

        /// <summary>
        /// 读取日志
        /// </summary>
        /// <param name="logFilePath">日志文件路径</param>
        /// <param name="logSearchFilterModel">日志检索条件</param>
        /// <returns>LogSearchResultModel对象</returns>
        public LogSearchResultModel readLog(string logFilePath, LogSearchFilterModel logSearchFilterModel)
        {
            try
            {
                if (string.IsNullOrEmpty(logFilePath)) return null;
                if (!FileController.getIns().isFileExists(logFilePath)) return null;

                readLogStartTime = DateTime.Now;
                LogSearchResultModel result = new LogSearchResultModel();
                using (FileStream fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 8, true))
                {
                    //时间检索
                    searchByTime(logSearchFilterModel, fs);

                    if (logSearchFilterModel.startRowIndex > logSearchFilterModel.endRowIndex) return result;
                    if (logSearchFilterModel.startRowIndex == -1 || logSearchFilterModel.endRowIndex == -1) return result;

                    //获取分页数据
                    if (logSearchFilterModel.startRowIndex != -1 && logSearchFilterModel.endRowIndex != -1)
                    {
                        result.totalDataCount = logSearchFilterModel.endRowIndex - logSearchFilterModel.startRowIndex + 1;
                    }
                    else
                    {
                        result.totalDataCount = getLogDataCount(fs.Length);
                    }

                    result.pageSize = logSearchFilterModel.pageSize;
                    result.totalPageCount = (long)Math.Ceiling(result.totalDataCount / (result.pageSize * 1.0));
                    result.pageIndex = logSearchFilterModel.pageIndex + 1 > result.totalPageCount ? result.totalPageCount - 1 : logSearchFilterModel.pageIndex;
                    if (result.totalDataCount > 0) readLogData(logSearchFilterModel, fs, result);

                    fs.Close();
                }

                return result;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "FileReader readLog");
                return null;
            }
        }

        #region 时间检索
        /// <summary>
        /// 根据开始时间和结束时间过滤
        /// </summary>
        /// <param name="logSearchFilterModel">检索条件</param>
        /// <param name="fs">日志文件流</param>
        private void searchByTime(LogSearchFilterModel logSearchFilterModel, FileStream fs)
        {
            try
            {
                searchByStartTime(logSearchFilterModel, fs);

                searchByEndTime(logSearchFilterModel, fs);
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "FileReader searchByTime");
            }
        }

        /// <summary>
        /// 检索开始时间所在行
        /// </summary>
        private void searchByStartTime(LogSearchFilterModel logSearchFilterModel, FileStream fs)
        {
            try
            {
                //时间为Null时不筛选
                if (!logSearchFilterModel.startDateTime.HasValue)
                {
                    logSearchFilterModel.startRowIndex = 0;
                    return;
                }

                DateTime startTime = logSearchFilterModel.startDateTime.Value;

                //日志记录行数
                long logDataRowCount = getLogDataCount(fs.Length);

                //检索偏移行
                long searchOffsetRowIndex = 0;

                #region 检索开始时间
                while (logDataRowCount > 2)
                {
                    if ((DateTime.Now - readLogStartTime).TotalSeconds > LogAttribute.readLogTimeOut)
                    {
                        throw new Exception("Read File Timeout.");
                    }

                    //检索行索引，从0开始
                    long searchRowIndex = (long)Math.Ceiling(logDataRowCount / 2.0) - 1;

                    //日志时间，升序排列
                    string logDateTime = getLogDateTime(fs, searchRowIndex + searchOffsetRowIndex);


                    //如果开始时间大于当前检索到的日志时间，则向下搜索
                    if (startTime > DateTime.Parse(logDateTime))
                    {
                        //设置检索偏移，向下检索
                        searchOffsetRowIndex += (searchRowIndex + 1);

                        //剩余要搜索的行数
                        logDataRowCount -= (searchRowIndex + 1);
                    }
                    //如果开始时间等于当前检索到的日志时间，则向上搜索(防止有时间一样的记录)
                    else if (startTime == DateTime.Parse(logDateTime))
                    {
                        //标记日期范围数据开始行
                        logSearchFilterModel.startRowIndex = searchOffsetRowIndex + searchRowIndex;

                        //剩余要搜索的行数
                        logDataRowCount = searchRowIndex;
                    }
                    //如果开始时间小于当前检索到的日志时间，则向上搜索
                    else if (startTime < DateTime.Parse(logDateTime))
                    {
                        //标记日期范围数据开始行
                        logSearchFilterModel.startRowIndex = searchOffsetRowIndex + searchRowIndex;

                        //剩余要搜索的行数
                        logDataRowCount = searchRowIndex;
                    }
                }

                //剩余最后2行及以下时，全部检索
                for (int i = 0; i < logDataRowCount; i++)
                {
                    //日志时间，升序排列
                    string logDateTime = getLogDateTime(fs, i + searchOffsetRowIndex);

                    //如果开始时间大于当前检索到的日志时间，继续检索
                    if (startTime > DateTime.Parse(logDateTime))
                    {
                        continue;
                    }
                    //如果开始时间等于当前检索到的日志时间，继续检索
                    else if (startTime == DateTime.Parse(logDateTime))
                    {
                        //标记日期范围数据开始行
                        logSearchFilterModel.startRowIndex = searchOffsetRowIndex + i;
                    }
                    //如果开始时间小于当前检索到的日志时间，不需要在继续检索
                    else if (startTime < DateTime.Parse(logDateTime))
                    {
                        //标记日期范围数据开始行
                        logSearchFilterModel.startRowIndex = searchOffsetRowIndex + i;
                        break;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "FileReader searchByStartTime");
            }
        }

        /// <summary>
        /// 检索结束时间所在行
        /// </summary>
        private void searchByEndTime(LogSearchFilterModel logSearchFilterModel, FileStream fs)
        {
            try
            {
                //日志记录行数
                long logDataRowCount = getLogDataCount(fs.Length);

                //时间为Null时不筛选
                if (!logSearchFilterModel.endDateTime.HasValue)
                {
                    logSearchFilterModel.endRowIndex = logDataRowCount - 1;
                    return;
                }

                DateTime endTime = logSearchFilterModel.endDateTime.Value;

                //检索偏移行
                long searchOffsetRowIndex = 0;

                #region 检索结束时间
                while (logDataRowCount > 2)
                {
                    if ((DateTime.Now - readLogStartTime).TotalSeconds > LogAttribute.readLogTimeOut)
                    {
                        throw new Exception("Read File Timeout.");
                    }

                    //检索行索引，从0开始
                    long searchRowIndex = (long)Math.Ceiling(logDataRowCount / 2.0) - 1;

                    //日志时间，升序排列
                    string logDateTime = getLogDateTime(fs, searchRowIndex + searchOffsetRowIndex);

                    //如果结束时间大于当前检索到的日志时间，则向下搜索
                    if (endTime > DateTime.Parse(logDateTime))
                    {
                        //标记日期范围数据开始行
                        logSearchFilterModel.endRowIndex = searchOffsetRowIndex + searchRowIndex;

                        //设置检索偏移，向下检索
                        searchOffsetRowIndex += (searchRowIndex + 1);

                        //剩余要搜索的行数
                        logDataRowCount -= (searchRowIndex + 1);
                    }
                    //如果结束时间等于当前检索到的日志时间，则向下搜索(防止有时间一样的记录)
                    else if (endTime == DateTime.Parse(logDateTime))
                    {
                        //标记日期范围数据开始行
                        logSearchFilterModel.endRowIndex = searchOffsetRowIndex + searchRowIndex;

                        //设置检索偏移，向下检索
                        searchOffsetRowIndex += (searchRowIndex + 1);

                        //剩余要搜索的行数
                        logDataRowCount -= (searchRowIndex + 1);
                    }
                    //如果结束时间小于当前检索到的日志时间，则向上搜索
                    else if (endTime < DateTime.Parse(logDateTime))
                    {
                        //剩余要搜索的行数
                        logDataRowCount = searchRowIndex;
                    }
                }

                for (int i = 0; i < logDataRowCount; i++)
                {
                    //日志时间，升序排列
                    string logDateTime = getLogDateTime(fs, i + searchOffsetRowIndex);

                    //如果结束时间大于当前检索到的日志时间，继续检索
                    if (endTime > DateTime.Parse(logDateTime))
                    {
                        //标记日期范围数据开始行
                        logSearchFilterModel.endRowIndex = searchOffsetRowIndex + i;
                    }
                    //如果结束时间等于当前检索到的日志时间，继续检索
                    else if (endTime == DateTime.Parse(logDateTime))
                    {
                        //标记日期范围数据开始行
                        logSearchFilterModel.endRowIndex = searchOffsetRowIndex + i;
                    }
                    //如果结束时间小于当前检索到的日志时间，不需要在继续检索
                    else if (endTime < DateTime.Parse(logDateTime))
                    {
                        break;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "FileReader searchByEndTime");
            }
        }

        /// <summary>
        /// 读取日志时间
        /// </summary>
        private string getLogDateTime(FileStream fs, long rowIndex)
        {
            try
            {
                //时间所在位置
                fs.Position = rowIndex * LogModel.logTotalDoubleByteLength + (LogModel.logTotalDoubleByteLength - LogModel.logEtcDoubleByteLength - LogModel.logDateTimeDoubleByteLength);

                byte[] buff = new byte[LogModel.logDateTimeDoubleByteLength];
                fs.Read(buff, 0, LogModel.logDateTimeDoubleByteLength);

                //日志时间，升序排列
                return buff.toUnicodeString(0, LogModel.logDateTimeDoubleByteLength);
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "FileReader getLogDateTime");
                return "";
            }
        }
        #endregion

        #region 读取日志数据
        /// <summary>
        /// 读取日志数据
        /// </summary>
        /// <param name="logSearchFilterModel">检索条件</param>
        /// <param name="fs">日志文件流</param>
        /// <param name="logSearchResultModel">检索结果</param>
        private void readLogData(LogSearchFilterModel logSearchFilterModel, FileStream fs, LogSearchResultModel logSearchResultModel)
        {
            try
            {
                logSearchResultModel.data = new List<LogModel>();

                int size = logSearchResultModel.pageSize;
                if (logSearchResultModel.totalDataCount - logSearchResultModel.pageSize * (logSearchResultModel.pageIndex + 1) < 0)
                {
                    size = (int)(logSearchResultModel.totalDataCount - logSearchResultModel.pageSize * logSearchResultModel.pageIndex);
                }

                //升序
                var startBufferIndex = logSearchResultModel.pageIndex * logSearchResultModel.pageSize;

                //设置读取偏移量
                if (logSearchFilterModel.logDateTimeOrder == LogSearchFilterModel.OrderTypes.Asc)
                {
                    //升序
                    fs.Position = (logSearchFilterModel.startRowIndex + logSearchResultModel.pageIndex * logSearchResultModel.pageSize) * LogModel.logTotalDoubleByteLength;
                }
                else
                {
                    //降序
                    fs.Position = (logSearchFilterModel.endRowIndex + 1 - logSearchResultModel.pageIndex * logSearchResultModel.pageSize - size) * LogModel.logTotalDoubleByteLength;
                }

                byte[] buff = new byte[LogModel.logTotalDoubleByteLength * size];
                fs.Read(buff, 0, buff.Length);

                //日志记录条数
                ParallelLoopResult result = Parallel.For(0, size, (i) =>
                {
                    try
                    {
                        IOLock.getIns().acquireWriterLock();
                        try
                        {
                            logSearchResultModel.data.Add(new LogModel(buff.breakUp(LogModel.logTotalDoubleByteLength * i, LogModel.logTotalDoubleByteLength))
                            {
                                id = logSearchFilterModel.logDateTimeOrder == LogSearchFilterModel.OrderTypes.Asc ? i + 1 : size - i
                            });
                        }
                        catch (Exception ex)
                        {
                            SimpleLogController.writeErrorLog(ex, "FileReader readLogData Parallel.For");
                        }
                        finally
                        {
                            IOLock.getIns().releaseWriterLock();
                        }
                    }
                    catch (Exception ex)
                    {
                        SimpleLogController.writeErrorLog(ex, "FileReader readLogData ParallelLoopResult");
                    }
                });

                if ((DateTime.Now - readLogStartTime).TotalSeconds > LogAttribute.readLogTimeOut)
                {
                    throw new Exception("Read File Timeout.");
                }

                if (!result.IsCompleted)
                {
                    throw new Exception("Result Not Completed.");
                }
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "FileReader readLogData");
            }
        }
        #endregion

        #region 其他
        /// <summary>
        /// 计算日志总记录条数
        /// </summary>
        /// <param name="logDataLength">日志文件长度</param>
        /// <returns>日志总记录条数</returns>
        private long getLogDataCount(long logDataLength)
        {
            try
            {
                return logDataLength / LogModel.logTotalDoubleByteLength;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "FileReader getLogDataCount");
                return 0;
            }
        }

        //private void initFullBTree()
        //{
        //    //满二叉树节点
        //    double btreeNodes = 1;
        //    //fullBTree.Add(btreeNodes, btreeNodes);
        //    Console.WriteLine("1     " + btreeNodes.ToString());
        //    for (int i = 1; i <= 100; i++)
        //    {
        //        btreeNodes += (long)Math.Pow(2, i);
        //        //fullBTree.Add(btreeNodes, btreeNodes);
        //        Console.WriteLine((i + 1).ToString() + "     " + btreeNodes.ToString("###,###,###,###,###,###"));
        //    }
        //}

        //private long getBTreeHeight(long nodeNumber)
        //{
        //    //计算二叉树高
        //    //总结点k、书高h
        //    //完全二叉树
        //    //2^(h-1)<k<2^h-1
        //    //h=log2(k)+1
        //    //满二叉树
        //    //k=2^h-1
        //    //h=log2(k+1)
        //    //满二叉树
        //    //满二叉树
        //    Dictionary<long, long> fullBTree = new Dictionary<long, long>();
        //    if (fullBTree.ContainsKey(nodeNumber))
        //    {
        //        //满二叉树
        //        return (long)Math.Floor(Math.Log(nodeNumber + 1, 2));
        //    }
        //    else
        //    {
        //        //完全二叉树
        //        return (long)Math.Floor(Math.Log(nodeNumber, 2) + 1);
        //    }
        //}
        #endregion
    }
}
