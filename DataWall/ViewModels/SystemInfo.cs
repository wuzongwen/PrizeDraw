using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrizeDraw.ViewModels
{
    public class SystemInfo
    {
        /// <summary>
        /// 服务器操作系统版本
        /// </summary>
        public string ServerVersion { get; set; }

        /// <summary>
        /// 服务器IIS版本
        /// </summary>
        public string IISVersion { get; set; }

        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string BrowserInfo { get; set; }

        /// <summary>
        /// 虚拟目录的绝对路径
        /// </summary>
        public string AbsolutePath { get; set; }

        /// <summary>
        /// 服务器区域语言
        /// </summary>
        public string ServerLanguage { get; set; }

        /// <summary>
        /// CPU数量
        /// </summary>
        public string CpuNumber { get; set; }

        /// <summary>
        /// .NET版本信息
        /// </summary>
        public string DotNetVersion { get; set; }

        /// <summary>
        /// 获取所在驱动器的剩余空间总大小(单位为GB) 
        /// </summary>
        public string HardDiskFreeSpace { get; set; }
    }
}