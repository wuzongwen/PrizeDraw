using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SysFile
    {
        [Key]
        [DisplayName("附件ID")]
        public int ID { get; set; }

        [DisplayName("内容Id")]
        public int SysContentId { get; set; }

        [DisplayName("资源类型:0图片,1视频,2其它......")]
        public int Type { get; set; }

        [DisplayName("资源路径")]
        public string FilePath { get; set; }

        [DisplayName("缩略图")]
        public string ThumbImage { get; set; }

        [DisplayName("删除状态:0正常,1已删除")]
        public int? DelState { get; set; }

        [JsonIgnore]
        public virtual SysContent SysContent { get; set; }
    }
}