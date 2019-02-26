using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SysActionRole
    {
        [Key]
        [DisplayName("ID")]
        public int ID { get; set; }

        [DisplayName("Controller名称")]
        public string ControllerName { get; set; }

        [DisplayName("Action名称")]
        public string ActionName { get; set; }

        [DisplayName("角色")]
        public string User { get; set; }
    }
}