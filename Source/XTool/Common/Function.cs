using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace XLugia.Lib.XTool.Common
{
    public class Function
    {
        private static Function _instance = new Function();
        public static Function getIns() { return _instance; }

        /// <summary>
        /// 返回两个时间之间的时间间隔，可以设置间隔单位。
        /// </summary>
        /// <param name="type">时间间隔单位，秒ss、分mm、时hh、天dd、周w、月M、季度qq、年yy</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public long dateDiff(string type, DateTime startTime, DateTime endTime)
        {
            long timeSpanValue = 0;
            try
            {
                System.TimeSpan timeSpan = new System.TimeSpan(endTime.Ticks - startTime.Ticks);
                switch (type)
                {
                    case "ss":
                        timeSpanValue = (long)timeSpan.TotalSeconds;
                        break;
                    case "mm":
                        timeSpanValue = (long)timeSpan.TotalMinutes;
                        break;
                    case "hh":
                        timeSpanValue = (long)timeSpan.TotalHours;
                        break;
                    case "dd":
                        timeSpanValue = (long)timeSpan.Days;
                        break;
                    case "w":
                        timeSpanValue = (long)(timeSpan.Days / 7);
                        break;
                    case "M":
                        timeSpanValue = (long)(timeSpan.Days / 30);
                        break;
                    case "qq":
                        timeSpanValue = (long)((timeSpan.Days / 30) / 3);
                        break;
                    case "yy":
                        timeSpanValue = (long)(timeSpan.Days / 365);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "Function dateDiff");
            }
            return timeSpanValue;
        }
    }
}
