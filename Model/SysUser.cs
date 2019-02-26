using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SysUser
    {
        [Key]
        [DisplayName("用户ID")]
        public int ID { get; set; }

        [DisplayName("用户名")]
        public string UserName { get; set; }

        [DisplayName("用户邮箱")]
        public string Email { get; set; }

        [DisplayName("用户密码")]
        public string Password { get; set; }

        [DisplayName("创建时间")]
        public DateTime CrateTime { get; set; }

        [DisplayName("修改时间")]
        public DateTime EditTime { get; set; }

        [DisplayName("启用状态:0启用,1禁用")]
        public int IsEnable { get; set; }

        [DisplayName("删除状态:0正常,1已删除")]
        public int? DelState { get; set; }

        public virtual ICollection<SysUserRole> SysUserRoles { get; set; }

        public virtual ICollection<SysLibraryUser> SysLibraryUsers { get; set; }
    }
}