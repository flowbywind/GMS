using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMS.Framework.Web
{
    public class HttpContextBase
    {

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
    }
}
