﻿using SimpleMapper.infrastructure;
using SimpleMapper.Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMapper
{
    public class ProviderFactory
    {
        #region 成员定义
        private static Dictionary<string, BaseProvider> dbProviderFactoryDic = new Dictionary<string, BaseProvider>();

        private IList parameterList = new List<DbParameter>();//过程参数列表
        private Dictionary<string, object> dicPara = new Dictionary<string, object>();//输出参数列表
        #endregion

        #region 构造方法，实例化连接字符串
        private static void RegisterProviderFactory(string connectionString, BaseProvider provider)
        {
            provider.connectionString = connectionString;
            dbProviderFactoryDic.Add(connectionString, provider);
        }
        public static void RegisterProviderFactory(string connectionString,DateProvider provider=DateProvider.SqlServer) {
            Check.ConnectionConfig(connectionString);
            Check.ArgumentNullException(provider);
            switch (provider)
            {
                case DateProvider.SqlServer:
                    RegisterProviderFactory(connectionString,new SqlerverProvider());
                    break;
                case DateProvider.Oracle:
                    RegisterProviderFactory(connectionString, new OracleProvider());
                    break;
                case DateProvider.MySql:
                    RegisterProviderFactory(connectionString, new MysqlProvider());
                    break;
                case DateProvider.Sqlite:
                    RegisterProviderFactory(connectionString, new SqliteProvider());
                    break;
                default:
                    RegisterProviderFactory(connectionString, new SqliteProvider());
                    break;
            }
        }
        #endregion

        #region 获取Provider
        public static BaseProvider GetProviderFatory(string connstr) {
            Check.ProviderIsNoRegister(connstr,c=> dbProviderFactoryDic.ContainsKey(c));
            return dbProviderFactoryDic[connstr];
        }
        #endregion
    }
    #region 数据库提供者
    public enum DateProvider
    {
        SqlServer,
        Oracle,
        MySql,
        Sqlite,
        SqlSserver2000
    }
    #endregion
}
