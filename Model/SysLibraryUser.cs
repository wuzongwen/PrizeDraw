using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class SysLibraryUser
    {
        [Key]
        [DisplayName("ID")]
        public int ID { get; set; }

        [DisplayName("用户ID")]
        public int SysUserID { get; set; }

        [DisplayName("场馆ID")]
        public int SysLibraryId { get; set; }

        public virtual SysUser SysUser { get; set; }

        public virtual SysLibrary SysLibrary { get; set; }
    }
}