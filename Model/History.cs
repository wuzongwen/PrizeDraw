using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class History
    {
        [Key]
        [DisplayName("编号")]
        public string _id { get; set; }

        [DisplayName("标题")]
        public string title { get; set; }

        [DisplayName("图片地址")]
        public string pic { get; set; }

        [DisplayName("年份")]
        public int year { get; set; }

        [DisplayName("月份")]
        public int month { get; set; }

        [DisplayName("日期")]
        public int day { get; set; }

        [DisplayName("详情")]
        public string des { get; set; }

        [DisplayName("农历日期")]
        public string lunar { get; set; }
    }
}
