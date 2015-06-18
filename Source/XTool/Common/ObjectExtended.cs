using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLugia.Lib.XTool.Common
{
    public static class ObjectExtended
    {
        /// <summary> 
        /// SQL用，Replace("'", "''")
        /// </summary>
        public static string quote(this Object val)
        {
            try
            {
                if (val == null) return "";
                return val.ToString().Replace("'", "''");
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "ObjectExtended quote");
                return "";
            }
        }

        /// <summary>
        /// SQL用，"'" + val.quote() + "'"
        /// </summary>
        public static string sqlString(this Object val)
        {
            try
            {
                return "'" + val.quote() + "'";
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "ObjectExtended sqlString");
                return "''";
            }
        }

        /// <summary>
        /// object转Int32，空或NUll返回0，转换失败时返回0。
        /// </summary>
        public static int toInt32(this Object val)
        {
            try
            {
                if (val == null) return 0;
                if (string.IsNullOrEmpty(val.toString())) return 0;
                return Convert.ToInt32(val);
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "ObjectExtended toInt32");
                return 0;
            }
        }

        /// <summary>
        /// object转decimal，空或NUll返回0，转换失败时返回0。
        /// </summary>
        public static decimal toDecimal(this Object val)
        {
            try
            {
                if (val == null) return 0;
                if (string.IsNullOrEmpty(val.toString())) return 0;
                return Convert.ToDecimal(val);
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "ObjectExtended toDecimal");
                return 0;
            }
        }

        /// <summary>
        /// object转double，空或NUll返回0，转换失败时返回0。
        /// </summary>
        public static double toDouble(this Object val)
        {
            try
            {
                if (val == null) return 0;
                if (string.IsNullOrEmpty(val.toString())) return 0;
                return Convert.ToDouble(val);
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "ObjectExtended toDouble");
                return 0;
            }
        }

        /// <summary>
        /// object转string，如果是Null会返回空字符串，转换失败时返回空字符串。
        /// </summary>
        public static string toString(this Object val)
        {
            try
            {
                if (val == null) return "";
                return val.ToString();
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "ObjectExtended toString");
                return "";
            }
        }

        /// <summary>
        /// 对象为1、true、yes、ok时，返回true
        /// 其余情况返回false，转换失败时返回false。
        /// </summary>
        public static bool toBoolean(this Object val)
        {
            try
            {
                if (val == null) return false;
                switch (val.ToString().ToLower())
                {
                    case "1":
                    case "true":
                    case "yes":
                    case "ok":
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "ObjectExtended toString");
                return false;
            }
        }
    }

}
