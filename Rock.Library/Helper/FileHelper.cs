using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace Rock.Library.Helper
{
    public class FileHelper
    {
        #region 文件读写
        /// <summary>
        /// 读文件
        /// </summary>
        public static string ReadFile(string path, Encoding encoding)
        {
            var result = string.Empty;
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        result = reader.ReadToEnd();
                        reader.Close();
                        reader.Dispose();
                    }
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 写文件
        /// </summary>
        public static bool WriteFile(string filePath, string filename, Encoding encoding, string value)
        {
            var flag = false;
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            string path = filePath + filename;
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    using (StreamWriter writer = new StreamWriter(stream, encoding))
                    {
                        writer.Write(value);
                        writer.Close();
                        writer.Dispose();
                    }
                    stream.Close();
                    stream.Dispose();
                }
                flag = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return flag;
        }
        #endregion

        #region 文件解压缩
        /// <summary>
        /// 压缩文件
        /// </summary> 
        public static void ZipDirectory(string path)
        {
            string startPath = path;
            string zipPath = path + ".zip";
            ZipFile.CreateFromDirectory(startPath, zipPath);
            Directory.Delete(path, true);
        }
        /// <summary>
        /// 解压文件
        /// </summary> 
        public static void ExtractZip(string path)
        {
            string zipPath = path;
            string extractPath = path.Substring(0, path.Length - 4);
            ZipFile.ExtractToDirectory(zipPath, extractPath);
            File.Delete(path);
        }
        #endregion

        #region 创建文件夹
        /// <summary>
        /// 创建文件夹
        /// </summary> 
        public static void CreateDirectory(string directoryPath)
        {
            //如果目录不存在则创建该目录
            if (!IsExistDirectory(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
        #endregion

        #region 删除文件夹下的所有文件
        public static void DelectDirectory(string srcPath, List<string> exceptDirs = null, List<string> files = null)
        {
            try
            {
                if (!Directory.Exists(srcPath))
                {
                    Directory.CreateDirectory(srcPath);
                    return;
                }
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        if (exceptDirs != null && exceptDirs.Contains(i.Name)) continue;
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        if (files != null && files.Contains(i.Name.ToLower())) continue;
                        File.Delete(i.FullName);      //删除指定文件
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 复制文件夹
        public static void CopyDirectory(string srcPath, string destPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)
                    {
                        if (!Directory.Exists(destPath + "\\" + i.Name))
                        {
                            Directory.CreateDirectory(destPath + "\\" + i.Name);
                        }
                        CopyDirectory(i.FullName, destPath + "\\" + i.Name);
                    }
                    else
                    {
                        File.Copy(i.FullName, destPath + "\\" + i.Name, true);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                throw;
            }
        }
        #endregion

    }
}
