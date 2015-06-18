using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using XLugia.Lib.XLog.Lib;

namespace XLugia.Lib.XLog.Base
{
    internal class DirectoryController
    {
        private static DirectoryController _instance = new DirectoryController();
        public static DirectoryController getIns() { return _instance; }

        public string repairDirPath(string dirPath)
        {
            try
            {
                string tmpPath = dirPath;
                string tmp = tmpPath.Remove(0, tmpPath.Length - 1);
                if (!tmp.Equals("\\"))
                {
                    return dirPath + "\\";
                }
                return dirPath;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "DirectoryController repairDirPath");
                return dirPath;
            }
        }

        public bool isDirectory(string dirPath)
        {
            try
            {
                return Directory.Exists(dirPath);
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "DirectoryController isDirectory");
                return false;
            }
        }

        public bool createDirectory(string dirPath)
        {
            try
            {
                if (!isDirectory(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                SimpleLogController.writeErrorLog(ex, "DirectoryController createDirectory");
                return false;
            }
        }
    }
}
