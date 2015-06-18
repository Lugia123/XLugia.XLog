using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using XLugia.Lib.XTool.Common;

namespace XLugia.Lib.XTool.OS
{
    public class DirectoryController
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
                LogController.writeErrorLog(ex, "DirectoryController repairDirPath");
                return dirPath;
            }
        }

        public string[] getFiles(string dirPath, string searchPattern, SearchOption searchOption = SearchOption.AllDirectories)
        {
            string[] filePath = null;
            try
            {
                if (isDirectory(dirPath))
                {
                    filePath = Directory.GetFiles(dirPath, searchPattern, searchOption);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "DirectoryController getFiles");
                return filePath;
            }
        }

        public string[] getFiles(string dirPath, SearchOption searchOption = SearchOption.AllDirectories)
        {
            string[] files = null;
            try
            {
                if (isDirectory(dirPath))
                {
                    files = Directory.GetFiles(dirPath, "*.*", searchOption);
                }
                return files;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "DirectoryController getFiles");
                return files;
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
                LogController.writeErrorLog(ex, "DirectoryController isDirectory");
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
                LogController.writeErrorLog(ex, "DirectoryController createDirectory");
                return false;
            }
        }

        public string[] getDirectories(string dirPath)
        {
            string[] dirs = null;
            try
            {
                if(isDirectory(dirPath))
                {
                    dirs = Directory.GetDirectories(dirPath);
                }
                return dirs;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "DirectoryController getDirectories");
                return dirs;
            }
        }

        public bool renameDirectory(string oldPath, string newPath)
        {
            try
            {
                oldPath = repairDirPath(oldPath);
                newPath = repairDirPath(newPath);

                if (!isDirectory(oldPath))
                {
                    return true;
                }

                Directory.Move(oldPath, newPath);
                return true;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "DirectoryController renameDirectory");
                return false;
            }
        }

        public bool deleteDirectory(string dirPath)
        {
            try
            {
                if (isDirectory(dirPath))
                {
                    Directory.Delete(dirPath, true);
                }

                return true;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "DirectoryController deleteDirectory");
                return false;
            }
        }

        public string getDirectoryName(string dirPath)
        {
            try
            {
                dirPath = repairDirPath(dirPath);
                string[] s = dirPath.Replace("\\\\", "\\").Split('\\');
                return s[s.Length - 2].ToString();
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "DirectoryController getDirectoryName");
                return "";
            }
        }
    }
}
