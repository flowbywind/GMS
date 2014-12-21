using System;
using System.Web;
using GMS.Framework.Utility;

namespace GMS.Web
{
    public class CookieContext : IAuthCookie
    {
        public CookieContext()
        {
        }
        
        /// <summary>
        /// Cache或者Cookie的Key前缀
        /// </summary>
        public virtual string KeyPrefix
        {
            get
            {
                return "Context_";
            }
        }

        public void Set(string key, string value, int expiresHours = 0)
        {
            if (expiresHours > 0)
                Cookie.Save(KeyPrefix + key, value, expiresHours);
            else
                Cookie.Save(KeyPrefix + key, value);
        }
        
        #region IAuthCookie
        private int userExpiresHours = 10;
        public virtual int UserExpiresHours
        {
            get
            {
                return userExpiresHours;
            }
            set
            {
                userExpiresHours = value;
            }
        }

        public virtual string UserName
        {
            get
            {
                return HttpUtility.UrlDecode(Cookie.GetValue(KeyPrefix + "UserName"));
            }
            set
            {
                Cookie.Save(KeyPrefix + "UserName", HttpUtility.UrlEncode(value), UserExpiresHours);
            }
        }

        public virtual int UserId
        {
            get
            {
                return Cookie.GetValue(KeyPrefix + "UserId").ToInt();
            }
            set
            {
                Cookie.Save(KeyPrefix + "UserId", value.ToString(), UserExpiresHours);
            }
        }

        public virtual Guid UserToken
        {
            get
            {
                return Cookie.GetValue(KeyPrefix + "UserToken").ToGuid();
            }
            set
            {
                Cookie.Save(KeyPrefix + "UserToken", value.ToString(), UserExpiresHours);
            }
        }

        public virtual Guid VerifyCodeGuid
        {
            get
            {
                var verifyCodeGuid = Cookie.GetValue(KeyPrefix + "VerifyCodeGuid");
                return Guid.Parse(verifyCodeGuid);
            }
            set
            {
                Cookie.Save(KeyPrefix + "VerifyCodeGuid", value.ToString(), 1);
            }
        }

        public virtual string VerifyCode
        {
            get
            {
                var verifyCode = Encrypt.Decode(Cookie.GetValue(KeyPrefix + "VerifyCode"));

                //获取完Cookie后马上过期，重新生成新的验证码
                Cookie.Save(KeyPrefix + "VerifyCode", Encrypt.Encode(DateTime.Now.Ticks.ToString()), 1);

                return verifyCode;
            }
            set
            {
                Cookie.Save(KeyPrefix + "VerifyCode", Encrypt.Encode(value), 1);
            }
        }

        public virtual int LoginErrorTimes
        {
            get
            {
                return Cookie.GetValue(KeyPrefix + "LoginErrorTimes").ToInt();
            }
            set
            {
                Cookie.Save(KeyPrefix + "LoginErrorTimes", value.ToString(), 1);
            }
        }

        public bool IsNeedVerifyCode
        {
            get
            {
                return LoginErrorTimes > 1;
            }
        }
        #endregion
    }
}
