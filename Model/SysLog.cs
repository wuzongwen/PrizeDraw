using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SysLog
    {
        [Key]
        [DisplayName("日志ID")]
        public int ID { get; set; }

        /// <summary>
        /// 0:登陆日志;1:添加日志;2:修改日志;3:删除日志;4:其他日志
        /// </summary>
        [DisplayName("日志类型")]
        public int Type { get; set; }

        [DisplayName("所属模块")]
        public string Modular { get; set; }

        [DisplayName("操作内容")]
        public string Details { get; set; }

        [DisplayName("操作用户")]
        public string UserName { get; set; }

        [DisplayName("操作设备Ip")]
        public string Ip { get; set; }

        [DisplayName("操作设备地址")]
        public string Address { get; set; }

        [DisplayName("操作时间")]
        public DateTime CrateTime { get; set; }
    }
}