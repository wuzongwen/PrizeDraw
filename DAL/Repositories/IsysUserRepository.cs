using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAL.Repositories
{
    public interface ISysUserRepository
    {
        //查询所有用户

        IQueryable<SysUser> SelectAll();

        //通过用户名查询用户

        SysUser SelectByName(string userName);

        //登陆验证

        SysUser SelectByNamePs(string userName, string passWord);

        //添加用户

        void Add(SysUser sysUser);

        //删除用户

        bool Delete(int id);

    }
}