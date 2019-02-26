using Common;
using DAL;
using Model;
using Model.ToolModels;
using DAL.Repositories;
using PrizeDraw.ViewModels;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using System.Web.Helpers;
using PrizeDraw.SignalR;

namespace PrizeDraw.Controllers.Admin
{
    [CustomAuthorize]//权限验证
    public class ContentController : Controller
    {
        IsysLogRepository Lg = new SysLogRepository();

        /// <summary>
        /// 内容列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ContentList(SearchInfo searchInfo, int Type, int? page)
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                //当前用户可管理抽奖活动
                List<SysLibrary> libList = GetLibraryList();
                ViewData["datalist"] = libList;

                //判断菜单内容
                SysMenu Menu = db.SysMenus.Where(m => m.ContentType == Type).AsNoTracking().FirstOrDefault();
                int MenuType = Type;
                ViewBag.MenuType = Type;
                ViewBag.Tip = Menu.MenuName;
                Type = Menu.Type;

                switch (Type)
                {
                    case 0:
                        ViewBag.addAction = "SlideAdd";
                        ViewBag.editAction = "SlideEdit";
                        break;
                }

                //内容类型
                ViewBag.Type = Type;

                int[] libids = new int[libList.Count];
                for (int i = 0; i < libList.Count; i++)
                {
                    libids[i] = libList[i].ID;
                }

                //全部内容
                List<SysContent> list = db.SysContents.Where(u => u.DelState == 0 & u.Type == MenuType & libids.Contains(u.SysLibraryId)).AsNoTracking().ToList();

                //第几页
                int pageNumber = page ?? 1;

                //条件查询
                if (searchInfo != null)
                {
                    int libraryIds = searchInfo.library;
                    string keywords = searchInfo.keyword;
                    if (keywords == null)
                    {
                        keywords = "";
                    }
                    if (libraryIds != 0)
                    {
                        list = db.SysContents.Where(u => u.DelState == 0 & u.Type == MenuType & u.SysLibraryId == libraryIds & u.Title.Contains(keywords)).AsNoTracking().ToList();
                    }
                    else
                    {
                        list = db.SysContents.Where(u => u.DelState == 0 & u.Type == MenuType & libids.Contains(u.SysLibraryId) & u.Title.Contains(keywords)).AsNoTracking().ToList();
                    }
                }
                    
                //每页显示的数据数量
                int pageSize = 10;

                ViewBag.DataCout = list.Count();
                ViewBag.library = searchInfo.library;
                ViewBag.keyword = searchInfo.keyword;

                //通过ToPagedList扩展方法进行分页
                IPagedList<SysContent> pagedList = list.OrderBy(c => c.Sort).ToPagedList(pageNumber, pageSize);

