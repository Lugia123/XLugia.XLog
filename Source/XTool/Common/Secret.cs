using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace XLugia.Lib.XTool.Common
{
    public class Secret
    {
        private static Secret _instance = new Secret();
        public static Secret getIns() { return _instance; }

        public string desDefaultPassword = "Google+Apple-Microsoft";

        #region md5
        /// <summary>
        /// 获取对应md5
        /// </summary>
        public string encryptMD5(byte[] bytes)
        {
            string r = "";
            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] bytHash = md5.ComputeHash(bytes);
                for (int i = 0; i < bytHash.Length; i++)
                {
                    r += bytHash[i].ToString("x").PadLeft(2, '0');
                }
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "Secret encryptMD5");
            }
            return r.ToLower();
        }
        #endregion

        #region des3
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="original">加密字符串</param>
        /// <param name="password">加密因子</param>
        /// <returns>加密后的字符串</returns>
        public string encryptDES3(string original, string password = "")
        {
            string encrypted = "";
            try
            {
                TripleDESCryptoServiceProvider des;
                MD5CryptoServiceProvider hashmd5;
                byte[] pwdhash, buff;

                if (Check.getIns().isEmpty(original)) return "";

                if (Check.getIns().isEmpty(password)) password = desDefaultPassword;

                hashmd5 = new MD5CryptoServiceProvider();
                pwdhash = hashmd5.ComputeHash(UnicodeEncoding.Unicode.GetBytes(password));
                hashmd5 = null;

                des = new TripleDESCryptoServiceProvider();
                des.Key = pwdhash;
                des.Mode = CipherMode.ECB;

                buff = UnicodeEncoding.Unicode.GetBytes(original);
                encrypted = Convert.ToBase64String(
                    des.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length)
                    );
                des = null;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "Secret encryptDES3");
            }
            return encrypted;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="original">解密字符串</param>
        /// <param name="password">解密因子</param>
        /// <returns>解密后的字符串</returns>
        public string decryptDES3(string original, string password = "")
        {
            string decrypted = "";
            try
            {
                TripleDESCryptoServiceProvider des;
                MD5CryptoServiceProvider hashmd5;
                byte[] pwdhash, buff;

                if (Check.getIns().isEmpty(original)) return "";

                if (Check.getIns().isEmpty(password)) password = desDefaultPassword;

                hashmd5 = new MD5CryptoServiceProvider();
                pwdhash = hashmd5.ComputeHash(UnicodeEncoding.Unicode.GetBytes(password));
                hashmd5 = null;

                des = new TripleDESCryptoServiceProvider();
                des.Key = pwdhash;
                des.Mode = CipherMode.ECB;

                buff = Convert.FromBase64String(original);

                decrypted = UnicodeEncoding.Unicode.GetString(
                    des.CreateDecryptor().TransformFinalBlock(buff, 0, buff.Length)
                    );
                des = null;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "Secret decryptDES3");
            }
            return decrypted;
        }
        #endregion

        #region rsa
        #endregion
    }
}
