using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SysRole
    {
        [Key]
        [DisplayName("角色ID")]
        public int ID { get; set; }

        [DisplayName("角色名称")]
        public string RoleName { get; set; }

        [DisplayName("角色详情")]
        public string RoleDesc { get; set; }

        public ICollection<SysUserRole> SysUserRoles { get; set; }
    }
}