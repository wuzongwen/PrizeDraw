using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrizeDraw.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        // GET: Error
        public ActionResult Error404()
        {
            ViewBag.Title = "很抱歉！当前页面找不到了";
            return View();
        }

        // GET: ErrorLoginTimeout
        public ActionResult ErrorLoginTimeout()
        {
            ViewBag.Title = "登录超时";
            return View();
        }
    }
}