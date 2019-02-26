using System;
using System.Linq;
using System.Web.Mvc;
using DAL;
using DAL.Repositories;
using Common;
using Model;
using Model.ToolModels;
using PrizeDraw.ViewModels;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PrizeDraw.Controllers
{
    public class AdminController : Controller
    {
        IsysLogRepository Lg = new SysLogRepository();

        //获取Cookies密钥
        public static readonly string CookieSecretKey = new PrizeDrawContext().SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey;

        /// <summary>
        /// 后台首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("User"), CookieSecretKey);
                if (Cookies != "")
                {
                    UserCookie user = JsonConvert.DeserializeObject<UserCookie>(Cookies);
                    ViewBag.UserName = user.UserName;
                    ViewBag.UserRole = user.RoleId;
                }
                else
                {
                    return RedirectToAction("Login");
                }
                SysProgramInfo sysProgramInfo = db.SysProgramInfos.AsNoTracking().FirstOrDefault();
                ViewBag.Title = "数据墙后台管理系统" + sysProgramInfo.ProgramVersion;

                //获取菜单
                List<SysMenu> menuList = db.SysMenus.Where(m => m.IsEnable == 0 & m.DelState == 0).AsNoTracking().ToList();
                ViewData["datalist"] = menuList;

                return View(sysProgramInfo);
            }
        }

        /// <summary>
        /// 欢迎页
        /// </summary>
        /// <returns></returns>
        public ActionResult Welcome()
        {
            try
            {
                using (PrizeDrawContext db = new PrizeDrawContext())
                {
                    List<SysLog> logList = db.SysLogs.Where(s => s.Modular != "User").OrderByDescending(u => u.CrateTime).Take(5).AsNoTracking().ToList();
                    ViewData["datalist"] = logList;
                    TimeSpan Time = DateTime.Now - db.SysProgramInfos.FirstOrDefault().FirstRunTime;
                    ViewBag.Time = Math.Round(Time.TotalHours, 0);

                    //获取软件信息
                    SysProgramInfo sysProgramInfo = db.SysProgramInfos.AsNoTracking().FirstOrDefault();
                    ViewBag.ProgramVersion = sysProgramInfo.ProgramVersion;

                    //获取服务器信息
                    ViewModels.SystemInfo systemInfo = JsonConvert.DeserializeObject<ViewModels.SystemInfo>(Common.SystemInfo.GetSystemInfo());
                    return View(systemInfo);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("欢迎页加载失败:" + ex.Message);
                return Json(new { error = "页面加载失败，请联系管理员" });
            }

        }

        /// <summary>
        /// 登陆页
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                SysProgramInfo sysProgramInfo = db.SysProgramInfos.AsNoTracking().FirstOrDefault();
                var Cookies = CookieHelper.GetCookieValue("User");
                if (Cookies != "")
                {
                    return RedirectToAction("Index");
                }
                return View(sysProgramInfo);
            }
        }

        #region 用户

        /// <summary>
        /// 验证登陆
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoginValidate(string UserName, string Password)
        {
            try
            {
                using (PrizeDrawContext db = new PrizeDrawContext())
                {
                    string PasswordToMd5 = MD5Helper.MD5Encrypt32(Password);
                    SysUser user = db.SysUsers.FirstOrDefault(u => (u.UserName == UserName || u.Email == UserName) & u.Password == PasswordToMd5);
                    if (user != null)
                    {
                        if (user.DelState == 0)
                        {
                            if (user.IsEnable == 0)
                            {
                                string RoleName = "";
                                int RoleId = db.SysUserRoles.Where(u => u.SysUserID == user.ID).AsNoTracking().FirstOrDefault().SysRoleId;
                                SysRole Role = db.SysRoles.Where(su => su.ID == RoleId).AsNoTracking().FirstOrDefault();
                                RoleName = Role.RoleName;

                                CookieHelper.SetCookie("User", SecurityHelper.EncryptDES(JsonConvert.SerializeObject(new { UserId = user.ID, UserName = user.UserName, RoleName = RoleName }), CookieSecretKey), DateTime.Now.AddMinutes(30));

                                Lg.AddLog("用户登陆", "User", 0);

                                return Json(new { code = "200", data = new { token = new { UserId = user.ID, UserName = user.UserName, RoleName = RoleName, RoleId = RoleId } }, msg = "success" });
                            }
                            else
                            {
                                return Json(new { code = "202", data = new { }, msg = "该用户已禁用" });
                            }
                        }
                        else
                        {
                            return Json(new { code = "202", data = new { }, msg = "该用户已删除" });
                        }
                    }
                    else
                    {
                        return Json(new { code = "201", data = new { }, msg = "用户名或密码不正确" });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("用户登录错误:" + ex.Message);
                return Json(new { code = "202", data = new { }, msg = ex.Message });
            }
        }

        /// <summary>
        /// 注销登陆
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginOut()
        {
            CookieHelper.RemoveCookie("User");
            return RedirectToAction("Login");
        }
        #endregion 
    }
}