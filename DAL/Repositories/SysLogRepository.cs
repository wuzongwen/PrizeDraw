using Common;
using Model;
using Model.ToolModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAL.Repositories
{
    public class SysLogRepository : IsysLogRepository 
    {
        protected PrizeDrawContext db = new PrizeDrawContext();

        //添加日志
        public void AddLog(string Details,string Modular,int Type)
        {
            try
            {
                using (PrizeDrawContext db = new PrizeDrawContext())
                {
                    //获取当前登陆用户
                    var Cookies = SecurityHelper.DecryptDES(CookieHelper.GetCookieValue("User"), db.SysProgramInfos.AsNoTracking().FirstOrDefault().CookieSecretKey);
                    UserCookie user = JsonConvert.DeserializeObject<UserCookie>(Cookies);
                    string ip = IpHelper.GetWebClientIp();
                    string city = IpHelper.GetCity(ip);
                    SysLog sysLog = new SysLog();
                    sysLog.Type = Type;
                    sysLog.Modular = Modular;
                    sysLog.Details = Details;
                    sysLog.UserName = user.UserName;
                    sysLog.CrateTime = DateTime.Now;
                    sysLog.Ip = ip;
                    sysLog.Address = city;
                    db.SysLogs.Add(sysLog);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(Details + "操作日志添加失败!" + ex.Message);
                throw;
            }
            
        }
    }
}