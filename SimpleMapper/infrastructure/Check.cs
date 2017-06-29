using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMapper
{
    public class Check
    {
        public static void ConnectionConfig(string config)
        {
            if (config == null || config.IsNullOrSpace())
            {
                throw new Exception("SqlSugarException.ArgumentNullException：" + ErrorMessage.ConnectionConfigIsNull);
            }
        }
    }
}
