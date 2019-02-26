using Common;
using DAL;
using PrizeDraw.SignalR;
using Model;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PrizeDraw.Controllers.WebApi
{
    public class CustCountController : Controller
    {
        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="value"></param>
        [Route("Api/Test/")]
        public void Test()
        {
            try
            {
                HttpContext.Response.Write("测试");
            }
            catch (Exception)
            {
                HttpContext.Response.Write("测试");
            }
        }

    }
}