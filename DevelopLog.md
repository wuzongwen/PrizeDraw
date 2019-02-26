# 实体类修改后更新数据库方法

工具 -> 库程序包管理器 -> 程序包管理器控制台
运行命令 Enable-Migrations

这时候，你会发现在程序端多出一个文件夹叫Migrations
这里面有一个Configuration.cs文件
打开它，然后修改成如下样子，
public Configuration()
{
AutomaticMigrationsEnabled = true; //这里变成true
ContextKey = "codefirst.DAL.BaseContext";
}

修改完成后，运行

更新数据库
Update-Database -Force
创建数据库
Update-Database -Verbose


--获取Guid
string Guid = System.Guid.NewGuid().ToString("D");
xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
