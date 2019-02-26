using System.Linq;
using System.Web.Mvc;
using Model;
using DAL;
using Common;
using DAL.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Configuration;
using Senparc.Weixin.Exceptions;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP;
using Senparc.Weixin;
using System.IO;
using Senparc.CO2NET.Utilities;
using Senparc.Weixin.MP.MvcExtension;
using DataWall.WeCaht;
using Senparc.Weixin.MP.Entities.Request;

namespace PrizeDraw.Controllers.Home
{
    public class HomeController : Controller
    {
        IsysLogRepository Lg = new SysLogRepository();
        public PrizeDrawContext db = new PrizeDrawContext();

        public static readonly string Token = Config.SenparcWeixinSetting.Token;//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = Config.SenparcWeixinSetting.EncodingAESKey;//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = Config.SenparcWeixinSetting.WeixinAppId;//与微信公众账号后台的AppId设置保持一致，区分大小写。
        private readonly string AppSecret = Config.SenparcWeixinSetting.WeixinAppSecret;//与微信公众账号后台的AppId设置保持一致，区分大小写。

        //获取Cookies密钥
        public static readonly string CookieSecretKey = new PrizeDrawContext().SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey;

        /// <summary>
        /// 前台首页
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult Index(string PrizeDrawCode)
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                string Code = SecurityHelper.DecryptDES(PrizeDrawCode, CookieSecretKey);
                SysLibrary sysLibrary = db.SysLibrarys.FirstOrDefault(l => l.LibraryCode == Code);
                if (sysLibrary != null)
                {
                    if (sysLibrary.DelState == 1)
                    {
                        HttpContext.Response.Write("活动已删除");
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    if (sysLibrary.IsEnable == 1)
                    {
                        HttpContext.Response.Write("活动已禁用");
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    ViewBag.PrizeDrawCode = PrizeDrawCode;
                    ViewBag.Title = sysLibrary.LibraryName;
                    CookieHelper.SetCookie("Library", SecurityHelper.EncryptDES(JsonConvert.SerializeObject(sysLibrary), CookieSecretKey), DateTime.Now.AddYears(1));
                }
                else
                {
                    HttpContext.Response.Write("活动ID无效");
                    return new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                return View();
            }
        }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        public ActionResult Content(int Type)
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("Library"), CookieSecretKey);
                if (Cookies != null & Cookies != "")
                {
                    SysLibrary Library = JsonConvert.DeserializeObject<SysLibrary>(Cookies);
                    var Content = db.SysContents.Include("SysFiles").AsNoTracking().Where(c => c.SysLibraryId == Library.ID & c.Type == Type & c.IsEnable == 0 & c.DelState == 0).Select(c => new { c.ID, c.Title, c.ContentDescribe, c.Type, c.Sort, c.EditTime, c.SysFiles }).OrderBy(c => c.Sort).ToList();
                    var data = JsonConvert.SerializeObject(Content.Select(c => new { c.ID, c.Title, c.ContentDescribe, c.Type, c.Sort, c.EditTime }).FirstOrDefault());
                    var files = JsonConvert.SerializeObject(Content.FirstOrDefault().SysFiles.Where(f => f.DelState == 0 && f.FilePath != null));
                    return Json(new
                    {
                        code = 201,
                        data = data,
                        files = files
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        code = 202,
                        msg = "验证失败"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// 参加活动
        /// </summary>
        /// <returns></returns>
        public ActionResult Join()
        {
            var state = "PrizeDraw-" + SystemTime.Now.Millisecond;//随机数，用于识别请求可靠性
            CookieHelper.SetCookie("State", state, DateTime.Now.AddYears(10));//储存随机数到Cookie
            //Session["State"] = state;//储存随机数到Session

            //此页面引导用户点击授权
            ViewData["UrlUserInfo"] = OAuthApi.GetAuthorizeUrl(AppId, "http://prizedraw.tuji365.cn/Home/UserInfoCallback", state, OAuthScope.snsapi_userinfo);
            ViewData["UrlBase"] = OAuthApi.GetAuthorizeUrl(AppId, "http://prizedraw.tuji365.cn/Home/BaseCallback", state, OAuthScope.snsapi_base);

            return View();
        }

        /// <summary>
        /// OAuthScope.snsapi_userinfo方式回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="returnUrl">用户最初尝试进入的页面</param>
        /// <returns></returns>
        public ActionResult UserInfoCallback(string code, string state, string returnUrl)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }

            if (state != CookieHelper.GetCookieValue("State"))
            {
                //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下，
                //建议用完之后就清空，将其一次性使用
                //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
                return Content("验证失败！请从正规途径进入！");
            }

            OAuthAccessTokenResult result = null;

            //通过，用code换取access_token
            try
            {
                result = OAuthApi.GetAccessToken(AppId, AppSecret, code);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            if (result.errcode != ReturnCode.请求成功)
            {
                //RedirectToRoute(new { controller = "Home", action = "Join" });
                return Redirect("Join");
                //return Content("错误：" + result.errmsg);
            }
            //下面2个数据也可以自己封装成一个类，储存在数据库中（建议结合缓存）
            //如果可以确保安全，可以将access_token存入用户的cookie中，每一个人的access_token是不一样的
            CookieHelper.SetCookie("OAuthAccessTokenStartTime", DateTime.Now.ToString(), DateTime.Now.AddMinutes(115));//储存随机数到Cookie
            CookieHelper.SetCookie("OAuthAccessToken", result.ToJson(), DateTime.Now.AddMinutes(115));//储存随机数到Cookie

            //因为第一步选择的是OAuthScope.snsapi_userinfo，这里可以进一步获取用户详细信息
            try
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
                return View(userInfo);
            }
            catch (ErrorJsonResultException ex)
            {
                return Content(ex.Message);
            }
        }

        /// <summary>
        /// OAuthScope.snsapi_base方式回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="returnUrl">用户最初尝试进入的页面</param>
        /// <returns></returns>
        public ActionResult BaseCallback(string code, string state, string returnUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return Content("您拒绝了授权！");
                }

                if (state != CookieHelper.GetCookieValue("State"))
                {
                    //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下，
                    //建议用完之后就清空，将其一次性使用
                    //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
                    return Content("验证失败！请从正规途径进入！");
                }

                //通过，用code换取access_token
                var result = OAuthApi.GetAccessToken(AppId, AppSecret, code);
                if (result.errcode != ReturnCode.请求成功)
                {
                    return Content("错误：" + result.errmsg);
                }

                //下面2个数据也可以自己封装成一个类，储存在数据库中（建议结合缓存）
                //如果可以确保安全，可以将access_token存入用户的cookie中，每一个人的access_token是不一样的
                CookieHelper.SetCookie("OAuthAccessTokenStartTime", DateTime.Now.ToString(), DateTime.Now.AddMinutes(115));//储存随机数到Cookie
                CookieHelper.SetCookie("OAuthAccessToken", result.ToJson(), DateTime.Now.AddMinutes(115));//储存随机数到Cookie

                //因为这里还不确定用户是否关注本微信，所以只能试探性地获取一下
                OAuthUserInfo userInfo = null;
                try
                {
                    //已关注，可以得到详细信息
                    userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }


                    ViewData["ByBase"] = true;
                    return View("UserInfoCallback", userInfo);
                }
                catch (ErrorJsonResultException ex)
                {
                    //未关注，只能授权，无法得到详细信息
                    //这里的 ex.JsonResult 可能为："{\"errcode\":40003,\"errmsg\":\"invalid openid\"}"
                    return Content("用户已授权，授权Token：" + result);
                }
            }
            catch (Exception ex)
            {
                WeixinTrace.SendCustomLog("BaseCallback 发生错误", ex.ToString());
                return Content("发生错误：" + ex.ToString());
            }
        }

        /// <summary>
        /// 测试ReturnUrl
        /// </summary>
        /// <returns></returns>
        public ActionResult TestReturnUrl()
        {
            string msg = "OAuthAccessTokenStartTime：" + CookieHelper.GetCookieValue("OAuthAccessTokenStartTime");
            //注意：OAuthAccessTokenStartTime这里只是为了方便识别和演示，
            //OAuthAccessToken千万千万不能传输到客户端！

            msg += "<br /><br />" +
                   "此页面为returnUrl功能测试页面，可以进行刷新（或后退），不会得到code不可用的错误。<br />测试不带returnUrl效果，请" +
                   string.Format("<a href=\"{0}\">点击这里</a>。", Url.Action("Index"));

            return Content(msg, "text/html");
        }

        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateQrCode(string PrizeDrawCode)
        {
            string str = "http://weixin.qq.com/r/GkhFXXTE6ymUrTA_9x0I?PrizeDrawCode=" + PrizeDrawCode;
            using (var memoryStream = QRCodeHelper.GetQRCode(str))
            {
                Response.ContentType = "image/jpeg";
                Response.OutputStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                Response.End();
            }
            return null;
        }

        /// <summary>
        /// 获取参与者头像和昵称
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetUsers(string PrizeDrawCode)
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                string Code = SecurityHelper.DecryptDES(PrizeDrawCode, CookieSecretKey);
                SysLibrary sysLibrary = db.SysLibrarys.FirstOrDefault(l => l.LibraryCode == Code);
                var idList = db.SysPrizeDrawRecords.Where(s => s.SysLibraryId == sysLibrary.ID).ToList();
                string[] phone = new string[idList.Count];
                string[] xinm = new string[idList.Count];
                string[] openid = new string[idList.Count];
                for (int i = 0; i < idList.Count; i++)
                {
                    int UserID = idList[i].SysPrizeDrawUserId;
                    var User = db.SysPrizeDrawUsers.AsNoTracking().FirstOrDefault(s => s.ID == UserID);
                    phone[i] = User.nickname;
                    xinm[i] = User.headimgurl;
                    openid[i] = User.openid;
                }
                return Json(new { code = "200", data = new { phone, xinm, openid } });
            }
        }
    }
}