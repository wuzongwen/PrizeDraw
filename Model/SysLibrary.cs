using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SysLibrary
    {
        [Key]
        [DisplayName("场馆ID")]
        public int ID { get; set; }

        [DisplayName("所在城市")]
        public string City { get; set; }

        [DisplayName("场馆名称")]
        public string LibraryName { get; set; }

        [DisplayName("场馆描述")]
        public string LibraryDescribe { get; set; }

        [DisplayName("场馆类型:0总馆,1分馆")]
        public int Type { get; set; }

        [DisplayName("上级场馆ID")]
        public int FatherLibraryId { get; set; }

        [DisplayName("馆代码")]
        public string LibraryCode { get; set; }

        [DisplayName("创建时间")]
        public DateTime CrateTime { get; set; }

        [DisplayName("修改时间")]
        public DateTime EditTime { get; set; }

        [DisplayName("启用状态:0启用,1禁用")]
        public int IsEnable { get; set; }

        [DisplayName("删除状态:0正常,1已删除")]
        public int? DelState { get; set; }

        public ICollection<SysLibraryUser> SysLibraryUsers { get; set; }

        public ICollection<SysContent> SysContents { get; set; }

        public ICollection<SysPrizeDrawRecord> SysPrizeDrawRecord { get; set; }
    }
}