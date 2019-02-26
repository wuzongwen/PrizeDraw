using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Model;
using System.IO;

namespace DAL
{
    public class PrizeDrawInitializer:DropCreateDatabaseIfModelChanges<PrizeDrawContext>
    {
        protected override void Seed(PrizeDrawContext context)
        {
            //添加默认用户
            var sysUsers = new List<SysUser> {
                new SysUser{UserName="admin",Email="admin@qq.com",Password="7AA33D66415397B416E99821B7C3F13",DelState=0,IsEnable=0,CrateTime=DateTime.Now,EditTime=DateTime.Now },
                new SysUser{UserName="general",Email="general@qq.com",Password="7AA33D66415397B416E99821B7C3F13",DelState=0,IsEnable=0,CrateTime=DateTime.Now,EditTime=DateTime.Now }
            };
            sysUsers.ForEach(s => context.SysUsers.Add(s));
            context.SaveChanges();

            //添加系统信息
            var sysProgramInfos = new List<SysProgramInfo> {
                new SysProgramInfo{
                    ProgramVersion="2.0 build-190112",CookieSecretKey="90-=uiop",UpdateTime=DateTime.Parse("2019-01-12 17:50:00"),FirstRunTime=DateTime.Now,Type=0 }
            };
            sysProgramInfos.ForEach(s => context.SysProgramInfos.Add(s));
            context.SaveChanges();

            //添加默认菜单
            var sysMenus = new List<SysMenu> {
                new SysMenu{MenuName="奖项管理",ContentType=0,Type=0,Sort=2,CrateTime=DateTime.Now,EditTime=DateTime.Now,IsEnable=0,DelState=0 }
            };
            sysMenus.ForEach(s => context.SysMenus.Add(s));
            context.SaveChanges();

            //添加权限
            var sysRoles = new List<SysRole> {
                new SysRole{ RoleName="Administrator",RoleDesc="超级管理员"},
                new SysRole{ RoleName="General",RoleDesc="一般用户"}
            };
            sysRoles.ForEach(s => context.SysRoles.Add(s));
            context.SaveChanges();

            var sysUserRoles = new List<SysUserRole> {
                new SysUserRole{ SysUserID=1,SysRoleId=1},
                new SysUserRole{ SysUserID=2,SysRoleId=2}
            };
            sysUserRoles.ForEach(s => context.SysUserRoles.Add(s));
            context.SaveChanges();

            var sysActionRoles = new List<SysActionRole> {
                //用户模块权限
                new SysActionRole{ControllerName="User",ActionName="List",User="Administrator,General"},
                new SysActionRole{ControllerName="User",ActionName="UserAdd",User="Administrator"},
                new SysActionRole{ControllerName="User",ActionName="UserEdit",User="Administrator"},
                new SysActionRole{ControllerName="User",ActionName="EditEnable",User="Administrator"},
                new SysActionRole{ControllerName="User",ActionName="PwdEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="User",ActionName="DelUser",User="Administrator"},
                new SysActionRole{ControllerName="User",ActionName="DelUserAll",User="Administrator"},
                //抽奖用户管理模块权限
                new SysActionRole{ControllerName="Library",ActionName="List",User="Administrator,General"},
                new SysActionRole{ControllerName="Library",ActionName="LibraryAdd",User="Administrator"},
                new SysActionRole{ControllerName="Library",ActionName="LibraryEdit",User="Administrator"},
                new SysActionRole{ControllerName="Library",ActionName="EditEnable",User="Administrator"},
                new SysActionRole{ControllerName="Library",ActionName="DelLibrary",User="Administrator"},
                new SysActionRole{ControllerName="Library",ActionName="DelLibraryAll",User="Administrator"},
                new SysActionRole{ControllerName="Library",ActionName="LibraryUser",User="Administrator,General"},
                new SysActionRole{ControllerName="Library",ActionName="DelLibraryUser",User="Administrator"},
                new SysActionRole{ControllerName="Library",ActionName="AddLibraryUser",User="Administrator"},
                //内容模块权限
                new SysActionRole{ControllerName="Content",ActionName="ContentList",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="DelContent",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="EditEnable",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="DelContentAll",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="NoticeAdd",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="NoticeEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="VideoAdd",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="VideoEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="VideoPlay",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="SlideAdd",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="SlideEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="SlideImageEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="DelImage",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="UploadForImageEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="Upload",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="NewBookList",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="NewBookAdd",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="NewBookEdit",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="DelNewBook",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="NewBookEditEnable",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="DelNewBookAll",User="Administrator,General"},
                new SysActionRole{ControllerName="Content",ActionName="GetBookInfo",User="Administrator,General"},
                //其它模块
                new SysActionRole{ControllerName="Other",ActionName="AdditionalDataList",User="Administrator,General"},
                new SysActionRole{ControllerName="Other",ActionName="AdditionalDataEdit",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="ExcelDownload",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="UploadExcel",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="DelAdditionalData",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="AdditionalDataEditEnable",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="DelAdditionalDataAll",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="CustDeviceList",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="CustDeviceAdd",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="CustDeviceEdit",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="DelCustDevice",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="CustDeviceEditEnable",User="Administrator"},
                new SysActionRole{ControllerName="Other",ActionName="DelCustDeviceAll",User="Administrator"}
            };
            sysActionRoles.ForEach(s => context.SysActionRoles.Add(s));
            context.SaveChanges();

        }
    }
}