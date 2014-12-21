using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMS.Account.Contract;
using GMS.Account.DAL;
using GMS.Framework.Utility;
using System.Data.Objects;
using GMS.Framework.Contract;
using EntityFramework.Extensions;
using GMS.Core.Cache;
using GMS.Core.Config;

namespace GMS.Account.BLL
{
    public class AccountService : IAccountService
    {
        private readonly int _UserLoginTimeoutMinutes = CachedConfigContext.Current.SystemConfig.UserLoginTimeoutMinutes;
        private readonly string _LoginInfoKeyFormat = "LoginInfo_{0}";


        public LoginInfo GetLoginInfo(Guid token)
        {
            return CacheHelper.Get<LoginInfo>(string.Format(_LoginInfoKeyFormat, token), () =>
            {
                using (var dbContext = new AccountDbContext())
                {
                    //如果有超时的，启动超时处理
                    var timeoutList = dbContext.FindAll<LoginInfo>(p => EntityFunctions.DiffMinutes(DateTime.Now, p.LastAccessTime) > _UserLoginTimeoutMinutes);
                    if (timeoutList.Count > 0)
                    {
                        foreach (var li in timeoutList)
                            dbContext.LoginInfos.Remove(li);
                    }

                    dbContext.SaveChanges();


                    var loginInfo = dbContext.FindAll<LoginInfo>(l => l.LoginToken == token).FirstOrDefault();
                    if (loginInfo != null)
                    {
                        loginInfo.LastAccessTime = DateTime.Now;
                        dbContext.Update<LoginInfo>(loginInfo);
                    }

                    return loginInfo;
                }
            });
        }

        public LoginInfo Login(string loginName, string password)
        {
            LoginInfo loginInfo = null;

            password = Encrypt.MD5(password);
            loginName = loginName.Trim();

            using (var dbContext = new AccountDbContext())
            {
                var user = dbContext.Users.Include("Roles").Where(u => u.LoginName == loginName && u.Password == password && u.IsActive).FirstOrDefault();
                if (user != null)
                {
                    var ip = Fetch.UserIp;
                    loginInfo = dbContext.FindAll<LoginInfo>(p => p.LoginName == loginName && p.ClientIP == ip).FirstOrDefault();
                    if (loginInfo != null)
                    {
                        loginInfo.LastAccessTime = DateTime.Now;
                    }
                    else
                    {
                        loginInfo = new LoginInfo(user.ID, user.LoginName);
                        loginInfo.ClientIP = ip;
                        loginInfo.BusinessPermissionList = user.BusinessPermissionList;
                        dbContext.Insert<LoginInfo>(loginInfo);
                    }
                }
            }

            return loginInfo;
        }

        public void Logout(Guid token)
        {
            using (var dbContext = new AccountDbContext())
            {
                var loginInfo = dbContext.FindAll<LoginInfo>(l => l.LoginToken == token).FirstOrDefault();
                if (loginInfo != null)
                {
                    dbContext.Delete<LoginInfo>(loginInfo);
                }
            }

            CacheHelper.Remove(string.Format(_LoginInfoKeyFormat, token));
        }

        public void ModifyPwd(User user)
        {
            user.Password = Encrypt.MD5(user.Password);

            using (var dbContext = new AccountDbContext())
            {
                if (dbContext.Users.Any(l => l.ID == user.ID && user.Password == l.Password))
                {
                    if (!string.IsNullOrEmpty(user.NewPassword))
                        user.Password = Encrypt.MD5(user.NewPassword);

                    dbContext.Update<User>(user);
                }
                else
                {
                    throw new BusinessException("Password", "原密码不正确！");
                }
            }
        }

        public User GetUser(int id)
        {
            using (var dbContext = new AccountDbContext())
            {
                return dbContext.Users.Include("Roles").Where(u=>u.ID == id).SingleOrDefault();
            }
        }

