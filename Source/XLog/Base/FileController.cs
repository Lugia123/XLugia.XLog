using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using XLugia.Lib.XLog.Lib;

namespace XLugia.Lib.XLog.Base
{
    internal class FileController
    {
        private static FileController _instance = new FileController();
        public static FileController getIns() { return _instance; }

        public bool isFileExists(string filePath)
        {
            try
            {
                return File.Exists(filePath);
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "FileController isFileExists");
                return false;
            }
        }

        public bool createFile(string filePath, string content)
        {
            try
            {
                if (isFileExists(filePath))
                {
                    return false;
                }

                DirectoryController.getIns().createDirectory(getDirectoryName(filePath));
                using (FileStream fs = File.Create(filePath))
                {
                    //byte[] info = new UTF8Encoding(true).GetBytes(content);
                    //info = Encoding.Convert(Encoding.UTF8, Encoding.Default, info);
                    byte[] info = System.Text.Encoding.Default.GetBytes(content);
                    fs.Write(info, 0, info.Length);
                    fs.Close();
                    info = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "FileController createFile");
                return false;
            }
        }

        public bool saveFile(string dirPath, string fileName, string content)
        {
            try
            {
                dirPath = DirectoryController.getIns().repairDirPath(dirPath);
                return saveFile(dirPath + fileName, content);
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "FileController saveFile");
                return false;
            }
        }

        public bool saveFile(string filePath, string content)
        {
            try
            {
                if (!isFileExists(filePath))
                {
                    return createFile(filePath, content);
                }

                using (FileStream fs = File.Open(filePath, FileMode.Append))
                {
                    //byte[] info = new UTF8Encoding(true).GetBytes(content);
                    //info = Encoding.Convert(Encoding.UTF8, Encoding.Default, info);
                    byte[] info = System.Text.Encoding.Default.GetBytes(content);
                    fs.Write(info, 0, info.Length);
                    fs.Close();
                    info = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "FileController saveFile");
                return false;
            }
        }

        public string getDirectoryName(string filePath)
        {
            try
            {
                return Path.GetDirectoryName(filePath);
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "FileController getDirectoryName");
                return "";
            }
        }
    }
}
