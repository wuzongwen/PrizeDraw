using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ToolModels
{
    public class UserCookie
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public List<LibrarylistItem> LibraryList { get; set; }
    }

    public class LibrarylistItem
    {
        public int ID { get; set; }

        public string LibraryName { get; set; }
    }
}
