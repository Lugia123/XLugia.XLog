using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XLugia.Lib.XTool.Common
{
    public class Check
    {
        private static Check _instance = new Check();
        public static Check getIns() { return _instance; }

        /// <summary>
        /// 是否是数字
        /// </summary>
        public bool isNumber(params object[] args)
        {
            try
            {
                if (isEmpty(args)) return false;

                Regex rx = new Regex(@"^\d*$");
                for (int i = 0; i < args.Length; i++)
                {
                    if (isEmpty(args[i])) return false;
                    if (!rx.IsMatch(args[i].ToString())) return false;
                }
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "Check isNumber");
            }
            return true;
        }

        /// <summary> 
        /// 判断是否 = null 或 = ""
        /// </summary>
        public bool isEmpty(params object[] args)
        {
            try
            {
                if (args == null) return true;
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == null) return true;
                    if ("".Equals(args[i].ToString())) return true;
                }
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "Check isEmpty");
            }
            return false;
        }

        /// <summary> 
        /// 判断str是否包含制定的字符串
        /// </summary>
        public bool isIndexOf(string str, params object[] args)
        {
            try
            {
                if (isEmpty(str)) return false;
                for (int i = 0; i < args.Length; i++)
                {
                    if (isEmpty(args[i])) continue;
                    if (str.IndexOf(args[i].toString()) >= 0) return true;
                }
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "Check isIndexOf");
            }
            return false;
        }
    }
}
