using Extended.Tool.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Extended.Log
{
    /// <summary>
    /// exLog检索缓冲区
    /// </summary>
    public class LogSearchBuffer
    {
        public LogSearchBuffer(long buffSize)
        {
            rowIndexBuffer = new long[buffSize];
            removeBufferCount = 0;
        }

        /// <summary>
        /// 检索结果行集合
        /// </summary>
        public long[] rowIndexBuffer { get; set; }

        long removeBufferCount = 0;

        public void removeBufferByRowIndex(long rowIndex)
        {
            var startTime = DateTime.Now;
            ParallelLoopResult result = Parallel.For(0, rowIndexBuffer.Length, (i,state) =>
            {
                try
                {
                    if (rowIndexBuffer[i] == rowIndex)
                    {
                        rowIndexBuffer[i] = -1;
                        removeBufferCount++;
                        state.Break();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            if ((DateTime.Now - startTime).TotalSeconds > LogAttribute.logBufferCommandTimeOut)
            {
                throw new Exception("RemoveBufferByRowIndex Timeout.");
            }
        }

        public void reBuildBuff()
        {
            if (removeBufferCount == 0) return;
            rowIndexBuffer = rowIndexBuffer.Where((a) => { return a >= 0; }).ToArray();
            removeBufferCount = 0;
        }

        public void Dispose()
        {
            this.rowIndexBuffer = null;
        } 
    }
}
