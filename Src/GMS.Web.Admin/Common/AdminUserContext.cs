using System;
using GMS.Core.Cache;

namespace GMS.Web.Admin.Common
{
    public class AdminUserContext : UserContext
    {
        public AdminUserContext()
            : base(AdminCookieContext.Current)
        {
        }

        public AdminUserContext(IAuthCookie authCookie)
            : base(authCookie)
        {
        }

        public static AdminUserContext Current
        {
            get
            {
                return CacheHelper.GetItem<AdminUserContext>();
            }
        }
    }
}
