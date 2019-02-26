using DAL;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using Model;
using Common;
using System.Data.Entity;

namespace PrizeDraw.Controllers.User
{
    [CustomAuthorize]//权限验证
    public class UserController : Controller
    {
        ISysUserRepository Us = new SysUserRepository();
        IsysLogRepository Lg = new SysLogRepository();

        /// <summary>
        /// 用户列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult List(int? page)
        {
            PrizeDrawContext db = new PrizeDrawContext();
            var Users = db.SysUsers.Where(u => u.DelState == 0).AsNoTracking().ToList();
            ViewBag.DataCout = Users.Count();

            //第几页
            int pageNumber = page ?? 1;

            //每页显示多少条
            int pageSize = 10;

            //根据ID排序
            Users = Users.OrderBy(u => u.ID).ToList();

            //通过ToPagedList扩展方法进行分页
            IPagedList<SysUser> pagedList = Users.ToPagedList(pageNumber, pageSize);

            //将分页处理后的列表传给View
            return View(pagedList);
        }

        /// <summary>
        /// 添加用户页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UserAdd()
        {
            return View();
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="User">用户对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserAdd(FormCollection User)
        {
            try
            {
                if (Check("UserAdd", 0, User["UserName"]))
                {
                    using (PrizeDrawContext db = new PrizeDrawContext())
                    {
                        //用户
                        SysUser sysUser = new SysUser();
                        sysUser.UserName = User["UserName"];
                        sysUser.Email = User["Email"];
                        sysUser.Password = MD5Helper.MD5Encrypt32(User["Password"]);
                        sysUser.IsEnable = 0;
                        sysUser.CrateTime = DateTime.Now;
                        sysUser.EditTime = DateTime.Now;
                        sysUser.DelState = 0;
                        db.SysUsers.Add(sysUser);

                        //权限
                        var sysUserRoles = new List<SysUserRole> {
                         new SysUserRole{ SysUserID=sysUser.ID,SysRoleId=2}
                        };
                        sysUserRoles.ForEach(s => db.SysUserRoles.Add(s));

                        //添加
                        db.SaveChanges();

                        Lg.AddLog("添加用户", "User", 1);

                        return Json(new { code = "200", msg = "添加成功!" });
                    }
                }
                else
                {
                    return Json(new { code = "202", msg = "用户名已存在!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("用户添加失败:" + ex.Message);
                return Json(new { code = "201", msg = "添加失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 修改用户页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UserEdit(int id)
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                SysUser sysUser = db.SysUsers.Find(id);
                return View(sysUser);
            }
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="User">用户对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserEdit(FormCollection User)
        {
            try
            {
                if (Check("UserEdit", int.Parse(User["ID"]), User["UserName"]))
                {
                    using (PrizeDrawContext db = new PrizeDrawContext())
                    {
                        SysUser sysUser = new SysUser()
                        {
                            ID = int.Parse(User["ID"]),
                            UserName = User["UserName"],
                            Email = User["Email"],
                            EditTime = DateTime.Now
                        };

                        db.Entry(sysUser).State = EntityState.Modified;

                        //修改密码
                        if (!string.IsNullOrEmpty(User["Password"]))
                        {
                            sysUser.Password = MD5Helper.MD5Encrypt32(User["Password"]);
                        }
                        //不修改密码
                        else
                        {
                            //不更新的字段
                            db.Entry(sysUser).Property(x => x.Password).IsModified = false;
                        }
                        //不更新的字段
                        db.Entry(sysUser).Property(x => x.CrateTime).IsModified = false;
                        db.Entry(sysUser).Property(x => x.IsEnable).IsModified = false;
                        db.Entry(sysUser).Property(x => x.DelState).IsModified = false;
                        db.SaveChanges();

                        Lg.AddLog("修改用户", "User", 2);

                        return Json(new { code = "200", msg = "修改成功!" });
                    }
                }
                else
                {
                    return Json(new { code = "202", msg = "用户名已存在!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("用户修改失败:" + ex.Message);
                return Json(new { code = "201", msg = "修改失败，请重试或联系管理员!" });
            }
        }

        /// <summary>
        /// 修改密码页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PwdEdit(string UserName)
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                SysUser sysUser = db.SysUsers.AsNoTracking().FirstOrDefault(u => u.UserName == UserName);
                return View(sysUser);
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="User">用户对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PwdEdit(FormCollection User)
        {
            try
            {
                using (PrizeDrawContext db = new PrizeDrawContext())
                {
                    SysUser sysUser = new SysUser()
                    {
                        ID = int.Parse(User["ID"]),
                        UserName = User["UserName"],
                        EditTime = DateTime.Now
                    };

                    db.Entry(sysUser).State = EntityState.Modified;
                    //修改密码
                    if (!string.IsNullOrEmpty(User["Password"]))
                    {
                        sysUser.Password = MD5Helper.MD5Encrypt32(User["Password"]);
                    }
                    //不修改密码
                    else
                    {
                        //不更新的字段
                        db.Entry(sysUser).Property(x => x.Password).IsModified = false;
                    }
                    //不更新的字段
                    db.Entry(sysUser).Property(x => x.Email).IsModified = false;
                    db.Entry(sysUser).Property(x => x.CrateTime).IsModified = false;
                    db.Entry(sysUser).Property(x => x.IsEnable).IsModified = false;
                    db.Entry(sysUser).Property(x => x.DelState).IsModified = false;
                    db.SaveChanges();

                    Lg.AddLog("修改用户密码", "User", 2);

                    return Json(new { code = "200", msg = "修改成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("用户修改失败:" + ex.Message);
                return Json(new { code = "201", msg = "修改失败，请重试或联系管理员!" });
            }
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditEnable(int id, int enable)
        {
            try
            {
                using (PrizeDrawContext db = new PrizeDrawContext())
                {
                    string RoleName = "";
                    foreach (var item in db.SysUsers.AsNoTracking().FirstOrDefault(u => u.ID == id).SysUserRoles)
                    {
                        RoleName = item.SysRole.RoleName;
                    }
                    if (RoleName != "Administrator")
                    {
                        SysUser sysUser = new SysUser()
                        {
                            ID = id,
                            IsEnable = enable,
                            EditTime = DateTime.Now
                        };

                        db.Entry(sysUser).State = EntityState.Modified;
                        //不更新的字段
                        db.Entry(sysUser).Property(x => x.UserName).IsModified = false;
                        db.Entry(sysUser).Property(x => x.Email).IsModified = false;
                        db.Entry(sysUser).Property(x => x.Password).IsModified = false;
                        db.Entry(sysUser).Property(x => x.CrateTime).IsModified = false;
                        db.Entry(sysUser).Property(x => x.DelState).IsModified = false;
                        db.SaveChanges();

                        Lg.AddLog("修改用户状态", "User", 2);
                        return Json(new { code = "200", msg = "修改成功!" });
                    }
                    else
                    {
                        LogHelper.ErrorLog("管理员状态修改失败");
                        return Json(new { code = "201", msg = "超级管理员用户不可进行该操作!" });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("用户状态修改失败:" + ex.Message);
                return Json(new { code = "201", msg = "修改失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="page">当前页码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelUser(int id, int page)
        {
            try
            {
                using (PrizeDrawContext db = new PrizeDrawContext())
                {
                    string RoleName = "";
                    foreach (var item in db.SysUsers.AsNoTracking().FirstOrDefault(u => u.ID == id).SysUserRoles)
                    {
                        RoleName = item.SysRole.RoleName;
                    }
                    if (RoleName != "Administrator")
                    {
                        SysUser sysUser = new SysUser()
                        {
                            ID = id,
                            DelState = 1,
                            EditTime = DateTime.Now
                        };

                        db.Entry(sysUser).State = EntityState.Modified;
                        //不更新的字段
                        db.Entry(sysUser).Property(x => x.UserName).IsModified = false;
                        db.Entry(sysUser).Property(x => x.Email).IsModified = false;
                        db.Entry(sysUser).Property(x => x.Password).IsModified = false;
                        db.Entry(sysUser).Property(x => x.CrateTime).IsModified = false;
                        db.Entry(sysUser).Property(x => x.IsEnable).IsModified = false;
                        db.SaveChanges();
                        int npage = 0;
                        int Count = db.SysUsers.Where(u => u.DelState == 0).AsNoTracking().Count();
                        double MaxPage = Convert.ToDouble(Convert.ToDouble(Count + 10) / Convert.ToDouble(10));
                        if (MaxPage > page)
                        {
                            npage = page;
                        }
                        else
                        {
                            if (Count <= 10)
                            {
                                npage = 1;
                            }
                            else
                            {
                                npage = page - 1;
                            }
                        }

                        Lg.AddLog("删除用户", "User", 3);

                        return Json(new { code = "200", page = npage, msg = "删除成功!" });
                    }
                    else
                    {
                        LogHelper.ErrorLog("管理员删除失败");
                        return Json(new { code = "201", msg = "超级管理员用户不可进行该操作!" });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("用户删除失败:" + ex.Message);
                return Json(new { code = "201", msg = "删除失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 批量删除用户
        /// </summary>
        /// <param name="idList">用户id集合</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelUserAll(string idList, int page)
        {
            try
            {
                using (PrizeDrawContext db = new PrizeDrawContext())
                {
                    //获取待删除用户id集
                    string[] sArray = idList.Split(',');
                    int[] IdList = new int[sArray.Length];
                    for (int i = 0; i < sArray.Length; i++)
                    {
                        IdList[i] = Int32.Parse(sArray[i]);
                    }
                    for (int i = 0; i < IdList.Length; i++)
                    {
                        int id = IdList[i];
                        
                        string RoleName = "";
                        foreach (var item in db.SysUsers.AsNoTracking().FirstOrDefault(u => u.ID == id).SysUserRoles)
                        {
                            RoleName = item.SysRole.RoleName;
                        }
                        if (RoleName != "Administrator")
                        {
                            SysUser sysUser = new SysUser()
                            {
                                ID = id,
                                DelState = 1,
                                EditTime = DateTime.Now
                            };

                            db.Entry(sysUser).State = EntityState.Modified;
                            //不更新的字段
                            db.Entry(sysUser).Property(x => x.ID).IsModified = false;
                            db.Entry(sysUser).Property(x => x.UserName).IsModified = false;
                            db.Entry(sysUser).Property(x => x.Email).IsModified = false;
                            db.Entry(sysUser).Property(x => x.Password).IsModified = false;
                            db.Entry(sysUser).Property(x => x.CrateTime).IsModified = false;
                            db.Entry(sysUser).Property(x => x.IsEnable).IsModified = false;
                            db.SaveChanges();
                        }
                    }
                    int npage = 0;
                    int Count = db.SysUsers.Where(u => u.DelState == 0).AsNoTracking().Count();
                    double MaxPage = Convert.ToDouble(Convert.ToDouble(Count + 10) / Convert.ToDouble(10));
                    if (MaxPage > page)
                    {
                        npage = page;
                    }
                    else
                    {
                        if (Count <= 10)
                        {
                            npage = 1;
                        }
                        else
                        {
                            if ((Count % 10) <= page)
                            {
                                npage = page - 1;
                            }
                        }
                    }

                    Lg.AddLog("删除用户", "User", 3);

                    return Json(new { code = "200", page = npage, msg = "删除成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("用户删除失败:" + ex.Message);
                return Json(new { code = "201", msg = "删除失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 检查用户名是否存在
        /// </summary>
        /// <param name="Action">方法</param>
        /// <param name="Id">新增用户时为0</param>
        /// <param name="UserName">用户名</param>
        /// <returns></returns>
        public bool Check(string Action, int Id, string UserName)
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                if (Action == "UserEdit")
                {
                    if (db.SysUsers.Where(u => u.UserName == DbFunctions.AsNonUnicode(UserName)).Count() >= 1)
                    {
                        //AsNoTracking将Hold住的对象释放掉
                        SysUser User = db.SysUsers.AsNoTracking().FirstOrDefault(u => u.ID == Id);
                        if (User.UserName == UserName)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (db.SysUsers.Where(u => u.UserName == DbFunctions.AsNonUnicode(UserName)).AsNoTracking().Count() > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
    }
}