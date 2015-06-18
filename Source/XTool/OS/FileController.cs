using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using XLugia.Lib.XTool.Common;

namespace XLugia.Lib.XTool.OS
{
    public class FileController
    {
        private static FileController _instance = new FileController();
        public static FileController getIns() { return _instance; }

        /// <summary> 
        /// 将文件名中不允许的字符替换为空
        /// </summary>
        public string repairFileName(string fileName)
        {
            try
            {
                if (Common.Check.getIns().isEmpty(fileName)) return "";
                fileName = fileName.Replace("\\", "");
                fileName = fileName.Replace("/", "");
                fileName = fileName.Replace(":", "");
                fileName = fileName.Replace("*", "");
                fileName = fileName.Replace("?", "");
                fileName = fileName.Replace("\"", "");
                fileName = fileName.Replace("<", "");
                fileName = fileName.Replace(">", "");
                fileName = fileName.Replace("|", "");
                return fileName;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController repairFileName");
                return "";
            }
        }

        public bool isFileExists(string filePath)
        {
            try
            {
                return File.Exists(filePath);
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController isFileExists");
                return false;
            }
        }

        public Process runFile(string dirPath, string fileName)
        {
            try
            {
                dirPath = DirectoryController.getIns().repairDirPath(dirPath);
                return runFile(dirPath + fileName);
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController runFile");
                return null;
            }
        }

        public Process runFile(string filePath)
        {
            try
            {
                if (isFileExists(filePath))
                {
                    return Process.Start(filePath);
                }
                return null;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController runFile");
                return null;
            }
        }

        public bool createFile(string dirPath, string fileName, string content)
        {
            try
            {
                dirPath = DirectoryController.getIns().repairDirPath(dirPath);
                return createFile(dirPath + fileName, content);
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController createFile");
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
                LogController.writeErrorLog(ex, "FileController createFile");
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
                LogController.writeErrorLog(ex, "FileController saveFile");
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
                LogController.writeErrorLog(ex, "FileController saveFile");
                return false;
            }
        }

        public bool saveFile(string dirPath, string fileName, byte[] content, FileMode fileMode)
        {
            try
            {
                dirPath = DirectoryController.getIns().repairDirPath(dirPath);
                DirectoryController.getIns().createDirectory(dirPath);
                using (FileStream fs = File.Open(dirPath + fileName, fileMode))
                {
                    fs.Write(content, 0, content.Length);
                    fs.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController saveFile");
                return false;
            }
        }

        public bool saveFile(string dirPath , string fileName , byte[] content)
        {
            try
            {
                dirPath = DirectoryController.getIns().repairDirPath(dirPath);
                DirectoryController.getIns().createDirectory(dirPath);

                using (FileStream fs = File.Open(dirPath + fileName, FileMode.Append))
                {
                    byte[] info = content;
                    fs.Write(info, 0, info.Length);
                    fs.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController saveFile");
                return false;
            }
        }

        public bool saveFile(string dirPath, string fileName, string content, FileMode fileMode)
        {
            try
            {
                dirPath = DirectoryController.getIns().repairDirPath(dirPath);
                DirectoryController.getIns().createDirectory(dirPath);

                using (FileStream fs = File.Open(dirPath + fileName, fileMode))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(content);
                    fs.Write(info, 0, info.Length);
                    fs.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController saveFile");
                return false;
            }
        }

        public bool moveFile(string currentDirPath, string fileName, string targetDirPath)
        {
            try
            {
                currentDirPath = DirectoryController.getIns().repairDirPath(currentDirPath);
                targetDirPath = DirectoryController.getIns().repairDirPath(targetDirPath);
                DirectoryController.getIns().createDirectory(targetDirPath);
                if (!isFileExists(currentDirPath + fileName))
                {
                    return false;
                }
                File.Move(currentDirPath + fileName, targetDirPath + fileName);
                return true;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController moveFile");
                return false;
            }
        }

        public bool copyFile(string currentDirPath, string fileName, string targetDirPath)
        {
            try
            {
                return copyFile(currentDirPath, fileName, targetDirPath, fileName);
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController copyFile");
                return false;
            }
        }

        public bool copyFile(string currentDirPath, string currentFileName, string targetDirPath, string targetFileName)
        {
            try
            {
                currentDirPath = DirectoryController.getIns().repairDirPath(currentDirPath);
                targetDirPath = DirectoryController.getIns().repairDirPath(targetDirPath);
                DirectoryController.getIns().createDirectory(targetDirPath);

                if (!isFileExists(currentDirPath + currentFileName))
                {
                    return false;
                }
                File.Copy(currentDirPath + currentFileName, targetDirPath + targetFileName);
                return true;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController copyFile");
                return false;
            }
        }

        public bool deleteFile(string dirPath, string fileName)
        {
            try
            {
                dirPath = DirectoryController.getIns().repairDirPath(dirPath);
                if (!isFileExists(dirPath + fileName))
                {
                    return false;
                }
                File.Delete(dirPath + fileName);
                return true;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController deleteFile");
                return false;
            }
        }

        public string readFile(string filePath)
        {
            try
            {
                if (!isFileExists(filePath))
                {
                    return "";
                }
                return Encoding.UTF8.GetString(File.ReadAllBytes(filePath));
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController readFile");
                return "";
            }
        }

        public byte[] readFileByByte(string filePath)
        {
            try
            {
                if (!isFileExists(filePath))
                {
                    return null;
                }
                return File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController readFileByByte");
                return null;
            }
        }

        public Stream readFileByStream(string filePath)
        {
            try
            {
                if (!isFileExists(filePath))
                {
                    return null;
                }
                MemoryStream ms = new MemoryStream(File.ReadAllBytes(filePath));
                return ms;
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController readFileByStream");
                return null;
            }
        }

        public string getExtension(string filePath)
        {
            try
            {
                return Path.GetExtension(filePath).ToLower();
            }
            catch (Exception ex)
            {
                LogController.writeErrorLog(ex, "FileController getExtension");
                return "";
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
                LogController.writeErrorLog(ex, "FileController getDirectoryName");
                return "";
            }
        }
    }
}
