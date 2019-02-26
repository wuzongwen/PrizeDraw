using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SysMenu
    {
        [Key]
        [DisplayName("菜单ID")]
        public int ID { get; set; }

        [DisplayName("菜单名称")]
        public string MenuName { get; set; }

        [DisplayName("菜单类型")]
        public int Type { get; set; }

        [DisplayName("内容类型")]
        public int ContentType { get; set; }

        [DisplayName("排序")]
        public int Sort { get; set; }

        [DisplayName("创建时间")]
        public DateTime CrateTime { get; set; }

        [DisplayName("修改时间")]
        public DateTime EditTime { get; set; }

        [DisplayName("启用状态:0启用,1禁用")]
        public int IsEnable { get; set; }

        [DisplayName("删除状态:0正常,1已删除")]
        public int? DelState { get; set; }
    }
}
