using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMS.Core.Cache
{
    public interface ICacheProvider
    {
        object Get(string key);
        void Set(string key, object value, int minutes, bool isAbsoluteExpiration, Action<string, object, string> onRemove);
        void Remove(string key);
        void Clear(string keyRegex);
    }
}
