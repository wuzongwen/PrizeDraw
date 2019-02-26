using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SysPrizeDrawRecord
    {
        [Key]
        [DisplayName("ID")]
        public int ID { get; set; }

        [DisplayName("用户ID")]
        public int SysPrizeDrawUserId { get; set; }

        [DisplayName("活动ID")]
        public int SysLibraryId { get; set; }

        [DisplayName("状态0:抽奖未开始;1:中奖;2:未中奖")]
        public int State { get; set; }

        [DisplayName("奖项ID")]
        public int? SysContentId { get; set; }

        [DisplayName("创建时间")]
        public DateTime CrateTime { get; set; }

        [DisplayName("修改时间")]
        public DateTime EditTime { get; set; }

        public virtual SysPrizeDrawUser SysPrizeDrawUser { get; set; }

        public virtual SysLibrary SysLibrary { get; set; }

        public virtual SysContent SysContent { get; set; }
    }
}
