using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SysContent
    {
        [Key]
        [DisplayName("内容ID")]
        public int ID { get; set; }

        [DisplayName("标题")]
        public string Title { get; set; }

        [DisplayName("描述")]
        public string ContentDescribe { get; set; }

        [DisplayName("内容类型:0图片,1视频,2通知公告......")]
        public int Type { get; set; }

        [DisplayName("资源所属馆Id")]
        public int SysLibraryId { get; set; }

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

        public virtual SysLibrary SysLibrary { get; set; }

        public ICollection<SysFile> SysFiles { get; set; }

        public ICollection<SysPrizeDrawRecord> SysPrizeDrawRecords { get; set; }
    }
}