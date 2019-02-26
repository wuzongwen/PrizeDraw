using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Model;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Migrations;

namespace DAL
{
    public class PrizeDrawContext : DbContext
    {
        public PrizeDrawContext() : base("PrizeDrawContext")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //因为表名称默认为复数形式，这里是移除复数形式，所以为单数形式生成
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        /// <summary>
        /// 用户
        /// </summary>
        public DbSet<SysUser> SysUsers { get; set; }

        /// <summary>
        /// 抽奖活动
        /// </summary>
        public DbSet<SysLibrary> SysLibrarys { get; set; }
        public DbSet<SysLibraryUser> SysLibraryUsers { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public DbSet<SysRole> SysRoles { get; set; }
        public DbSet<SysUserRole> SysUserRoles { get; set; }
        public DbSet<SysActionRole> SysActionRoles { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public DbSet<SysContent> SysContents { get; set; }
        public DbSet<SysFile> SysFiles { get; set; }

        /// <summary>
        /// 系统信息
        /// </summary>
        public DbSet<SysProgramInfo> SysProgramInfos { get; set; }

        /// <summary>
        /// 系统菜单
        /// </summary>
        public DbSet<SysMenu> SysMenus { get; set; }

        /// <summary>
        /// 系统日志
        /// </summary>
        public DbSet<SysLog> SysLogs { get; set; }

        /// <summary>
        /// 参与用户
        /// </summary>
        public DbSet<SysPrizeDrawUser> SysPrizeDrawUsers { get; set; }

        /// <summary>
        /// 参与记录
        /// </summary>
        public DbSet<SysPrizeDrawRecord> SysPrizeDrawRecords { get; set; }
    }

}