        public IEnumerable<User> GetUserList(UserRequest request = null)
        {
            request = request ?? new UserRequest();
            
            using (var dbContext = new AccountDbContext())
            {
                IQueryable<User> users = dbContext.Users.Include("Roles");

                if (!string.IsNullOrEmpty(request.LoginName))
                    users = users.Where(u => u.LoginName.Contains(request.LoginName));

                if (!string.IsNullOrEmpty(request.Mobile))
                    users = users.Where(u => u.Mobile.Contains(request.Mobile));

                return users.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public void SaveUser(User user)
        {
            using (var dbContext = new AccountDbContext())
            {
                if (user.ID > 0)
                {
                    dbContext.Update<User>(user);

                    var roles = dbContext.Roles.Where(r => user.RoleIds.Contains(r.ID)).ToList();
                    user.Roles = roles;
                    dbContext.SaveChanges();
                }
                else
                {
                    var existUser = dbContext.FindAll<User>(u => u.LoginName == user.LoginName);
                    if (existUser.Count > 0)
                    {
                        throw new BusinessException("LoginName", "此登录名已存在！");
                    }
                    else
                    {
                        dbContext.Insert<User>(user);

                        var roles = dbContext.Roles.Where(r => user.RoleIds.Contains(r.ID)).ToList();
                        user.Roles = roles;
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        public void DeleteUser(List<int> ids)
        {
            using (var dbContext = new AccountDbContext())
            {
                dbContext.Users.Include("Roles").Where(u => ids.Contains(u.ID)).ToList().ForEach(a => { a.Roles.Clear(); dbContext.Users.Remove(a); });
                dbContext.SaveChanges();
            }
        }

        public Role GetRole(int id)
        {
            using (var dbContext = new AccountDbContext())
            {
                return dbContext.Find<Role>(id);
            }
        }

        public IEnumerable<Role> GetRoleList(RoleRequest request = null)
        {
            request = request ?? new RoleRequest();
            using (var dbContext = new AccountDbContext())
            {
                IQueryable<Role> roles = dbContext.Roles;

                if (!string.IsNullOrEmpty(request.RoleName))
                {
                    roles = roles.Where(u => u.Name.Contains(request.RoleName));
                }

                return roles.OrderByDescending(u => u.ID).ToPagedList(request.PageIndex, request.PageSize);
            }
        }

        public void SaveRole(Role role)
        {
            using (var dbContext = new AccountDbContext())
            {
                if (role.ID > 0)
                {
                    dbContext.Update<Role>(role);
                }
                else
                {
                    dbContext.Insert<Role>(role);
                }
            }
        }

        public void DeleteRole(List<int> ids)
        {
            using (var dbContext = new AccountDbContext())
            {
                dbContext.Roles.Include("Users").Where(u => ids.Contains(u.ID)).ToList().ForEach(a => { a.Users.Clear(); dbContext.Roles.Remove(a); });
                dbContext.SaveChanges();
            }
        }

        public Guid SaveVerifyCode(string verifyCodeText)
        {
            if (string.IsNullOrWhiteSpace(verifyCodeText))
                throw new BusinessException("verifyCode", "输入的验证码不能为空！");

            using (var dbContext = new AccountDbContext())
            {
                var verifyCode = new VerifyCode(){VerifyText = verifyCodeText, Guid = Guid.NewGuid()};
                dbContext.Insert<VerifyCode>(verifyCode);
                return verifyCode.Guid;
            }
        }

        public bool CheckVerifyCode(string verifyCodeText, Guid guid)
        {
            using (var dbContext = new AccountDbContext())
            {
                var verifyCode = dbContext.FindAll<VerifyCode>(v => v.Guid == guid && v.VerifyText == verifyCodeText).LastOrDefault();
                if (verifyCode != null)
                {
                    dbContext.VerifyCodes.Remove(verifyCode);
                    dbContext.SaveChanges();

                    //清除验证码大于2分钟还没请求的
                    var expiredTime = DateTime.Now.AddMinutes(-2);
                    dbContext.VerifyCodes.Where(v => v.CreateTime < expiredTime).Delete();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
