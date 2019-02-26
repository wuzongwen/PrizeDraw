using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Common
{
    public class LogHelper
    {
        public static readonly log4net.ILog logHanlder4Error = log4net.LogManager.GetLogger("ErrorLog");
        public static readonly log4net.ILog logHanlder4Info = log4net.LogManager.GetLogger("InfoLog");

        public static void SetConfig()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static void SetConfig(FileInfo configFile)
        {
            log4net.Config.XmlConfigurator.Configure(configFile);
        }

        public static void ErrorLog(string error)
        {
            if (logHanlder4Error.IsErrorEnabled)
            {
                logHanlder4Error.Error(error);
            }
            else
            {
                SetConfig();
                logHanlder4Error.Error(error);
            }
        }

        public static void InfoLog(string error)
        {
            if (logHanlder4Info.IsInfoEnabled)
            {
                logHanlder4Info.Info(error);
            }
            else
            {
                SetConfig();
                logHanlder4Info.Info(error);
            }
        }
    }
}