using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLugia.Lib.XTool.Common
{
    public class Transform
    {
        private static Transform _instance = new Transform();
        public static Transform getIns() { return _instance; }

        #region math
        char[] rDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
                        , 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M'
                        , 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
                        , 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm'
                        , 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};

        /// <summary>
        /// 将指定基数的数字的 System.String 表示形式转换为等效的 64 位有符号整数。
        /// </summary>
        /// <param name="value">包含数字的 System.String。</param>
        /// <param name="fromBase">value 中数字的基数，它必须是[2,36]</param>
        /// <returns>等效于 value 中的数字的 64 位有符号整数。- 或 - 如果 value 为 null，则为零。</returns>
        private long x2h(string value, int fromBase)
        {
            try
            {
                value = value.Trim();
                if (string.IsNullOrEmpty(value))
                {
                    return 0L;
                }

                string sDigits = new string(rDigits, 0, fromBase);
                long result = 0;
                //value = reverse(value).ToUpper(); 1
                //value = value.ToUpper();// 2
                for (int i = 0; i < value.Length; i++)
                {
                    if (!sDigits.Contains(value[i].ToString()))
                    {
                        throw new ArgumentException(string.Format("The argument \"{0}\" is not in {1} system.", value[i], fromBase));
                    }
                    else
                    {
                        try
                        {
                            //result += (long)Math.Pow(fromBase, i) * getcharindex(rDigits, value[i]); 1
                            result += (long)Math.Pow(fromBase, i) * getCharIndex(rDigits, value[value.Length - i - 1]);//   2
                        }
                        catch
                        {
                            throw new OverflowException("运算溢出.");
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "Transform x2h");
                return 0L;
            }
        }

        private int getCharIndex(char[] arr, char value)
        {
            try
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] == value)
                    {
                        return i;
                    }
                }
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "Transform getCharIndex");
            }
            return 0;
        }

        //long转化为toBase进制
        private string h2x(long value, int toBase)
        {
            try
            {
                int digitIndex = 0;
                long longPositive = Math.Abs(value);
                int radix = toBase;
                char[] outDigits = new char[63];

                for (digitIndex = 0; digitIndex <= 64; digitIndex++)
                {
                    if (longPositive == 0) { break; }

                    outDigits[outDigits.Length - digitIndex - 1] =
                        rDigits[longPositive % radix];
                    longPositive /= radix;
                }

                return new string(outDigits, outDigits.Length - digitIndex, digitIndex);
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "Transform h2x");
            }
            return "";
        }

        /// <summary>
        /// 任意进制转换,将fromBase进制表示的value转换为toBase进制
        /// </summary>
        public string convertNumber(string value, int fromBase, int toBase)
        {
            try
            {
                if (string.IsNullOrEmpty(value.Trim()))
                {
                    return string.Empty;
                }

                if (fromBase < 2 || fromBase > 62)
                {
                    throw new ArgumentException(String.Format("The fromBase radix \"{0}\" is not in the range 2..62.", fromBase));
                }

                if (toBase < 2 || toBase > 62)
                {
                    throw new ArgumentException(String.Format("The toBase radix \"{0}\" is not in the range 2..62.", toBase));
                }

                long m = x2h(value, fromBase);
                string r = h2x(m, toBase);
                return r;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "Transform convertNumber");
            }
            return "";
        }
        #endregion
    }
}
