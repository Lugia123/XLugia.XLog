using XLugia.Lib.XTool.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLugia.Lib.XTool.OS
{
    public class NetController
    {
        private static NetController _instance = new NetController();
        public static NetController getIns() { return _instance; }

        /// <summary>
        /// 将IPv4格式的字符串转换为int型表示
        /// </summary>
        /// <param name="ipv4">IPv4格式的字符</param>
        public int convertIPv4ToNumber(string ipv4)
        {
            try
            {
                //将目标IP地址字符串strIPAddress转换为数字
                string[] arrayIP = ipv4.Split('.');
                int sip1 = Int32.Parse(arrayIP[0]);
                int sip2 = Int32.Parse(arrayIP[1]);
                int sip3 = Int32.Parse(arrayIP[2]);
                int sip4 = Int32.Parse(arrayIP[3]);
                int tmpIpNumber;
                tmpIpNumber = sip1 * 256 * 256 * 256 + sip2 * 256 * 256 + sip3 * 256 + sip4;
                return tmpIpNumber;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "NetController convertIPv4ToNumber");
                return 0;
            }
        }

        /// <summary>
        /// 将int型表示的IP还原成正常IPv4格式。
        /// </summary>
        /// <param name="number">int型表示的IP</param>
        public string convertNumberToIPv4(int number)
        {
            try
            {
                int tempIPAddress;
                //将目标整形数字intIPAddress转换为IP地址字符串    //-1062731518 192.168.1.2 
                //-1062731517 192.168.1.3 
                if (number >= 0)
                {
                    tempIPAddress = number;
                }
                else
                {
                    tempIPAddress = number + 1;
                }
                int s1 = tempIPAddress / 256 / 256 / 256;
                int s21 = s1 * 256 * 256 * 256;
                int s2 = (tempIPAddress - s21) / 256 / 256;
                int s31 = s2 * 256 * 256 + s21;
                int s3 = (tempIPAddress - s31) / 256;
                int s4 = tempIPAddress - s3 * 256 - s31;
                if (number < 0)
                {
                    s1 = 255 + s1;
                    s2 = 255 + s2;
                    s3 = 255 + s3;
                    s4 = 255 + s4;
                }
                return s1.ToString() + "." + s2.ToString() + "." + s3.ToString() + "." + s4.ToString();
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "NetController convertNumberToIPv4");
                return "";
            }
        }

        public string getIPv4()
        {
            try
            {
                string ipv4 = "";
                System.Net.IPHostEntry ipHost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                for (int i = 0; i < ipHost.AddressList.Length; i++)
                {
                    System.Net.IPAddress ipAddr = ipHost.AddressList[i];
                    if (ipAddr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) ipv4 += ipAddr.ToString() + ";";
                }
                return ipv4.TrimEnd(';');
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "NetController getIPv4");
                return "";
            }
        }

        /// <summary>
        /// 获取某http地址返回内容
        /// </summary>
        public string getUrlContext(string url)
        {
            try
            {
                System.Net.WebRequest webRequest = System.Net.WebRequest.Create(url);
                System.Net.WebResponse webResponse = webRequest.GetResponse();
                using (System.IO.Stream stream = webResponse.GetResponseStream())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "NetController getUrlContext");
                return "";
            }
        }
    }
}