                return View(pagedList);
            }
        }

        #region 内容操作
        /// <summary>
        /// 删除内容
        /// </summary>
        /// <param name="id">内容id</param>
        /// <param name="page">当前页码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelContent(int id, int page)
        {
            try
            {
                using (PrizeDrawContext db = new PrizeDrawContext())
                {

                    //日志内容
                    string LogTxt = db.SysMenus.Where(m => m.ContentType == db.SysContents.Where(c => c.ID == id).FirstOrDefault().Type).AsNoTracking().FirstOrDefault().MenuName;

                    SysContent sysContent = new SysContent()
                    {
                        ID = id,
                        DelState = 1,
                        EditTime = DateTime.Now
                    };

                    db.Entry(sysContent).State = EntityState.Modified;
                    //不更新的字段
                    db.Entry(sysContent).Property(x => x.Title).IsModified = false;
                    db.Entry(sysContent).Property(x => x.ContentDescribe).IsModified = false;
                    db.Entry(sysContent).Property(x => x.Type).IsModified = false;
                    db.Entry(sysContent).Property(x => x.SysLibraryId).IsModified = false;
                    db.Entry(sysContent).Property(x => x.Sort).IsModified = false;
                    db.Entry(sysContent).Property(x => x.CrateTime).IsModified = false;
                    db.Entry(sysContent).Property(x => x.IsEnable).IsModified = false;
                    db.SaveChanges();
                    int npage = 0;
                    int Count = db.SysContents.Where(u => u.DelState == 0).AsNoTracking().Count();
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

                    Lg.AddLog("删除" + LogTxt, "Content", 3);

                    //推送更新
                    PushUpdate(id);

                    return Json(new { code = "200", page = npage, msg = "删除成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("内容删除失败:" + ex.Message);
                return Json(new { code = "201", msg = "删除失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id">内容id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditEnable(int id, int enable)
        {
            try
            {
                using (PrizeDrawContext db = new PrizeDrawContext())
                {
                    //日志内容
                    string LogTxt = db.SysMenus.Where(m => m.ContentType == db.SysContents.Where(c => c.ID == id).FirstOrDefault().Type).AsNoTracking().FirstOrDefault().MenuName;

                    SysContent sysContent = new SysContent()
                    {
                        ID = id,
                        IsEnable = enable,
                        EditTime = DateTime.Now,
                    };

                    db.Entry(sysContent).State = EntityState.Modified;
                    //不更新的字段
                    db.Entry(sysContent).Property(x => x.Title).IsModified = false;
                    db.Entry(sysContent).Property(x => x.ContentDescribe).IsModified = false;
                    db.Entry(sysContent).Property(x => x.Type).IsModified = false;
                    db.Entry(sysContent).Property(x => x.SysLibraryId).IsModified = false;
                    db.Entry(sysContent).Property(x => x.Sort).IsModified = false;
                    db.Entry(sysContent).Property(x => x.CrateTime).IsModified = false;
                    db.Entry(sysContent).Property(x => x.DelState).IsModified = false;
                    db.SaveChanges();

                    Lg.AddLog("修改" + LogTxt + "状态", "Content", 2);

                    //推送更新
                    PushUpdate(id);

                    return Json(new { code = "200", msg = "修改成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("内容状态修改失败:" + ex.Message);
                return Json(new { code = "201", msg = "修改失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 批量删除内容
        /// </summary>
        /// <param name="idList">内容id集合</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelContentAll(string idList, int page)
        {
            try
            {
                using (PrizeDrawContext db = new PrizeDrawContext())
                {
                    //日志内容
                    string LogTxt = string.Empty;

                    //获取待删除内容id集合
                    string[] sArray = idList.Split(',');
                    int[] IdList = new int[sArray.Length];
                    for (int i = 0; i < sArray.Length; i++)
                    {
                        IdList[i] = Int32.Parse(sArray[i]);
                    }
                    for (int i = 0; i < IdList.Length; i++)
                    {
                        int id = IdList[i];

                        //日志内容
                        LogTxt = db.SysMenus.Where(m => m.ContentType == db.SysContents.Where(c => c.ID == id).FirstOrDefault().Type).AsNoTracking().FirstOrDefault().MenuName;

                        SysContent sysContent = new SysContent()
                        {
                            ID = id,
                            DelState = 1,
                            EditTime = DateTime.Now
                        };

                        db.Entry(sysContent).State = EntityState.Modified;
                        //不更新的字段
                        db.Entry(sysContent).Property(x => x.Title).IsModified = false;
                        db.Entry(sysContent).Property(x => x.ContentDescribe).IsModified = false;
                        db.Entry(sysContent).Property(x => x.Type).IsModified = false;
                        db.Entry(sysContent).Property(x => x.SysLibraryId).IsModified = false;
                        db.Entry(sysContent).Property(x => x.Sort).IsModified = false;
                        db.Entry(sysContent).Property(x => x.CrateTime).IsModified = false;
                        db.Entry(sysContent).Property(x => x.IsEnable).IsModified = false;
                        db.SaveChanges();

                        //推送更新
                        PushUpdate(id);
                    }
                    int npage = 0;
                    int Count = db.SysContents.Where(u => u.DelState == 0).AsNoTracking().Count();
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

                    Lg.AddLog("删除" + LogTxt, "Content", 3);

                    return Json(new { code = "200", page = npage, msg = "删除成功!" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("内容删除失败:" + ex.Message);
                return Json(new { code = "201", msg = "删除失败，请重试或联/系管理员!" });
            }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload()
        {
            try
            {
                var file = Request.Files[0];
                var filecombin = file.FileName.Split('.');
                if (file == null || String.IsNullOrEmpty(file.FileName) || file.ContentLength == 0 || filecombin.Length < 2)
                {
                    return Json(new
                    {
                        fileid = 0,
                        src = "",
                        name = "",
                        msg = "上传出错 请检查文件名 或 文件内容"
                    });
                }

                //判断文件类型
                string FileType = file.ContentType.Split('/')[0];
                string TypePath = string.Empty;
                switch (FileType)
                {
                    case "image":
                        TypePath = "Images";
                        break;
                    case "video":
                        TypePath = "Videos";
                        break;
                }

                //定义本地路径位置
                string local = "Files\\UploadFiles\\" + System.DateTime.Now.ToString("yyyy-MM-dd") + "\\" + TypePath;
                string filePathName = string.Empty;
                string localPath = Path.Combine(HttpRuntime.AppDomainAppPath, local);

                var tmpName = Server.MapPath("~/Files/UploadFilse/" + System.DateTime.Now.ToString("yyyy-MM-dd") + "/") + TypePath + "/";

                //获取文件扩展名
                string strExtension = file.FileName.Substring(file.FileName.LastIndexOf("."));

                var tmp = DateTime.Now.ToFileTimeUtc() + strExtension;
                var tmpIndex = 0;
                //判断是否存在相同文件名的文件 相同累加1继续判断
                while (System.IO.File.Exists(tmpName + tmp))
                {
                    tmp = filecombin[0] + "_" + ++tmpIndex + "." + filecombin[1];
                }

                //不带路径的最终文件名
                filePathName = tmp;

                if (!System.IO.Directory.Exists(localPath))
                    System.IO.Directory.CreateDirectory(localPath);
                string localURL = Path.Combine(local, filePathName);
                file.SaveAs(Path.Combine(localPath, filePathName));   //保存文件

                if (FileType == "image")
                {

                    //保存缩略图
                    string thumbPath = Path.Combine(HttpRuntime.AppDomainAppPath, local + "\\thumb");
                    if (!System.IO.Directory.Exists(thumbPath))
                        System.IO.Directory.CreateDirectory(thumbPath);
                    var thumbImage = new WebImage(file.InputStream);
                    thumbImage.Resize(80, 80);
                    thumbImage.Save(Path.Combine(thumbPath, filePathName));

                    return Json(new
                    {
                        src = "/" + localURL.Trim().Replace("\\", "/"),
                        thumbsrc = local.Trim().Replace("\\", "/") + "/thumb/" + filePathName,
                        name = Path.GetFileNameWithoutExtension(file.FileName),   // 获取文件名不含后缀名
                        msg = "上传成功",
                        fileName = System.DateTime.Now.ToString("yyyy-MM-dd") + "/" + filePathName//最终返回的路径及文件名称
                    });
                }

                return Json(new
                {
                    src = "/" + localURL.Trim().Replace("\\", "/"),
                    name = Path.GetFileNameWithoutExtension(file.FileName),   // 获取文件名不含后缀名
                    msg = "上传成功",
                    fileName = System.DateTime.Now.ToString("yyyy-MM-dd") + "/" + filePathName//最终返回的路径及文件名称
                });
            }
            catch(Exception ex)
            {
                LogHelper.ErrorLog("文件上传错误:" + ex.Message);
                return Json(new
                {
                    src = "",
                    name = "",   // 获取文件名不含后缀名
                    msg = "上传出错",
                    fileName = ""
                });
            }
        }
        #endregion

        #region 奖项管理
        /// <summary>
        /// 奖项内容添加页
        /// </summary>
        /// <returns></returns>
        public ActionResult SlideAdd(string MenuType)
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                //当前用户可管理场馆
                List<SysLibrary> libList = GetLibraryList();
                ViewData["datalist"] = libList;
                ViewBag.MenuType = MenuType;
                return View();
            }
        }

        /// <summary>
        /// 添加奖项内容
        /// </summary>
        /// <param name="Slide">奖项内容</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SlideAdd(FormCollection Slide)
        {
            try
            {
                using (PrizeDrawContext db = new PrizeDrawContext())
                {
                    //当前用户可管理场馆
                    List<SysLibrary> libList = GetLibraryList();
                    if (libList.Count == 0)
                    {
                        return Json(new { code = "202", msg = "当前用户未分配可管理场馆，请联系系统管理员！" });
                    }
                    ViewData["datalist"] = libList;

                    //奖项内容
                    if (Slide["Library"] != null)
                    {
                        if (Check("ContentAdd", 0, int.Parse(Slide["Library"]), Slide["Title"]))
                        {
                            //日志内容
                            int MenuType = int.Parse(Slide["MenuType"]);
                            string LogTxt = db.SysMenus.Where(m => m.ContentType == MenuType).AsNoTracking().FirstOrDefault().MenuName;

                            SysContent sysContent = new SysContent()
                            {
                                Title = Slide["Title"],
                                Sort = int.Parse(Slide["Sort"]),
                                ContentDescribe = HttpUtility.UrlDecode(Slide["Describe"]),
                                SysLibraryId = int.Parse(Slide["Library"]),
                                Type = int.Parse(Slide["MenuType"]),
                                CrateTime = DateTime.Now,
                                EditTime = DateTime.Now,
                                IsEnable = 0,
                                DelState = 0
                            };

                            db.SysContents.Add(sysContent);

                            //添加
                            db.SaveChanges();

                            string[] sArray = Slide["FilePath"].Substring(1, Slide["FilePath"].Length - 1).Split('|');

                            //添加文件
                            for (int i = 0; i < sArray.Count(); i++)
                            {
                                SysFile sysFile = new SysFile()
                                {
                                    SysContentId = sysContent.ID,
                                    Type = 0,
                                    FilePath = sArray[i],
                                    ThumbImage= GetThumbImage(sArray[i]),
                                    DelState = 0
                                };
                                db.SysFiles.Add(sysFile);
                            }

                            //添加
                            db.SaveChanges();

                            Lg.AddLog("添加" + LogTxt, "Content", 1);

                            //推送更新
                            PushUpdate(sysContent.ID);

                            return Json(new { code = "200", msg = "添加成功!" });
                        }
                        else
                        {
                            return Json(new { code = "202", msg = "内容已存在!" });
                        }
                    }
                    else
                    {
                        return Json(new { code = "202", msg = "请选择奖项所属抽奖活动!" });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("奖项添加失败:" + ex.Message);
                return Json(new { code = "201", msg = "添加失败，请重试或联系管理员!" });
            }
        }

        /// <summary>
        /// 获取缩略图地址
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetThumbImage(string path)
        {
            try
            {
                string[] ThumbPath = path.Split(new Char[] { '/' });
                return "/Files/UploadFiles/" + ThumbPath[2] + "/Images/thumb/" + ThumbPath[4];
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 奖项内容修改页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SlideEdit(int id)
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                //当前用户可管理场馆
                List<SysLibrary> libList = GetLibraryList();
                ViewData["datalist"] = libList;
                SysContent sysContent = db.SysContents.Find(id);
                SysFile File = db.SysFiles.Where(f => f.SysContentId == id).AsNoTracking().FirstOrDefault();
                //文件名
                ViewBag.File = File.FilePath;

                return View(sysContent);
            }
        }

        /// <summary>
        /// 修改奖项内容
        /// </summary>
        /// <param name="Slide">奖项内容</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SlideEdit(FormCollection Slide)
        {
            try
            {
                using (PrizeDrawContext db = new PrizeDrawContext())
                {
                    //当前用户可管理抽奖活动
                    List<SysLibrary> libList = GetLibraryList();
                    if (libList.Count == 0)
                    {
                        return Json(new { code = "202", msg = "当前用户未分配可管理抽奖活动，请联系系统管理员！" });
                    }
                    ViewData["datalist"] = libList;

                    //奖项内容
                    if (Slide["Library"] != null)
                    {
                        if (Check("ContentEdit", int.Parse(Slide["ID"]), int.Parse(Slide["Library"]), Slide["Title"]))
                        {
                            //奖项内容
                            int id = int.Parse(Slide["ID"]);
                            string LogTxt = db.SysMenus.Where(m => m.ContentType == db.SysContents.Where(c => c.ID == id).FirstOrDefault().Type).AsNoTracking().FirstOrDefault().MenuName;

                            SysContent sysContent = new SysContent()
                            {
                                ID = int.Parse(Slide["ID"]),
                                Title = Slide["Title"],
                                Sort = int.Parse(Slide["Sort"]),
                                ContentDescribe = HttpUtility.UrlDecode(Slide["Describe"]),
                                SysLibraryId = int.Parse(Slide["Library"]),
                                EditTime = DateTime.Now
                            };

                            db.Entry(sysContent).State = EntityState.Modified;
                            //不更新的字段
                            db.Entry(sysContent).Property(x => x.Type).IsModified = false;
                            db.Entry(sysContent).Property(x => x.CrateTime).IsModified = false;
                            db.Entry(sysContent).Property(x => x.IsEnable).IsModified = false;
                            db.Entry(sysContent).Property(x => x.DelState).IsModified = false;

                            db.SaveChanges();

                            string[] sArray = Slide["FilePath"].Substring(1, Slide["FilePath"].Length - 1).Split('|');

                            //删除文件
                            SysFile sysFileOld = db.SysFiles.FirstOrDefault(s => s.SysContentId == sysContent.ID);
                            if (sysFileOld != null)
                            {
                                db.SysFiles.Remove(sysFileOld);
                            }

                            //添加文件
                            for (int i = 0; i < sArray.Count(); i++)
                            {
                                SysFile sysFile = new SysFile()
                                {
                                    SysContentId = sysContent.ID,
                                    Type = 0,
                                    FilePath = sArray[i],
                                    ThumbImage = GetThumbImage(sArray[i]),
                                    DelState = 0
                                };
                                db.SysFiles.Add(sysFile);
                            }
                            db.SaveChanges();

                            Lg.AddLog("修改" + LogTxt, "Content", 2);

                            //推送更新
                            PushUpdate(id);

                            return Json(new { code = "200", msg = "修改成功!" });
                        }
                        else
                        {
                            return Json(new { code = "202", msg = "奖项已存在!" });
                        }
                    }
                    else
                    {
                        return Json(new { code = "202", msg = "请选择奖项所属抽奖活动!" });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("奖项修改失败:" + ex.Message);
                return Json(new { code = "201", msg = "修改失败，请重试或联系管理员!" });
            }
        }
        #endregion

        /// <summary>
        /// 获取当前用户可管理抽奖活动
        /// </summary>
        /// <returns></returns>
        public List<SysLibrary> GetLibraryList()
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("User"), db.SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey);
                UserCookie user = JsonConvert.DeserializeObject<UserCookie>(Cookies);
                var Libids = db.SysLibraryUsers.Where(u => u.SysUserID == user.UserId).AsNoTracking().Select(u => u.SysLibraryId).ToList();
                return db.SysLibrarys.Where(lib => Libids.Contains(lib.ID) & lib.DelState == 0 & lib.IsEnable == 0).AsNoTracking().ToList();
            }
        }

        /// <summary>
        /// 检查内容是否存在
        /// </summary>
        /// <param name="Action">方法</param>
        /// <param name="Id">新增时为0</param>
        /// <param name="LibraryId">场馆Id</param>
        /// /// <param name="Title">标题</param>
        /// <returns></returns>
        public bool Check(string Action, int Id,int LibraryId, string Title)
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                if (Action == "ContentEdit")
                {
                    if (db.SysContents.Where(u => u.Title == Title && u.SysLibraryId == LibraryId).AsNoTracking().Count() >= 1)
                    {
                        //AsNoTracking将Hold住的对象释放掉
                        SysContent Content = db.SysContents.AsNoTracking().FirstOrDefault(u => u.ID == Id);
                        if (Content.Title == Title)
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
                    if (db.SysContents.Where(u => u.Title == DbFunctions.AsNonUnicode(Title) && u.SysLibraryId == LibraryId).AsNoTracking().Count() > 0)
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

        /// <summary>
        /// 推送更新
        /// </summary>
        public void PushUpdate(int ContentId)
        {
            using (PrizeDrawContext db = new PrizeDrawContext())
            {
                var Content = db.SysContents.Find(ContentId);
                string ToLibraryName = db.SysLibrarys.Find(Content.SysLibraryId).LibraryName;
                int type = Content.Type;
                var msg = db.SysMenus.Where(m => m.ContentType == type).FirstOrDefault().MenuName + "内容更新";
                MyHub.Show(ToLibraryName, JsonConvert.SerializeObject(new
                {
                    msg = msg,
                    action = "Notice",
                    type = type
                }));
            }
        }
    }
}