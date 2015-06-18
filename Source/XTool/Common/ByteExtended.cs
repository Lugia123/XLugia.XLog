using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLugia.Lib.XTool.Common
{
    public static class ByteExtended
    {
        public static byte[] breakUp(this byte[] val, long index, long count)
        {
            try
            {
                byte[] result = new byte[count];
                Array.Copy(val, index, result, 0, count);
                return result;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "ByteExtended breakUp");
                return new byte[0];
            }
        }
        public static byte[] insertUnicodeString(this byte[] val, string content, int offsetIndex = 0)
        {
            try
            {
                char[] contentChar = content.ToCharArray();
                Encoding.Unicode.GetBytes(contentChar, 0, contentChar.Length, val, offsetIndex);
                return val;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "ByteExtended insertUnicodeString");
                return new byte[0];
            }
        }

        /// <summary>
        /// 将字节数组转换成Unicode编码的字符串。
        /// </summary>
        /// <param name="val">字节数组</param>
        /// <param name="offsetIndex">第一个要解码的字节的索引。</param>
        /// <param name="length">要解码的字节数。</param>
        /// <returns>Unicode编码的字符串。</returns>
        public static string toUnicodeString(this byte[] val, int index, int count)
        {
            try
            {
                return Encoding.Unicode.GetString(val, index, count).TrimEnd('\0');
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "ByteExtended toUnicodeString");
                return "";
            }
        }
    }

}
