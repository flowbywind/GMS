using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GMS.Framework.Utility;

namespace GMS.Framework.Contract
{
    /// <summary>
    /// 业务异常，用于在后端抛出到前端做相应处理
    /// </summary>
    public class BusinessException : Exception
    {
        public BusinessException()
            : this(string.Empty)
        {
        }

        public BusinessException(string message):
            this("error", message)
        {
        }

        public BusinessException(string name, string message)
            :base(message)
        {
            this.Name = name;
        }

        public BusinessException(string message, Enum errorCode)
            : this("error", message, errorCode)
        {
        }

        public BusinessException(string name, string message, Enum errorCode)
            : base(message)
        {
            this.Name = name;
            this.ErrorCode = errorCode;
        }

        public string Name { get; set; }
        public Enum ErrorCode { get; set; }
    }
}