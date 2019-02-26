using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAL.Repositories
{
    public interface IsysLogRepository
    {
        /// <summary>
        /// 添加操作日志;Type,0:登陆日志;1:添加日志;2:修改日志;3:删除日志;4:其他日志
        /// </summary>
        /// <param name="Details">日志内容</param>
        /// <param name="Modular">模块</param>
        /// <param name="Type">类型</param>
        void AddLog(string Details, string Modular, int Type);
    }
}