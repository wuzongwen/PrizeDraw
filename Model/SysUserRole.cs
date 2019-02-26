using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SysUserRole
    {
        [Key]
        [DisplayName("用户权限ID")]
        public int ID { get; set; }

        [DisplayName("用户ID")]
        public int SysUserID { get; set; }

        [DisplayName("角色ID")]
        public int SysRoleId { get; set; }

        public virtual SysUser SysUser { get; set; }

        public virtual SysRole SysRole { get; set; }
    }
}