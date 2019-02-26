using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrizeDraw.App_Start
{
    /// <summary>
    /// 后台路由
    /// </summary>
    public class admin_routing : RazorViewEngine
    {
        public admin_routing()
        {
            //视图位置
            ViewLocationFormats = new[]
            {
                "~/Views/Admin/{1}/{0}.cshtml",
                //内容管理
                "~/Views/Admin/{1}/NewBook/{0}.cshtml",
                "~/Views/Admin/{1}/Notice/{0}.cshtml",
                "~/Views/Admin/{1}/Slide/{0}.cshtml",
                "~/Views/Admin/{1}/Video/{0}.cshtml",
               
                //其他
                 "~/Views/Admin/{1}/AdditionalData/{0}.cshtml",
                "~/Views/Admin/{1}/CustDevice/{0}.cshtml",

                //模块管理
                 "~/Views/Admin/{1}/Modules/{0}.cshtml",

                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml"
            };
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return base.FindView(controllerContext, viewName, masterName, useCache);
        }
    }
}