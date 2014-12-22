GMS
===

基于EF+MVC+Bootstrap的通用后台管理系统及架构 http://www.cnblogs.com/guozili/archive/2014/01/02/3496265.html#2850220

源码下载：http://pan.baidu.com/s/1dDBqSBR
平台：VS2010+，Sql Server, MVC4，Silverlight5_Tools(可选)
脚本部署：新建5个数据库（GMSAccount、GMSCms、GMSCrm、GMSLog、GMSOA），并执行源码里的Deploy.sql初始化表和数据
配置更改：更改GMS.Web.Admin\Config\DaoConfig.xml下的数据库连接字符串
启动：VS IIS Express或建立IIS对应网站（主网站项目GMS.Web.Admin）
登录：初始化用户名：admin 密码：111111
