using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SysPrizeDrawUser
    {
        [Key]
        [DisplayName("记录ID")]
        public int ID { get; set; }

        [DisplayName("用户openid")]
        public string openid { get; set; }

        [DisplayName("昵称")]
        public string nickname { get; set; }

        [DisplayName("城市")]
        public string city { get; set; }

        [DisplayName("省份")]
        public string province { get; set; }

        [DisplayName("国家")]
        public string country { get; set; }

        [DisplayName("头像")]
        public string headimgurl { get; set; }

        [DisplayName("创建时间")]
        public DateTime CrateTime { get; set; }

        [DisplayName("修改时间")]
        public DateTime EditTime { get; set; }

        public ICollection<SysPrizeDrawRecord> SysPrizeDrawRecords { get; set; }
    }
}
