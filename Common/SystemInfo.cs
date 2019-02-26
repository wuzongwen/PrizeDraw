using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Common
{
    public static class SystemInfo
    {
        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        public static string GetSystemInfo()
        {
            string serverVersion = ServerVersion();
            string iISVersion = IISVersion();
            string browserInfo = GetBrowser();
            string absolutePath = AbsolutePath();
            string serverLanguage = ServerLanguage();
            string cpuNumber = CpuNumber();
            string dotNetVersion = DotNetVersion();
            string hardDiskFreeSpace = HardDiskFreeSpace();
            return JsonConvert.SerializeObject(new
            {
                ServerVersion = serverVersion,
                IISVersion = iISVersion,
                BrowserInfo = browserInfo,
                AbsolutePath = absolutePath,
                ServerLanguage = serverLanguage,
                CpuNumber = cpuNumber,
                DotNetVersion = dotNetVersion,
                HardDiskFreeSpace = hardDiskFreeSpace
            });
        }

        /// <summary>
        /// 浏览器信息
        /// </summary>
        /// <returns></returns>
        public static string GetBrowser()
        {
            string browsers;
            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
            string aa = bc.Browser.ToString();
            string bb = bc.Version.ToString();
            string cc = bc.Platform.ToString();
            browsers = cc + aa + bb;
            return browsers;
        }

        /// <summary>
        /// 服务器操作系统版本
        /// </summary>
        /// <returns></returns>
        public static string ServerVersion() {
            return Environment.OSVersion.ToString();
        }

        /// <summary>
        /// 服务器IIS版本
        /// </summary>
        /// <returns></returns>
        public static string IISVersion()
        {
            return HttpContext.Current.Request.ServerVariables["SERVER_SOFTWARE"];
        }

        /// <summary>
        /// 虚拟目录的绝对路径
        /// </summary>
        /// <returns></returns>
        public static string AbsolutePath()
        {
            string dataDir = AppDomain.CurrentDomain.BaseDirectory;//获得当前服务器程序的运行目录  
            if (dataDir.EndsWith(@"\bin\Debug\") || dataDir.EndsWith(@"\bin\Release\"))
            {
                dataDir = System.IO.Directory.GetParent(dataDir).Parent.Parent.FullName;  //获取网站根目录(上一级目录的上一级目录)
            }
            return dataDir;
        }

        /// <summary>
        /// 服务器区域语言
        /// </summary>
        /// <returns></returns>
        public static string ServerLanguage()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"];
        }

        /// <summary>
        /// CPU数量
        /// </summary>
        /// <returns></returns>
        public static string CpuNumber()
        {
            return Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS");
        }

        /// <summary>
        /// .NET版本信息
        /// </summary>
        /// <returns></returns>
        public static string DotNetVersion()
        {
            return ".NET解释引擎版本：" + ".NET CLR" + Environment.Version.Major + "." + Environment.Version.Minor + "." + Environment.Version.Build + "." + Environment.Version.Revision;//.NET解释引擎版本  
        }

        /// <summary>
        /// 获取所在驱动器的剩余空间总大小(单位为GB) 
        /// </summary>
        /// <returns></returns>
        public static string HardDiskFreeSpace()
        {
            long freeSpace = new long();
            string FreeSpace = "";
            string str_HardDiskName = AbsolutePath().Substring(0, 1) + ":\\";
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == str_HardDiskName)
                {
                    freeSpace = drive.TotalFreeSpace / (1024 * 1024 * 1024);
                }
            }
            FreeSpace = freeSpace.ToString();

            return FreeSpace + "GB";
        }
    }
}
