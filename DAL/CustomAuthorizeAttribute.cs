using Common;
using Model.ToolModels;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DAL
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        //获取Cookies密钥
        public static readonly string CookieSecretKey = new PrizeDrawContext().SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey;

        /// <summary>
        /// 对应Action允许的角色
        /// </summary>
        private string[] AuthRoles { get; set; }

        /// <summary>
        /// 在请求授权时调用
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                if (httpContext == null)
                {
                    throw new ArgumentNullException("HttpContext");
                }
                if (AuthRoles == null || AuthRoles.Length == 0)
                {
                    return false;
                }

                #region 确定当前用户角色是否属于指定的角色
                //获取当前登陆用户所在角色
                var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("User"), CookieSecretKey);
                UserCookie user = JsonConvert.DeserializeObject<UserCookie>(Cookies);
                if (user != null)
                {
                    //验证用户是否被禁用
                    if (db.SysUsers.FirstOrDefault(u => u.UserName == user.UserName & u.IsEnable == 0 & u.DelState == 0) != null)
                    {
                        //验证是否属于对应角色
                        for (int i = 0; i < AuthRoles.Length; i++)
                        {
                            if (user.RoleName.Contains(AuthRoles[i]))
                            {
                                return true;
                            }
                        }
                    }
                }
                #endregion
                return false;
            }
        }

        /// <summary>
        /// 提供一个入口点用于自定义授权检查，通过为true
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            using (PrizeDrawContext db = new PrizeDrawContext()) {
                var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("User"), CookieSecretKey);
                string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                string actionName = filterContext.ActionDescriptor.ActionName;
                //获取数据库中action允许的角色
                string User = GetActionRoles(actionName, controllerName);
                if (!string.IsNullOrWhiteSpace(User))
                {
                    this.AuthRoles = User.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    this.AuthRoles = new string[] { };
                }
                base.OnAuthorization(filterContext);
            }  
        }

        /// <summary>
        /// AuthorizeCore返回false时执行
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("User"), CookieSecretKey);
                base.HandleUnauthorizedRequest(filterContext);
                if (Cookies.Length != 0)
                {
                    UserCookie user = JsonConvert.DeserializeObject<UserCookie>(Cookies);
                    //验证用户是否被禁用或删除
                    if (db.SysUsers.FirstOrDefault(u => u.IsEnable == 0 & u.DelState == 0 & u.UserName == user.UserName) != null)
                    {
                        if (filterContext != null)
                        {
                            filterContext.Result = new JsonResult
                            {
                                Data = new { code = "401", msg = "您没有权限进行该操作!" },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                        }
                    }
                    else
                    {
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            filterContext.Result = new JsonResult
                            {
                                Data = new { code = "402", msg = "登录超时,请重新登录!", url = "/Admin/Login" },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                        }
                        else
                        {
                            //跳转至登录超时页面
                            filterContext.Result = new RedirectResult("/Error/ErrorLoginTimeout");
                            //当前访问页面:filterContext.HttpContext.Request.Url
                            //filterContext.HttpContext.Response.Redirect("/Admin/Login");
                        }
                    }
                }
                else
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JsonResult

                        {
                            Data = new { code = "402", msg = "登录超时,请重新登录!", url = "/Admin/Login" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    else
                    {
                        //跳转至登录超时页面
                        filterContext.Result = new RedirectResult("/Error/ErrorLoginTimeout");
                        //当前访问页面:filterContext.HttpContext.Request.Url
                        //filterContext.HttpContext.Response.Redirect("/Admin/Login");
                    }
                }
            }
        }

        /// <summary>
        /// 根据当前Controller和Action名称获取对应节点内容
        /// </summary>
        /// <param name="action">Action名称</param>
        /// <param name="controller">Controller名称</param>
        /// <returns></returns>
        public string GetActionRoles(string action, string controller)
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                SysActionRole sysActionRole = db.SysActionRoles.FirstOrDefault(s => s.ControllerName == controller & s.ActionName == action);
                if (sysActionRole != null)
                {
                    if (sysActionRole.User != null)
                    {
                        return sysActionRole.User;
                    }
                }
                return "";
            }
        }
    }
}